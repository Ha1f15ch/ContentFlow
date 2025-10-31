<template>
  <div>
    <h2>Вход</h2>
    <form @submit.prevent="handleLogin">
      <input v-model="email" type="email" placeholder="Email" required />
      <input v-model="password" type="password" placeholder="Пароль" required />
      <button type="submit">Войти</button>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { authService } from '@/services/authService';

const email = ref('');
const password = ref('');

const handleLogin = async () => {
  try {
    const response = await authService.login({
      email: email.value,
      password: password.value,
    });
    console.log('Успешный вход:', response);
    // Перенаправление, например:
    // router.push('/dashboard');
  } catch (error) {
    console.error('Ошибка входа:', error.response?.data || error.message);
  }
};
</script>