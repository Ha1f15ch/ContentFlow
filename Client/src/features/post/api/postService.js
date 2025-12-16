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

export const postService = {
  // Получить все посты
  async getPosts() {
    const response = await apiClient.get('/posts');
    return response;
  },

  // Получить пост по ID
  async getPostById(id) {
    const response = await apiClient.get(`/posts/${id}`);
    return response;
  },

  // Создать новый пост
  async createPost(postData) {
    const response = await apiClient.post('/posts', postData);
    return response;
  },

  // Обновить пост
  async updatePost(id, postData) {
    const response = await apiClient.put(`/posts/${id}`, postData);
    return response;
  },

  // Удалить пост
  async deletePost(id) {
    const response = await apiClient.delete(`/posts/${id}`);
    return response;
  },
};