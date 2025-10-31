<template>
  <div>
    <h2>Регистрация</h2>
    <form @submit.prevent="handleRegister">
      <input v-model="email" type="email" placeholder="Email" required />
      <input v-model="password" type="password" placeholder="Пароль" required />
      <button type="submit">Зарегистрироваться</button>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { authService } from '@/services/authService';

const email = ref('');
const password = ref('');

const handleRegister = async () => {
  try {
    const response = await authService.register({
      email: email.value,
      password: password.value,
    });
    console.log('Успешная регистрация:', response);
    // Можно перенаправить на подтверждение email
  } catch (error) {
    console.error('Ошибка регистрации:', error.response?.data || error.message);
  }
};
</script>