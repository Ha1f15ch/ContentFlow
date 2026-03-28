import apiClient from "@/shared/api/HttpClient.js";

export const commentService = {
  getComments(postId) {
    return apiClient.get(`/posts/${postId}/comments`);
  },

  createComment(postId, payload) {
    return apiClient.post(`/posts/${postId}/comments`, {
      content: payload.content,
      parentCommentId: payload.parentCommentId ?? null,
    });
  },

  updateComment(postId, commentId, payload) {
    return apiClient.put(`/posts/${postId}/comments/${commentId}`, {
      newCommentText: payload.newCommentText,
    });
  },

  deleteComment(postId, commentId) {
    return apiClient.delete(`/posts/${postId}/comments/${commentId}`);
  },
};