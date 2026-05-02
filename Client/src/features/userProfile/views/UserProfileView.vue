<template>
  <div class="profile-page">
    <div v-if="loading" class="profile-state">
      Загрузка профиля...
    </div>

    <div v-else-if="error" class="profile-state error">
      {{ error }}
    </div>

    <section v-else-if="profile" class="profile-card">
      <ProfileHeader
        :profile="profile"
        :avatar-url="resolvedAvatarUrl"
      >
        <template #actions>
          <button
            v-if="!isOwnProfile"
            class="action-btn"
            :class="{ primary: !isSubscribed, secondary: isSubscribed }"
            type="button"
            :disabled="subscriptionSaving"
            @click="toggleSubscription"
          >
            {{ subscriptionButtonText }}
          </button>

          <button
            v-else
            class="action-btn secondary"
            type="button"
            @click="router.push('/me')"
          >
            Мой профиль
          </button>
        </template>

        <template #below>
          <p v-if="subscriptionError" class="subscription-error">
            {{ subscriptionError }}
          </p>
        </template>
      </ProfileHeader>

      <ProfileInfoGrid :profile="profile" />
    </section>
  </div>
</template>

<script setup>
import { computed, onMounted, ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import { useAuthStore } from "@/features/auth/stores/authStore";
import { userProfileService } from "@/features/userProfile/api/userProfileService";
import { subscriptionService } from "@/features/userProfile/api/subscriptionService";
import ProfileHeader from "@/features/userProfile/components/ProfileHeader.vue";
import ProfileInfoGrid from "@/features/userProfile/components/ProfileInfoGrid.vue";

const route = useRoute();
const router = useRouter();
const authStore = useAuthStore();

const loading = ref(true);
const error = ref("");
const profile = ref(null);
const subscriptionSaving = ref(false);
const subscriptionError = ref("");

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || "http://127.0.0.1:8080/api";
const API_ORIGIN = API_BASE_URL.replace(/\/api\/?$/, "");

const profileId = computed(() => {
  const id = Number(route.params.profileId);
  return Number.isNaN(id) ? null : id;
});

const resolvedAvatarUrl = computed(() => {
  const raw = profile.value?.avatarUrl?.trim();
  if (!raw) return "";

  if (raw.startsWith("http://") || raw.startsWith("https://")) {
    return raw;
  }

  return `${API_ORIGIN}${raw}`;
});

const isOwnProfile = computed(() => {
  return authStore.user?.id === profile.value?.id;
});

const isSubscribed = computed(() => {
  return profile.value?.subscriptionInfo?.isActive === true;
});

const subscriptionButtonText = computed(() => {
  if (subscriptionSaving.value) {
    return isSubscribed.value ? "Отписываемся..." : "Подписываемся...";
  }

  return isSubscribed.value ? "Отписаться" : "Подписаться";
});

async function loadProfile() {
  if (!authStore.isAuthenticated) {
    await router.replace("/login");
    return;
  }

  if (!profileId.value) {
    error.value = "Некорректный идентификатор профиля.";
    loading.value = false;
    return;
  }

  loading.value = true;
  error.value = "";
  subscriptionError.value = "";

  try {
    await authStore.bootstrap();
    const response = await userProfileService.getById(profileId.value);
    profile.value = response.data;
  } catch (err) {
    console.error("Ошибка загрузки профиля:", err);
    error.value = err?.response?.data?.message || "Не удалось загрузить профиль.";
  } finally {
    loading.value = false;
  }
}

async function toggleSubscription() {
  if (!profile.value || isOwnProfile.value || subscriptionSaving.value) return;

  subscriptionSaving.value = true;
  subscriptionError.value = "";

  try {
    if (isSubscribed.value) {
      await subscriptionService.unsubscribe(profile.value.id);
      profile.value = {
        ...profile.value,
        subscriptionInfo: null,
      };
      return;
    }

    await subscriptionService.subscribe(profile.value.id);
    profile.value = {
      ...profile.value,
      subscriptionInfo: {
        isActive: true,
        isPaused: false,
        notificationsEnabled: true,
        subscribedAt: new Date().toISOString(),
      },
    };
  } catch (err) {
    console.error("Ошибка изменения подписки:", err);
    subscriptionError.value =
      err?.response?.data?.message || "Не удалось изменить подписку.";
  } finally {
    subscriptionSaving.value = false;
  }
}

onMounted(loadProfile);

watch(
  () => route.params.profileId,
  () => {
    loadProfile();
  }
);
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

.profile-state.error,
.subscription-error {
  color: #d93025;
}

.profile-card {
  background: var(--card-bg, #1f1f1f);
  border: 1px solid var(--border-color, #333);
  border-radius: 16px;
  padding: 24px;
}

.action-btn {
  border: 1px solid var(--border-color);
  background: transparent;
  color: var(--text-primary);
  border-radius: 10px;
  padding: 10px 14px;
  cursor: pointer;
  font: inherit;
  white-space: nowrap;
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
