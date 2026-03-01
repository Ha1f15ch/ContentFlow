import { defineStore } from "pinia";
import { getToken, setToken, clearToken } from "@/shared/api/TokenStorage.js";
import { userProfileService } from "@/features/userProfile/api/userProfileService";
import { authService } from "@/features/auth/api/authApi.js";

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

    async bootstrap() {
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
            await this.logout();
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
