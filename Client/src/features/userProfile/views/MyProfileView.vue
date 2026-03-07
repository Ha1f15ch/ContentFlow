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
                <div class="avatar-wrap">
                    <img
                        v-if="profile.avatarUrl"
                        :src="profile.avatarUrl"
                        alt="Аватар пользователя"
                        class="avatar"
                    />
                    <div v-else class="avatar avatar-fallback">
                        {{ initials }}
                    </div>
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
    </div>
</template>

<script setup>
    import { computed, onMounted, ref } from "vue";
    import { userProfileService } from "@/features/userProfile/api/userProfileService";
    import ProfilePersonalInfo from "@/features/userProfile/components/ProfilePersonalInfo.vue";
    import ProfileSubscriptions from "@/features/userProfile/components/ProfileSubscriptions.vue";

    const loading = ref(true);
    const subscriptionsLoading = ref(false);
    const error = ref("");
    const profile = ref(null);
    const followers = ref([]);
    const following = ref([]);
    const activeTab = ref("personal");

    function handleProfileUpdated(updatedProfile) {
        profile.value = updatedProfile;
    }

    const fullName = computed(() => {
        if(!profile.value) return "";

        const parts = [
            profile.value.firstName,
            profile.value.lastName,
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
  gap: 20px;
  align-items: center;
  margin-bottom: 32px;
}

.avatar-wrap {
  flex-shrink: 0;
}

.avatar {
  width: 112px;
  height: 112px;
  border-radius: 999px;
  object-fit: cover;
  display: block;
}

.avatar-fallback {
  display: flex;
  align-items: center;
  justify-content: center;
  background: #666;
  color: white;
  font-size: 32px;
  font-weight: 700;
}

.profile-main h1 {
  margin: 0 0 8px;
}

.username {
  opacity: 0.8;
  margin: 0 0 10px;
}

.bio {
  margin: 0;
  line-height: 1.5;
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
</style>