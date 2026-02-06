import { defineStore } from "pinia";
import { getToken, setToken, clearToken } from "@/shared/api/TokenStorage.js";
import { userProfileService } from "@/features/userProfile/api/userProfileService";

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

    logout() {
      this.clearToken();
    },

    /**
     * Грузит текущего пользователя (/userprofile/me) один раз.
     * Если нет токена — ничего не делает.
     * Если токен битый/протух — делает logout().
     */
    async bootstrap() {
      if (!this.token) {
        this.user = null;
        return null;
      }

      // уже загружено
      if (this.user) return this.user;

      // уже грузим
      if (this._mePromise) return this._mePromise;

      this._mePromise = (async () => {
        try {
          const resp = await userProfileService.getMe(); // axios response
          this.user = resp.data;
          return this.user;
        } catch (err) {
          // если токен невалиден — очищаем
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
