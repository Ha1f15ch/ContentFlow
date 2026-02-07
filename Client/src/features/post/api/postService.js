import apiClient from "@/shared/api/HttpClient.js";

export const postService = {
  getPosts(params) {
    return apiClient.get("/posts", { params }); // используется фильтрация + пагинация
  },

  getPostById(id) {
    return apiClient.get(`/posts/${id}`);
  },

  createPost(postData) {
    return apiClient.post("/posts", postData);
  },

  updatePost(id, postData) {
    return apiClient.put(`/posts/${id}`, postData);
  },

  deletePost(id) {
    return apiClient.delete(`/posts/${id}`);
  },
};
