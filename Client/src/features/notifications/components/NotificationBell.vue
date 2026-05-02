<template>
  <div ref="rootRef" class="notifications-wrap">
    <button
      class="notification-bell"
      type="button"
      aria-label="Уведомления"
      title="Уведомления"
      @click="toggleDropdown"
    >
      <svg
        class="bell-icon"
        viewBox="0 0 24 24"
        aria-hidden="true"
      >
        <path
          d="M12 22a2.6 2.6 0 0 0 2.45-1.75h-4.9A2.6 2.6 0 0 0 12 22Zm7-6.5-1.35-1.65V9.7a5.66 5.66 0 0 0-4.4-5.52V3.5a1.25 1.25 0 0 0-2.5 0v.68A5.66 5.66 0 0 0 6.35 9.7v4.15L5 15.5a1.05 1.05 0 0 0 .82 1.7h12.36A1.05 1.05 0 0 0 19 15.5Z"
          fill="currentColor"
        />
      </svg>

      <span
        v-if="unreadCount > 0"
        class="notification-badge"
      >
        {{ badgeText }}
      </span>
    </button>

    <div
      v-if="isOpen"
      class="notifications-dropdown"
      role="menu"
    >
      <div class="dropdown-header">
        <span>Уведомления</span>
        <span v-if="unreadCount > 0" class="dropdown-count">
          {{ unreadCount }}
        </span>
      </div>

      <div v-if="loading" class="dropdown-state">
        Загрузка...
      </div>

      <div v-else-if="notifications.length === 0" class="dropdown-state">
        Уведомлений пока нет.
      </div>

      <template v-else>
        <button
          v-for="notification in notifications"
          :key="notification.id"
          class="notification-item"
          :class="{ unread: !notification.isRead }"
          type="button"
          @click="openNotification(notification)"
        >
          <span class="notification-title">
            {{ getNotificationTitle(notification) }}
          </span>
          <span class="notification-text">
            {{ getNotificationText(notification) }}
          </span>
          <span class="notification-date">
            {{ formatDate(notification.createdAt) }}
          </span>
        </button>
      </template>
    </div>
  </div>
</template>

<script setup>
import { computed, onBeforeUnmount, onMounted, ref } from "vue";
import { useRouter } from "vue-router";
import { notificationService } from "@/features/notifications/api/notificationService";
import {
  startNotificationConnection,
  stopNotificationConnection,
} from "@/features/notifications/api/notificationRealtimeService";

const NOTIFICATION_TYPE = {
  NewPost: 1,
  NewSubscriber: 2,
};

const router = useRouter();
const rootRef = ref(null);
const unreadCount = ref(0);
const notifications = ref([]);
const isOpen = ref(false);
const loading = ref(false);

const badgeText = computed(() => {
  return unreadCount.value > 99 ? "99+" : String(unreadCount.value);
});

function normalizeNotification(notification) {
  return {
    ...notification,
    payload: typeof notification.payload === "string"
      ? JSON.parse(notification.payload)
      : (notification.payload ?? {}),
  };
}

async function loadUnreadCount() {
  try {
    const response = await notificationService.getUnreadCount();
    unreadCount.value = response.data?.unreadCount ?? 0;
  } catch (err) {
    console.warn("Failed to load unread notifications count", err);
    unreadCount.value = 0;
  }
}

async function loadNotifications() {
  loading.value = true;

  try {
    const response = await notificationService.getNotifications(20);
    notifications.value = (response.data ?? []).map(normalizeNotification);
  } catch (err) {
    console.warn("Failed to load notifications", err);
    notifications.value = [];
  } finally {
    loading.value = false;
  }
}

async function refreshNotifications() {
  await Promise.all([loadUnreadCount(), loadNotifications()]);
}

async function toggleDropdown() {
  isOpen.value = !isOpen.value;

  if (isOpen.value) {
    await refreshNotifications();
  }
}

function getNotificationTitle(notification) {
  if (notification.type === NOTIFICATION_TYPE.NewSubscriber) {
    return "Новый подписчик";
  }

  if (notification.type === NOTIFICATION_TYPE.NewPost) {
    return "Новый пост";
  }

  return "Уведомление";
}

