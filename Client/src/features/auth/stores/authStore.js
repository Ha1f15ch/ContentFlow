import { defineStore } from "pinia";
import { getToken, setToken, clearToken } from "@/shared/api/TokenStorage.js";
import { getRolesFromToken, isAccessTokenExpired } from "@/shared/utils/jwtUtils.js";
import { ensureFreshAccessToken } from "@/shared/api/HttpClient.js";
import { userProfileService } from "@/features/userProfile/api/userProfileService";
import { authService } from "@/features/auth/api/authApi.js";

export const useAuthStore = defineStore("auth", {
  state: () => ({
    token: getToken(),
    user: null,
    sessionReady: false,

    _mePromise: null,
    _initPromise: null,
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    isLoggedIn: (state) => state.sessionReady && !!state.token,
    roles: (state) => getRolesFromToken(state.token),
    canModerate: (state) => {
      const roles = getRolesFromToken(state.token);
      return roles.includes("Moderator") || roles.includes("Admin");
    },
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

    async initSession() {
      if (this._initPromise) return this._initPromise;

      this._initPromise = (async () => {
        try {
          await this.bootstrap();
        } catch (err) {
          console.warn("session init failed", err);
        } finally {
          this.sessionReady = true;
          this._initPromise = null;
        }
      })();

      return this._initPromise;
    },

    /**
     * Refresh access token before calling protected APIs when the stored
     * access token is missing-but-we-had-one / expired. Guests (no token ever)
     * skip refresh entirely and browse as anonymous.
     */
    async ensureSessionToken() {
      const current = this.token || getToken();

      // Never logged in (or already cleaned up) — stay anonymous, no refresh call.
      if (!current) {
        this.user = null;
        return null;
      }

      if (!isAccessTokenExpired(current)) {
        if (this.token !== current) {
          this.token = current;
        }
        return current;
      }

      // Access token exists but expired — try rotate via refresh cookie.
      try {
        return await ensureFreshAccessToken();
      } catch {
        this.clearToken();
        return null;
      }
    },

    async bootstrap() {
      if (this.user && this.token && !isAccessTokenExpired(this.token)) {
        return this.user;
      }

      const token = await this.ensureSessionToken();
      if (!token) {
        this.user = null;
        return null;
      }

      if (this._mePromise) return this._mePromise;

      this._mePromise = (async () => {
        try {
          const resp = await userProfileService.getMe();
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
