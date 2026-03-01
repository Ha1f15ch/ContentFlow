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
        <button class="btn" @click="goToLogin">Войти</button>
      </div>
      
      <div v-else class="user-area">
        <button class="btn create-post-btn" @click="goToCreatePost">+ Create</button>

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
  import { useRouter } from 'vue-router';
  import { useThemeStore } from '@/shared/stores/theme';
  import { useAuthStore } from '@/features/auth/stores/authStore';

  // Импортируем компонент UserMenu
  import UserMenu from '@/shared/components/UserMenu.vue';

  const router = useRouter();
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