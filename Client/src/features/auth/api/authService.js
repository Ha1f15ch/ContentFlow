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
  // Вход пользователя
  async login(credentials) {
    const response = await apiClient.post('/auth/login', credentials);
    return response.data; // ← возвращает только data
  },

  // Регистрация пользователя
  async register(userData) {
    const response = await apiClient.post('/auth/register', userData);
    return response.data; // ← возвращает только data
  },

  // Подтверждение email
  async confirmEmail(data) {
    const response = await apiClient.post('/auth/confirm-email', data);
    return response.data; // ← возвращает только data
  },

  // Повторная отправка подтверждения
  async resendConfirmation(data) {
    const response = await apiClient.post('/auth/resend-confirmation', data);
    return response.data; // ← возвращает только data
  },

  // Выход
  logout() {
    localStorage.removeItem('token');
  },
};