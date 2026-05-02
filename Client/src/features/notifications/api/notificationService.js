import apiClient from "@/shared/api/HttpClient.js";

export const notificationService = {
  getNotifications(take = 20) {
    return apiClient.get("/notifications", {
      params: { take },
    });
  },

  getUnreadCount() {
    return apiClient.get("/notifications/unread-count");
  },

  markAsRead(notificationId) {
    return apiClient.post(`/notifications/${notificationId}/read`);
  },
};