function getNotificationText(notification) {
  const payload = notification.payload ?? {};

  if (notification.type === NOTIFICATION_TYPE.NewSubscriber) {
    const userName = payload.followerUserName ? `@${payload.followerUserName}` : "Пользователь";
    return `${userName} подписался на вас.`;
  }

  if (notification.type === NOTIFICATION_TYPE.NewPost) {
    const userName = payload.authorUserName ? `@${payload.authorUserName}` : "Пользователь";
    return `${userName} опубликовал новый пост.`;
  }

  return "Открыть уведомление.";
}

function formatDate(isoDate) {
  if (!isoDate) return "";

  return new Date(isoDate).toLocaleString("ru-RU", {
    day: "2-digit",
    month: "short",
    hour: "2-digit",
    minute: "2-digit",
  });
}

async function markAsRead(notification) {
  if (!notification.id || notification.isRead) return;

  try {
    await notificationService.markAsRead(notification.id);
    notification.isRead = true;
    unreadCount.value = Math.max(0, unreadCount.value - 1);
  } catch (err) {
    console.warn("Failed to mark notification as read", err);
  }
}

async function openNotification(notification) {
  await markAsRead(notification);
  isOpen.value = false;

  const payload = notification.payload ?? {};

  if (notification.type === NOTIFICATION_TYPE.NewSubscriber && payload.followerProfileId) {
    await router.push({
      name: "userProfile",
      params: { profileId: payload.followerProfileId },
    });
    return;
  }

  if (notification.type === NOTIFICATION_TYPE.NewPost && payload.postId) {
    await router.push({
      name: "home",
      query: { postId: payload.postId },
    });
  }
}

function handleRealtimeNotification() {
  refreshNotifications();
}

function handleDocumentClick(event) {
  if (!rootRef.value?.contains(event.target)) {
    isOpen.value = false;
  }
}

onMounted(async () => {
  await refreshNotifications();
  document.addEventListener("click", handleDocumentClick);

  try {
    await startNotificationConnection({
      onNotification: handleRealtimeNotification,
    });
  } catch (err) {
    console.warn("Failed to start notification realtime connection", err);
  }
});

onBeforeUnmount(async () => {
  document.removeEventListener("click", handleDocumentClick);
  await stopNotificationConnection();
});
</script>

<style scoped>
.notifications-wrap {
  position: relative;
}

.notification-bell {
  position: relative;
  width: 38px;
  height: 38px;
  border: 1px solid var(--border-color);
  border-radius: 999px;
  background: transparent;
  color: var(--text-primary);
  display: inline-flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  padding: 0;
}

.notification-bell:hover {
  background: var(--bg-secondary);
}

.bell-icon {
  width: 20px;
  height: 20px;
}

.notification-badge {
  position: absolute;
  top: -5px;
  right: -6px;
  min-width: 18px;
  height: 18px;
  padding: 0 5px;
  border-radius: 999px;
  background: #d93025;
  color: white;
  font-size: 11px;
  font-weight: 700;
  line-height: 18px;
  text-align: center;
  box-sizing: border-box;
}

.notifications-dropdown {
  position: absolute;
  top: calc(100% + 10px);
  right: 0;
  width: min(360px, calc(100vw - 24px));
  max-height: 420px;
  overflow-y: auto;
  background: var(--card-bg);
  border: 1px solid var(--border-color);
  border-radius: 16px;
  box-shadow: 0 16px 38px rgba(0, 0, 0, 0.22);
  z-index: 1200;
}

.dropdown-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.9rem 1rem;
  border-bottom: 1px solid var(--border-color);
  color: var(--text-primary);
  font-weight: 700;
}

.dropdown-count {
  min-width: 22px;
  height: 22px;
  padding: 0 7px;
  border-radius: 999px;
  background: #d93025;
  color: white;
  font-size: 12px;
  line-height: 22px;
  text-align: center;
}

.dropdown-state {
  padding: 1rem;
  color: var(--text-secondary);
  text-align: center;
}

.notification-item {
  width: 100%;
  border: none;
  border-bottom: 1px solid var(--border-color);
  background: transparent;
  color: var(--text-primary);
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  padding: 0.85rem 1rem;
  text-align: left;
  cursor: pointer;
}

.notification-item:hover {
  background: var(--bg-secondary);
}

.notification-item.unread {
  background: color-mix(in srgb, var(--btn-primary-bg) 12%, transparent);
}

.notification-title {
  font-weight: 700;
}

.notification-text {
  color: var(--text-secondary);
  font-size: 0.9rem;
  line-height: 1.35;
}

.notification-date {
  color: var(--text-secondary);
  font-size: 0.78rem;
  opacity: 0.8;
}
</style>
