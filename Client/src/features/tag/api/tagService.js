import apiClient from "@/shared/api/HttpClient.js";

export const tagService = {
  getTags() {
    return apiClient.get("/tags");
  },

  getTagById(id) {
    return apiClient.get(`/tags/${id}`);
  },

  createTag(tagData) {
    return apiClient.post("/tags", tagData);
  },

  updateTag(id, tagData) {
    return apiClient.put(`/tags/${id}`, tagData);
  },

  deleteTag(id) {
    return apiClient.delete(`/tags/${id}`);
  },
};
