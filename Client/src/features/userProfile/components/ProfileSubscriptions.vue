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
          <div
            v-for="item in followers"
            :key="item.id ?? item.userId"
            class="profile-row"
          >
            <div class="name">
              {{ getDisplayName(item) }}
            </div>
            <div class="meta">
              @{{ item.userName }}
            </div>
          </div>
        </div>
      </div>

      <div class="list-block">
        <h3>Мои подписки</h3>

        <div v-if="!following.length" class="empty">
          Пока нет подписок
        </div>

        <div v-else class="profile-list">
          <div
            v-for="item in following"
            :key="item.id ?? item.userId"
            class="profile-row"
          >
            <div class="name">
              {{ getDisplayName(item) }}
            </div>
            <div class="meta">
              @{{ item.userName }}
            </div>
          </div>
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

function getDisplayName(item) {
  const parts = [item.lastName, item.firstName, item.middleName].filter(Boolean);
  return parts.length ? parts.join(" ") : item.userName || "Пользователь";
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
  padding: 12px;
  border-radius: 10px;
  background: rgba(255, 255, 255, 0.03);
}

.name {
  font-weight: 600;
}

.meta {
  font-size: 13px;
  opacity: 0.75;
  margin-top: 4px;
}
</style>