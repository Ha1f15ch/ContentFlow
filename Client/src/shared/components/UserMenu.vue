<!-- src/components/UserMenu.vue -->
<template>
  <div class="user-menu" @mouseenter="showMenu = true" @mouseleave="showMenu = false">
    <div class="avatar" @click="toggleMenu">
      <img :src="userAvatar" alt="Аватар" />
    </div>
    <div v-if="showMenu" class="menu">
      <div class="menu-item" @click="goToProfile">Профиль</div>
      <div class="menu-item" @click="goToSettings">Настройки</div>
      <div class="menu-item" @click="logout">Выйти</div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/features/auth/stores/authStore';

const router = useRouter();
const authStore = useAuthStore();

const showMenu = ref(false);
const userAvatar = ref('https://via.placeholder.com/40'); // замени на реальный URL

const toggleMenu = () => {
  showMenu.value = !showMenu.value;
};

const goToProfile = () => {
  router.push('/profile');
};

const goToSettings = () => {
  router.push('/settings');
};

const logout = () => {
  authStore.logout();
  router.push('/login');
};
</script>

<style scoped>
.user-menu {
  position: relative;
  display: inline-block;
}

.avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  overflow: hidden;
  cursor: pointer;
}

.avatar img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.menu {
  position: absolute;
  top: 50px;
  right: 0;
  background-color: white;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
  border-radius: 8px;
  z-index: 1000;
}

.menu-item {
  padding: 0.75rem 1.5rem;
  cursor: pointer;
}

.menu-item:hover {
  background-color: #f5f5f5;
}
</style>