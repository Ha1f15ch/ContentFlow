<template>
  <header class="app-header">

    <div class="header-left">
      <RouterLink v-if="!isHomePage" to="/" class="home-link">
        ContentFlow
      </RouterLink>

      <div class="theme-toggle">
        <label class="switch">
          <input type="checkbox" v-model="isDarkTheme">
          <span class="slider"></span>
        </label>
        <span>Тёмная тема</span>
      </div>
    </div>

    <div class="auth-section">
      <div id="guest-actions" v-if="!authStore.isAuthenticated">
        <button class="btn" @click="goToLogin">Войти</button>
      </div>
      
      <div v-else class="user-area">
        <button class="btn create-post-btn" @click="goToCreatePost">Новый пост</button>

        <UserMenu 
          :userName="authStore.user?.userName || 'Пользователь'" 
          :avatarUrl="authStore.user?.avatarUrl"
        />
      </div>
    </div>
  </header>
</template>

<script setup>
  import { computed } from 'vue';
  import { useRouter, useRoute } from 'vue-router';
  import { useThemeStore } from '@/shared/stores/theme';
  import { useAuthStore } from '@/features/auth/stores/authStore';

  // Импортируем компонент UserMenu
  import UserMenu from '@/shared/components/UserMenu.vue';

  const router = useRouter();
  const route = useRoute();
  const themeStore = useThemeStore();
  const authStore = useAuthStore();

  const isHomePage = computed(() => route.path === '/');

  const isDarkTheme = computed({
    get: () => themeStore.isDark,
    set: (value) => {
      themeStore.isDark = value;
      themeStore.applyTheme();
      localStorage.setItem('theme', value ? 'dark' : 'light');
    },
  });

  const goToLogin = () => {
    router.push('/login');
  };

  const goToCreatePost = () => {
    // Проверка авторизации (на всякий случай)
    if (!authStore.isAuthenticated) {
      router.push('/login');
      return;
    }
    router.push('/create-post');
  };

</script>

<style scoped>

  .app-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    gap: 1rem;
  }

  .header-left {
    display: flex;
    align-items: center;
    gap: 1.25rem;
  }

  .home-link {
    text-decoration: none;
    color: var(--text-primary);
    font-weight: 600;
    padding: 0.45rem 0.8rem;
    border: 1px solid var(--border-color);
    border-radius: 8px;
    transition: background-color 0.2s, color 0.2s;
  }

  .home-link:hover {
    background: var(--bg-secondary);
  }

  .create-post-btn {
    margin-right: 10px; 
    background-color: #007bff; 
    color: white; 
    border: none; 
    padding: 8px 12px; 
    border-radius: 4px; 
    cursor: pointer; 
    font-size: 14px; 
  }

  .create-post-btn:hover {
    background-color: #0056b3; 
  }

  .auth-section {
    display: flex;
    align-items: center;
    justify-content: flex-end;
  }

  .user-area {
    display: flex;
    align-items: center;
    gap: 10px;
  }

  .theme-toggle span {
    color: var(--text-primary);
  }

</style>