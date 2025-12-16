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

export const categoryService = {
  // Получить все категории
  async getCategories() {
    const response = await apiClient.get('/categories');
    return response;
  },

  // Получить категорию по ID
  async getCategoryById(id) {
    const response = await apiClient.get(`/categories/${id}`);
    return response;
  },

  // Создать новую категорию
  async createCategory(categoryData) {
    const response = await apiClient.post('/categories', categoryData);
    return response;
  },

  // Обновить категорию
  async updateCategory(id, categoryData) {
    const response = await apiClient.put(`/categories/${id}`, categoryData);
    return response;
  },

  // Удалить категорию
  async deleteCategory(id) {
    const response = await apiClient.delete(`/categories/${id}`);
    return response;
  },
};