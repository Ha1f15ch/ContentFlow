import apiClient from "@/shared/api/HttpClient.js";

export const userProfileService = {
  getMe() {
    return apiClient.get("/userprofile/me");
  },

  getById(profileId) {
    return apiClient.get(`/userprofile/${profileId}`);
  },

  updateMe(profileData) {
    return apiClient.put("/userprofile", profileData);
  },

  deleteMe(userId) {
    return apiClient.delete("/userprofile", {
      data: { userId },
    });
  },

  getMyFollowers() {
    return apiClient.get("/userprofile/my-followers");
  },

  getMyFollowing() {
    return apiClient.get("/userprofile/my-following");
  },

  updateAvatar(file) {
    const formData = new FormData();
    formData.append("file", file);

    return apiClient.put("/userprofile/avatar", formData, {
      headers: {
        "Content-Type": "multipart/form-data",
      },
    });
  },

};
