import apiClient from "@/shared/api/HttpClient.js";

export const authService = {
  async login(credentials) {
    const { data } = await apiClient.post("/auth/login", credentials);
    return data;
  },

  async register(userData) {
    const { data } = await apiClient.post("/auth/register", userData);
    return data;
  },

  async confirmEmail(payload) {
    const { data } = await apiClient.post("/auth/confirm-email", payload);
    return data;
  },

  async resendConfirmation(payload) {
    const { data } = await apiClient.post("/auth/resend-confirmation", payload);
    return data;
  },

  async logout() {
    const { data } = await apiClient.get("/auth/logout");
    return data;
  },
};
