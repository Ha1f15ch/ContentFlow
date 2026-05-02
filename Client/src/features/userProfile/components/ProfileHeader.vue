<template>
  <div class="profile-header">
    <div class="avatar-section">
      <div class="avatar-wrap">
        <img
          v-if="avatarUrl"
          :src="avatarUrl"
          alt="Аватар пользователя"
          class="avatar"
          :class="{ clickable: avatarClickable }"
          @click="emitAvatarClick"
        />
        <div
          v-else
          class="avatar avatar-fallback"
          :class="{ clickable: fallbackClickable }"
          @click="emitFallbackClick"
        >
          {{ initials }}
        </div>
      </div>

      <slot name="avatar-actions" />
    </div>

    <div class="profile-main">
      <div class="profile-title-row">
        <div>
          <h1>{{ fullName }}</h1>
          <p class="username">@{{ profile.userName }}</p>
        </div>

        <slot name="actions" />
      </div>

      <p v-if="profile.bio" class="bio">{{ profile.bio }}</p>
      <slot name="below" />
    </div>
  </div>
</template>

<script setup>
import { computed } from "vue";

const props = defineProps({
  profile: {
    type: Object,
    required: true,
  },
  avatarUrl: {
    type: String,
    default: "",
  },
  avatarClickable: {
    type: Boolean,
    default: false,
  },
  fallbackClickable: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits(["avatar-click", "fallback-click"]);

const fullName = computed(() => {
  const parts = [
    props.profile.lastName,
    props.profile.firstName,
    props.profile.middleName,
  ].filter(Boolean);

  return parts.length ? parts.join(" ") : props.profile.userName;
});

const initials = computed(() => {
  const source = fullName.value?.trim() || props.profile.userName?.trim() || "U";
  const parts = source.split(/\s+/);
  const first = parts[0]?.[0] ?? "U";
  const second = parts.length > 1 ? parts[1]?.[0] ?? "" : "";

  return (first + second).toUpperCase();
});

function emitAvatarClick() {
  if (props.avatarClickable) {
    emit("avatar-click");
  }
}

function emitFallbackClick() {
  if (props.fallbackClickable) {
    emit("fallback-click");
  }
}
</script>

<style scoped>
.profile-header {
  display: flex;
  gap: 24px;
  align-items: flex-start;
  margin-bottom: 24px;
}

.avatar-section {
  display: flex;
  flex-direction: column;
  gap: 12px;
  min-width: 160px;
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

.avatar.clickable {
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
}

.profile-main {
  flex: 1;
}

.profile-title-row {
  display: flex;
  justify-content: space-between;
  gap: 16px;
  align-items: flex-start;
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

@media (max-width: 768px) {
  .profile-header,
  .profile-title-row {
    flex-direction: column;
  }

  .avatar-section {
    min-width: auto;
  }
}
</style>
