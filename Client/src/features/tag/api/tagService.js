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

export const tagService = {
  // Получить все теги
  async getTags() {
    const response = await apiClient.get('/tags');
    return response;
  },

  // Получить тег по ID
  async getTagById(id) {
    const response = await apiClient.get(`/tags/${id}`);
    return response;
  },

  // Создать новый тег
  async createTag(tagData) {
    const response = await apiClient.post('/tags', tagData);
    return response;
  },

  // Обновить тег
  async updateTag(id, tagData) {
    const response = await apiClient.put(`/tags/${id}`, tagData);
    return response;
  },

  // Удалить тег
  async deleteTag(id) {
    const response = await apiClient.delete(`/tags/${id}`);
    return response;
  },
};