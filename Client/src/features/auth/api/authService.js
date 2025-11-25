import apiClient from '@/shared/api/client'

export const authService = {
  async register(userData) {
    const response = await apiClient.post('/auth/register', userData);
    return response.data;
  },

  async login(credentials) {
  const response = await apiClient.post('/auth/login', credentials);
  return response.data; // просто возвращает данные !
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