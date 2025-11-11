import axios from 'axios';

const API_BASE_URL = 'http://localhost:5006/api';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
});

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

export const authService = {
  async register(userData) {
    const response = await apiClient.post('/auth/register', userData);
    return response.data;
  },

  async login(credentials) {
    const response = await apiClient.post('/auth/login', credentials);
    const { token } = response.data;
    if (token) {
      localStorage.setItem('token', token);
    }
    return response.data;
  },

  async confirmEmail(data) {
    const response = await apiClient.post('/auth/confirm-email', data);
    return response.data;
  },

  async resendConfirmation(data) {
    const response = await apiClient.post('/auth/resend-confirmation', data);
    return response.data;
  },

  logout() {
    localStorage.removeItem('token');
  },
};