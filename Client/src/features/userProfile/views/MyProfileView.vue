<template>
    <div class="profile-page">
        <div v-if="loading" class="profile-state">
            <p>Загрузка профиля...</p>
        </div>

        <div v-else-if="error" class="profile-state error">
            {{ error }}
        </div>

        <div v-else-if="profile" class="profile-card">
            <div class="profile-header">
                <div class="avatar-section">
                    <div class="avatar-wrap">
                        <img
                            v-if="resolvedAvatarUrl"
                            :src="resolvedAvatarUrl"
                            alt="Аватар пользователя"
                            class="avatar"
                            @click="openAvatarPreview"
                        />
                        <div
                            v-else
                            class="avatar avatar-fallback"
                            @click="triggerAvatarUpload"
                        >
                            {{ initials }}
                        </div>
                    </div>
            
                    <div class="avatar-actions">
                        <button
                            class="action-btn primary"
                            type="button"
                            @click="triggerAvatarUpload"
                            :disabled="avatarUploading"
                        >
                            {{ avatarUploading ? "Загрузка..." : "Загрузить аватар" }}
                        </button>

                        <button
                            v-if="resolvedAvatarUrl"
                            class="action-btn secondary"
                            type="button"
                            @click="openAvatarPreview"
                        >
                            Посмотреть
                        </button>
                    </div>
                        
                    <input
                        ref="avatarInput"
                        type="file"
                        accept="image/png,image/jpeg,image/gif,image/webp"
                        class="hidden-file-input"
                        @change="onAvatarSelected"
                    />
            
                    <div v-if="avatarError" class="avatar-error">
                        {{ avatarError }}
                    </div>
                </div>
        
                <div class="profile-main">
                    <h1>{{ fullName }}</h1>
                    <p class="username">@{{ profile.userName }}</p>
                    <p v-if="profile.bio" class="bio">{{ profile.bio }}</p>
                </div>
            </div>
        
            <div class="tabs">
                <button
                    class="tab-btn"
                    :class="{ active: activeTab === 'personal' }"
                    @click="activeTab = 'personal'"
                >
                    Персональная информация
                </button>
        
                <button
                    class="tab-btn"
                    :class="{ active: activeTab === 'subscriptions' }"
                    @click="activeTab = 'subscriptions'"
                >
                    Подписки
                </button>
            </div>
        
            <div class="tab-content">
                <ProfilePersonalInfo
                    v-if="activeTab === 'personal' && profile"
                    :profile="profile"
                    @updated="handleProfileUpdated"
                />
        
                <ProfileSubscriptions
                    v-else-if="activeTab === 'subscriptions' && profile"
                    :subscription-info="profile.subscriptionInfo"
                    :followers="followers"
                    :following="following"
                    :subscriptions-loading="subscriptionsLoading"
                />
            </div>
        
            <div v-if="showAvatarPreview" class="avatar-modal" @click.self="closeAvatarPreview">
                <div class="avatar-modal-content">
                    <img :src="resolvedAvatarUrl" alt="Аватар пользователя" class="avatar-preview-image" />
                    <button class="action-btn primary" type="button" @click="closeAvatarPreview">
                        Закрыть
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<script setup>
    import { computed, onMounted, ref } from "vue";
    import { userProfileService } from "@/features/userProfile/api/userProfileService";
    import ProfilePersonalInfo from "@/features/userProfile/components/ProfilePersonalInfo.vue";
    import ProfileSubscriptions from "@/features/userProfile/components/ProfileSubscriptions.vue";
    import { useAuthStore } from "@/features/auth/stores/authStore";

    const authStore = useAuthStore();

    const loading = ref(true);
    const subscriptionsLoading = ref(false);
    const error = ref("");
    const profile = ref(null);
    const followers = ref([]);
    const following = ref([]);
    const activeTab = ref("personal");

    const avatarInput = ref(null);
    const avatarUploading = ref(false);
    const avatarError = ref("");
    const showAvatarPreview = ref(false);

    const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || "http://localhost:8080/api";
    const API_ORIGIN = API_BASE_URL.replace(/\/api\/?$/, "");

    const fullName = computed(() => {
        if(!profile.value) return "";

        const parts = [
            profile.value.lastName,
            profile.value.firstName,
            profile.value.middleName
        ].filter(Boolean);

        return parts.length ? parts.join(" ") : profile.value.userName;
    });

    const initials = computed(() => {
        if(!profile.value) return "U";

        const source = fullName.value?.trim() || profile.value.userName?.trim() || "U";

        const parts = source.split(/\s+/);
        const first = parts[0]?.[0] ?? "U";
        const second = parts.length > 1 ? parts[1]?.[0] ?? "" : "";

        return (first + second).toUpperCase();
    });

    const resolvedAvatarUrl = computed(() => {
        const raw = profile.value?.avatarUrl?.trim();
        if (!raw) return "";

        if (raw.startsWith("http://") || raw.startsWith("https://")) {
            return raw;
        }

        return `${API_ORIGIN}${raw}`;
    });

    function handleProfileUpdated(updatedProfile) {
        profile.value = updatedProfile;

        if (authStore.user) {
            authStore.user = {
                ...authStore.user,
                avatarUrl: updatedProfile.avatarUrl,
                userName: updatedProfile.userName,
            };
        }
    }

    function triggerAvatarUpload() {
        avatarInput.value?.click();
    }

    function openAvatarPreview() {
        if (!resolvedAvatarUrl.value) return;
        showAvatarPreview.value = true;
    }

    function closeAvatarPreview() {
        showAvatarPreview.value = false;
    }

    async function onAvatarSelected(event) {
        const file = event.target.files?.[0];
        if (!file) return;

        avatarError.value = "";

        try {
            avatarUploading.value = true;
            
            const response = await userProfileService.updateAvatar(file);
            profile.value = response.data;

            if (authStore.user) {
            authStore.user = {
                ...authStore.user,
                avatarUrl: response.data.avatarUrl,
            };
        }
        } catch (err) {
            console.error("Ошибка загрузки аватара:", err);
            avatarError.value = err?.response?.data?.message || "Не удалось загрузить аватар.";
        } finally {
            avatarUploading.value = false;

            if (avatarInput.value) {
                avatarInput.value.value = "";
            }
        }
    }

    onMounted(async () => {
        loading.value = true;
        error.value = "";

        try {
            const profileResponse = await userProfileService.getMe();
            profile.value = profileResponse.data;

            subscriptionsLoading.value = true;

            const [followersResp, followingResp] = await Promise.all([
                userProfileService.getMyFollowers(),
                userProfileService.getMyFollowing(),
            ]);

            followers.value = Array.isArray(followersResp.data) ? followersResp.data : [];
            following.value = Array.isArray(followingResp.data) ? followingResp.data : [];

        } catch (err) {
            console.error("Ошибка при загрузке профиля:", err);
            error.value = "Не удалось загрузить профиль. Пожалуйста, попробуйте позже.";
        } finally {
            loading.value = false;
            subscriptionsLoading.value = false;
        }
    });

