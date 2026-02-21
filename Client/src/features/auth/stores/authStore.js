import { defineStore } from "pinia";
import { getToken, setToken, clearToken } from "@/shared/api/TokenStorage.js";
import { userProfileService } from "@/features/userProfile/api/userProfileService";
import apiClient from "@/shared/api/HttpClient.js";

export const useAuthStore = defineStore("auth", {
  state: () => ({
    token: getToken(),
    user: null,

    // чтобы не дергать /me много раз параллельно
    _mePromise: null,
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
  },

  actions: {
    setToken(token) {
      this.token = token;
      setToken(token);
    },

    setUser(user) {
      this.user = user;
    },

    clearToken() {
      this.token = null;
      this.user = null;
      clearToken();
    },

    async logout() {
      try {
        await authService.logout();
      } catch (e) {
        console.warn("server logout failed", e);
      } finally {
        this.clearToken();
      }
    },

    /**
     * Грузит текущего пользователя (/userprofile/me) один раз.
     * Если нет токена — ничего не делает.
     * Если токен битый/протух — делает logout().
     */
    async bootstrap() {
      // если user уже есть
      if (this.user) return this.user;

      // если access токена нет — пробуем восстановить через refresh-cookie
      if (!this.token) {
        try {
          const resp = await apiClient.post("/auth/refresh");
          const newToken = resp.data?.token;
          if (newToken) this.setToken(newToken);
          else return null;
        } catch {
          this.user = null;
          return null;
        }
      }

      // уже грузим /me
      if (this._mePromise) return this._mePromise;

      this._mePromise = (async () => {
        try {
          const resp = await userProfileService.getMe();
          this.user = resp.data;
          return this.user;
        } catch (err) {
          const status = err?.response?.status;
          if (status === 401 || status === 403) {
            this.logout();
          }
          throw err;
        } finally {
          this._mePromise = null;
        }
      })();

      return this._mePromise;
    },
  },
});
