<template>
  <header class="app-header">
    <!-- Логотип -->
    <div class="logo">
      <router-link to="/">
        <span class="logo-text">ContentFlow</span>
      </router-link>
    </div>

    <!-- Поиск -->
    <div class="search-bar">
      <input type="text" placeholder="Find anything..." v-model="searchQuery" @keyup.enter="search" />
      <button class="search-btn" @click="search">
        <i class="icon-search">🔍</i>
      </button>
    </div>

    <!-- Правая часть: тема, авторизация -->
    <div class="header-actions">
      <!-- Переключатель темы -->
      <div class="theme-toggle">
        <label class="switch">
          <input type="checkbox" v-model="isDarkTheme">
          <span class="slider"></span>
        </label>
        <span>Тёмная тема</span>
      </div>

      <!-- Авторизация -->
      <div id="guest-actions" v-if="!authStore.isAuthenticated">
        <button class="btn" @click="goToLogin">Войти</button>
      </div>
      
      <div id="user-actions" class="user-info" v-else>
        <!-- ✅ Добавлена ссылка на метод goToCreatePost -->
        <button class="action-btn create-btn" title="Создать пост" @click="goToCreatePost">
          <i class="icon-create">+</i> Create
        </button>
        <button class="action-btn bell-btn" title="Уведомления">
          <i class="icon-bell">🔔</i>
        </button>
        <span id="username">{{ authStore.user?.userName || 'Пользователь' }}</span>
        <button class="btn logout-btn" @click="handleLogout">Выйти</button>
      </div>
    </div>
  </header>
</template>

<script setup>
import { ref, computed } from 'vue';
import { useRouter } from 'vue-router';
import { useThemeStore } from '@/shared/stores/theme';
import { useAuthStore } from '@/features/auth/stores/authStore';

const router = useRouter();
const themeStore = useThemeStore();
const authStore = useAuthStore();

const searchQuery = ref('');

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

const handleLogout = () => {
  authStore.logout();
};

// ✅ Новый метод
const goToCreatePost = () => {
  router.push('/create-post');
};

const search = () => {
  // Пока пусто — логику добавим позже
  console.log('Поиск:', searchQuery.value);
};
</script>

<style scoped>
.app-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.75rem 1.5rem;
  background-color: #1a1a1a;
  color: white;
  border-bottom: 1px solid #333;
}

.logo {
  font-size: 1.5rem;
  font-weight: bold;
}

.logo a {
  color: white;
  text-decoration: none;
}

.search-bar {
  display: flex;
  align-items: center;
  flex-grow: 1;
  max-width: 600px;
  margin: 0 1.5rem;
}

.search-bar input {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #444;
  border-radius: 4px 0 0 4px;
  background-color: #2d2d2d;
  color: white;
  font-size: 0.9rem;
  outline: none;
}

.search-bar input:focus {
  border-color: #007bff;
}

.search-btn {
  padding: 0.5rem 0.75rem;
  border: 1px solid #444;
  border-left: none;
  border-radius: 0 4px 4px 0;
  background-color: #2d2d2d;
  color: white;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 40px; /* Фиксированная ширина */
}

.search-btn:hover {
  background-color: #333;
}

.icon-search {
  font-size: 1.1rem;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.theme-toggle {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.user-info {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.btn {
  padding: 0.5rem 0.75rem;
  border: 1px solid #444;
  border-radius: 4px;
  background-color: #2d2d2d;
  color: white;
  cursor: pointer;
  font-size: 0.9rem;
}

.btn:hover {
  background-color: #333;
}

.action-btn {
  display: flex;
  align-items: center;
  gap: 0.25rem;
  padding: 0.5rem 0.75rem;
  border: 1px solid #444;
  border-radius: 4px;
  background-color: #2d2d2d;
  color: white;
  cursor: pointer;
  font-size: 0.9rem;
  transition: all 0.2s ease;
}

.action-btn:hover {
  background-color: #333;
  transform: translateY(-1px);
}

/* Стиль для кнопки "Create" */
.create-btn {
  background-color: #007bff;
  border-color: #007bff;
}

.create-btn:hover {
  background-color: #0056b3;
}

/* Стиль для колокольчика */
.bell-btn {
  padding: 0.5rem 0.6rem; /* Уменьшаем отступы */
}

.icon-bell {
  font-size: 1.1rem;
}

/* Анимация для всех кнопок */
.action-btn, .btn {
  transition: all 0.2s ease;
}

.action-btn:hover, .btn:hover {
  transform: translateY(-1px);
}
</style>