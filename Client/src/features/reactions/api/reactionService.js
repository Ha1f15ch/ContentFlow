import apiClient from "@/shared/api/HttpClient.js";

export const REACTION_TYPE = {
  Like: 0,
  Dislike: 1,
};

export const reactionService = {
  setPostReaction(postId, reactionType) {
    return apiClient.put(`/posts/${postId}/reaction`, {
      reactionType,
    });
  },

  removePostReaction(postId) {
    return apiClient.delete(`/posts/${postId}/reaction`);
  },

  setCommentReaction(commentId, reactionType) {
    return apiClient.put(`/comments/${commentId}/reaction`, {
      reactionType,
    });
  },

  removeCommentReaction(commentId) {
    return apiClient.delete(`/comments/${commentId}/reaction`);
  },
};
