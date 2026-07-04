import apiClient from "@/shared/api/HttpClient.js";

export const moderationService = {
  getOpenCases(page = 1, pageSize = 10) {
    return apiClient.get("/moderation/cases", { params: { page, pageSize } });
  },

  getCaseById(caseId) {
    return apiClient.get(`/moderation/cases/${caseId}`);
  },

  takeInReview(caseId) {
    return apiClient.post(`/moderation/cases/${caseId}/take`);
  },

  resolve(caseId, payload) {
    return apiClient.post(`/moderation/cases/${caseId}/resolve`, payload);
  },

  dismiss(caseId, payload = {}) {
    return apiClient.post(`/moderation/cases/${caseId}/dismiss`, payload);
  },
};
