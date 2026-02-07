import apiClient from "@/shared/api/HttpClient.js";

export const userProfileService = {
  getMe() {
    return apiClient.get("/userprofile/me");
  },

  updateMe(profileData) {
    return apiClient.put("/userprofile", profileData);
  },

  deleteMe() {
    return apiClient.delete("/userprofile");
  },
};
