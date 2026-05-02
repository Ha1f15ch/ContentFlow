<template>
  <div class="subscriptions">
    <div class="summary-cards">
      <div class="summary-card">
        <div class="summary-label">Подписчики</div>
        <div class="summary-value">
          {{ subscriptionInfo?.followersCount ?? followers.length }}
        </div>
      </div>

      <div class="summary-card">
        <div class="summary-label">Подписки</div>
        <div class="summary-value">
          {{ subscriptionInfo?.followingCount ?? following.length }}
        </div>
      </div>
    </div>

    <div v-if="subscriptionsLoading" class="loading">
      Загрузка подписок...
    </div>

    <div v-else class="lists">
      <div class="list-block">
        <h3>Мои подписчики</h3>

        <div v-if="!followers.length" class="empty">
          Пока нет подписчиков
        </div>

        <div v-else class="profile-list">
          <RouterLink
            v-for="item in followers"
            :key="item.subscriptionId ?? getProfileId(item)"
            class="profile-row"
            :to="{ name: 'userProfile', params: { profileId: getProfileId(item) } }"
          >
            <img
              v-if="getAvatarUrl(item)"
              :src="getAvatarUrl(item)"
              alt="Аватар пользователя"
              class="profile-avatar"
            />
            <div v-else class="profile-avatar avatar-fallback">
              {{ getInitials(item) }}
            </div>

            <div class="profile-row-main">
              <div class="name">
                @{{ getUserName(item) }}
              </div>
              <div class="meta">
                Подписан с {{ formatDate(item.subscribedAt) }}
              </div>
            </div>
          </RouterLink>
        </div>
      </div>

      <div class="list-block">
        <h3>Мои подписки</h3>

        <div v-if="!following.length" class="empty">
          Пока нет подписок
        </div>

        <div v-else class="profile-list">
          <RouterLink
            v-for="item in following"
            :key="item.subscriptionId ?? getProfileId(item)"
            class="profile-row"
            :to="{ name: 'userProfile', params: { profileId: getProfileId(item) } }"
          >
            <img
              v-if="getAvatarUrl(item)"
              :src="getAvatarUrl(item)"
              alt="Аватар пользователя"
              class="profile-avatar"
            />
            <div v-else class="profile-avatar avatar-fallback">
              {{ getInitials(item) }}
            </div>

            <div class="profile-row-main">
              <div class="name">
                @{{ getUserName(item) }}
              </div>
              <div class="meta">
                Подписка с {{ formatDate(item.subscribedAt) }}
              </div>
            </div>
          </RouterLink>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  subscriptionInfo: {
    type: Object,
    default: null,
  },
  followers: {
    type: Array,
    default: () => [],
  },
  following: {
    type: Array,
    default: () => [],
  },
  subscriptionsLoading: {
    type: Boolean,
    default: false,
  },
});

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || "http://127.0.0.1:8080/api";
const API_ORIGIN = API_BASE_URL.replace(/\/api\/?$/, "");

function getProfileId(item) {
  return item.followerProfileId ?? item.followingProfileId;
}

function getUserName(item) {
  return item.followerUserName || item.followingUserName || "user";
}

function getAvatarUrl(item) {
  const raw = (item.followerAvatarUrl || item.followingAvatarUrl || "").trim();
  if (!raw) return "";

  if (raw.startsWith("http://") || raw.startsWith("https://")) {
    return raw;
  }

  return `${API_ORIGIN}${raw}`;
}

function getInitials(item) {
  return getUserName(item).slice(0, 2).toUpperCase();
}

function formatDate(value) {
  if (!value) return "—";

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return "—";

  return date.toLocaleDateString("ru-RU");
}
</script>

<style scoped>
.summary-cards {
  display: flex;
  gap: 16px;
  margin-bottom: 24px;
}

.summary-card {
  flex: 1;
  padding: 16px;
  border: 1px solid var(--border-color, #333);
  border-radius: 12px;
  background: rgba(255, 255, 255, 0.03);
}

.summary-label {
  font-size: 13px;
  opacity: 0.75;
  margin-bottom: 8px;
}

.summary-value {
  font-size: 28px;
  font-weight: 700;
}

.loading,
.empty {
  opacity: 0.75;
}

.lists {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
}

.list-block {
  border: 1px solid var(--border-color, #333);
  border-radius: 12px;
  padding: 16px;
}

.profile-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.profile-row {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 12px;
  border-radius: 10px;
  background: rgba(255, 255, 255, 0.03);
  color: inherit;
  text-decoration: none;
  transition: background 0.2s, border-color 0.2s;
}

.profile-row:hover {
  background: rgba(255, 255, 255, 0.06);
}

.profile-avatar {
  width: 40px;
  height: 40px;
  border-radius: 999px;
  object-fit: cover;
  flex: 0 0 auto;
}

.avatar-fallback {
  display: flex;
  align-items: center;
  justify-content: center;
  background: #666;
  color: white;
  font-size: 13px;
  font-weight: 700;
}

.profile-row-main {
  min-width: 0;
}

.name {
  font-weight: 600;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.meta {
  font-size: 13px;
  opacity: 0.75;
  margin-top: 4px;
}
</style>