import { defineStore } from "pinia";
import { getToken, setToken, clearToken } from "@/shared/api/TokenStorage.js";
import { userProfileService } from "@/features/userProfile/api/userProfileService";
import { authService } from "@/features/auth/api/authApi.js";

export const useAuthStore = defineStore("auth", {
  state: () => ({
    token: getToken(),
    user: null,

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
      const hadToken = !!this.token;

      try {
        if (hadToken) {
          await authService.logout();
        }
      } catch (e) {
        console.warn("server logout failed", e);
      } finally {
        this.clearToken();
      }
    },

    async bootstrap() {
      if (!this.token) {
        this.user = null;
        return null;
      }

      if (this.user) return this.user;

      if (this._mePromise) return this._mePromise;

      this._mePromise = (async () => {
        try {
          const resp = await userProfileService.getMe(); // interceptor сам разрулит refresh
          this.user = resp.data;
          return this.user;
        } catch (err) {
          const status = err?.response?.status;
          if (status === 401 || status === 403) {
            this.clearToken();
            return null;
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
