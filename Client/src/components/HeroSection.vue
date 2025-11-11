<template>
  <section class="hero" id="home">
    <h1>Добро пожаловать!</h1>
    <p id="welcome-message">
      {{ authStore.isAuthenticated ? 'Рады видеть!' : 'Пожалуйста, войдите, чтобы получить доступ к контенту.' }}
    </p>
  </section>
</template>

<script setup>
import { useAuthStore } from '@/stores/auth';

const authStore = useAuthStore();
</script>