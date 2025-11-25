<template>
  <div class="content">
    <h2>Защищённый контент</h2>
    <p v-if="authStore.isAuthenticated" id="protected-content">
      Привет, <span id="greeting-user">{{ authStore.user?.name || 'Пользователь' }}</span>! 
      Это защищённый контент, доступный только после входа.
    </p>
    <p v-else>Пожалуйста, авторизуйтесь.</p>
  </div>
</template>

<script setup>
import { useAuthStore } from '@/features/auth/stores/authStore';

const authStore = useAuthStore();
</script>