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

export const userProfileService = {
  // Получить профиль текущего пользователя
  async getProfile() {
    const response = await apiClient.get('/userprofile');
    return response;
  },

  // Обновить профиль пользователя
  async updateProfile(profileData) {
    const response = await apiClient.put('/userprofile', profileData);
    return response;
  },

  // Удалить профиль (опционально)
  async deleteProfile() {
    const response = await apiClient.delete('/userprofile');
    return response;
  },
};