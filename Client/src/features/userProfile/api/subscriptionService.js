import apiClient from "@/shared/api/HttpClient.js";

export const subscriptionService = {
  subscribe(followingProfileId) {
    return apiClient.post("/subscriptions", { followingProfileId });
  },

  unsubscribe(followingProfileId) {
    return apiClient.delete("/subscriptions", {
      data: { followingProfileId },
    });
  },

  pause(followingProfileId) {
    return apiClient.put("/subscriptions/pause", { followingProfileId });
  },

  resume(followingProfileId) {
    return apiClient.put("/subscriptions/resume", { followingProfileId });
  },

  enableNotifications(followingProfileId) {
    return apiClient.put("/subscriptions/enable-notifications", { followingProfileId });
  },

  disableNotifications(followingProfileId) {
    return apiClient.put("/subscriptions/disable-notifications", { followingProfileId });
  },
};
