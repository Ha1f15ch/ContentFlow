<template>
  <header>
    <div class="theme-toggle">
      <label class="switch">
        <input type="checkbox" v-model="isDarkTheme">
        <span class="slider"></span>
      </label>
      <span>Тёмная тема</span>
    </div>

    <div class="auth-section">
      <div id="guest-actions" v-if="!authStore.isAuthenticated">
        <button class="btn" @click="emit('open-login')">Войти</button>
      </div>
      
      <div id="user-actions" class="user-info" v-else>
        <span id="username">{{ authStore.user?.name || 'Пользователь' }}</span>
        <button class="btn" @click="handleLogout">Выйти</button>
      </div>
    </div>
  </header>
</template>

<script setup>
import { computed } from 'vue';
import { useThemeStore } from '@/stores/theme';
import { useAuthStore } from '@/stores/auth';

const themeStore = useThemeStore();
const authStore = useAuthStore();

const isDarkTheme = computed({
  get: () => themeStore.isDark,
  set: (value) => {
    themeStore.isDark = value;
    themeStore.applyTheme();
    localStorage.setItem('theme', value ? 'dark' : 'light');
  },
});

const handleLogout = () => {
  authStore.logout();
};

// Для передачи событий в App.vue
const emit = defineEmits(['open-login']);
</script>