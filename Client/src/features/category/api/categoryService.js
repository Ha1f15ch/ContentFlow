import apiClient from "@/shared/api/HttpClient.js";

export const categoryService = {
  list({ page = 1, pageSize = 50 } = {}) {
    return apiClient.get("/categories", { params: { page, pageSize } });
  },
  getById(id) {
    return apiClient.get(`/categories/${id}`);
  },
};