</script>

<style scoped>

.profile-page {
  max-width: 980px;
  margin: 0 auto;
  padding: 24px;
}

.profile-state {
  text-align: center;
  padding: 32px;
  font-size: 18px;
}

.profile-state.error {
  color: #b00020;
}

.profile-card {
  background: var(--card-bg, #1f1f1f);
  border: 1px solid var(--border-color, #333);
  border-radius: 16px;
  padding: 24px;
}

.profile-header {
  display: flex;
  gap: 24px;
  align-items: flex-start;
  margin-bottom: 24px;
}

.avatar-wrap {
  flex-shrink: 0;
}

.avatar-error {
  color: #d93025;
  font-size: 14px;
}

.avatar {
  width: 112px;
  height: 112px;
  border-radius: 999px;
  object-fit: cover;
  display: block;
  cursor: pointer;
}

.avatar-fallback {
  display: flex;
  align-items: center;
  justify-content: center;
  background: #666;
  color: white;
  font-size: 32px;
  font-weight: 700;
  cursor: pointer;
}

.avatar-modal {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.65);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
  padding: 24px;
}

.avatar-modal-content {
  background: var(--bg-secondary);
  border: 1px solid var(--border-color);
  border-radius: 16px;
  padding: 20px;
  max-width: 90vw;
  max-height: 90vh;
  display: flex;
  flex-direction: column;
  gap: 16px;
  align-items: center;
}

.avatar-preview-image {
  max-width: min(520px, 80vw);
  max-height: 70vh;
  border-radius: 16px;
  display: block;
}

.avatar-actions {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.avatar-section {
  display: flex;
  flex-direction: column;
  gap: 12px;
  min-width: 160px;
}

.profile-main {
  flex: 1;
}

.profile-main h1 {
  margin: 0 0 8px;
  color: var(--text-primary);
}

.username {
  margin: 0 0 10px;
  color: var(--text-secondary);
}

.bio {
  margin: 0;
  line-height: 1.5;
  color: var(--text-primary);
}

.tabs {
  display: flex;
  gap: 10px;
  margin-top: 24px;
  margin-bottom: 20px;
  border-bottom: 1px solid var(--border-color, #333);
  padding-bottom: 12px;
}

.tab-btn {
  padding: 10px 14px;
  border: 1px solid var(--border-color, #444);
  background: transparent;
  color: inherit;
  border-radius: 10px;
  cursor: pointer;
}

.tab-btn.active {
  border-color: #4ea1ff;
  box-shadow: inset 0 0 0 1px #4ea1ff;
}

.tab-content {
  margin-top: 16px;
}

@media (max-width: 768px) {
  .profile-header {
    flex-direction: column;
  }

  .avatar-section {
    min-width: auto;
  }
}

.hidden-file-input {
  display: none;
}

.action-btn {
  border: 1px solid var(--border-color);
  background: transparent;
  color: var(--text-primary);
  border-radius: 10px;
  padding: 10px 14px;
  cursor: pointer;
  font: inherit;
}

.action-btn.primary {
  background: var(--btn-primary-bg);
  border-color: var(--btn-primary-bg);
  color: white;
}

.action-btn.secondary {
  background: transparent;
}

.action-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

</style>