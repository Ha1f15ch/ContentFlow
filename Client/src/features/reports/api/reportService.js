import apiClient from "@/shared/api/HttpClient.js";

export const reportService = {
  reportPost(postId, payload) {
    return apiClient.post(`/posts/${postId}/report`, payload);
  },

  reportComment(commentId, payload) {
    return apiClient.post(`/comments/${commentId}/report`, payload);
  },
};
