import axios from 'axios'
import {clearToken, getToken, setToken} from "@/shared/api/TokenStorage.js";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'http://127.0.0.1:8080/api'

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
  withCredentials: true
})

// аттачим access токен
apiClient.interceptors.request.use((config) => {
  const token = getToken();
  if (token) {
    config.headers = config.headers ?? {};
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config
});

// --- refresh logic (single-flight) generated part
let isRefreshing = false;
let refreshWaiters = [];

function addRefreshWaiter(resolve, reject) {
  refreshWaiters.push({ resolve, reject });
}

function resolveWaiters(newToken) {
  refreshWaiters.forEach((w) => w.resolve(newToken));
  refreshWaiters = [];
}

function rejectWaiters(err) {
  refreshWaiters.forEach((w) => w.reject(err));
  refreshWaiters = [];
}

async function refreshAccessTokenApi() {
  const resp = await apiClient.post("/auth/refresh", null);
  return resp.data; 
}

apiClient.interceptors.response.use(
  (resp) => resp,
  async (err) => {
    const originalRequest = err.config;

    if (!err.response) {
      return Promise.reject(err);
    }

    const status = err.response.status;
    const isAuthRefreshCall = originalRequest?.url?.includes("/auth/refresh");
    const hasToken = !!getToken();

    if (status !== 401 || originalRequest._retry || isAuthRefreshCall || !hasToken) {
      return Promise.reject(err);
    }

    originalRequest._retry = true;

    if (isRefreshing) {
      return new Promise((resolve, reject) => {
        addRefreshWaiter(
          (newToken) => {
            originalRequest.headers = originalRequest.headers ?? {};
            originalRequest.headers.Authorization = `Bearer ${newToken}`;
            resolve(apiClient(originalRequest));
          },
          reject
        );
      });
    }

    isRefreshing = true;

    try {
      const data = await refreshAccessTokenApi();
      const newToken = data.accessToken ?? data.token;

      if (!newToken) {
        throw new Error("Refresh did not return access token");
      }

      setToken(newToken);
      resolveWaiters(newToken);

      originalRequest.headers = originalRequest.headers ?? {};
      originalRequest.headers.Authorization = `Bearer ${newToken}`;

      return apiClient(originalRequest);
    } catch (refreshErr) {
      clearToken();
      rejectWaiters(refreshErr);
      return Promise.reject(refreshErr);
    } finally {
      isRefreshing = false;
    }
  }
);

export default apiClient