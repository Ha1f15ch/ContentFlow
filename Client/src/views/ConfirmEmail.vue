<template>
  <div class="auth-container">
    <div class="auth-form">
      <h2>Подтверждение регистрации</h2>
      <p>Мы отправили код на вашу почту. Введите его ниже.</p>
      <form @submit.prevent="handleConfirm">
        <input
          v-model="confirmCode"
          type="text"
          placeholder="Введите код"
          required
        />
        <div v-if="error" class="error-message">{{ error }}</div>
        <button type="submit" class="btn">Подтвердить</button>
      </form>
      <p>
        Уже есть аккаунт? <router-link to="/login">Войти</router-link>
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { authService } from '@/features/auth/api/authService';

const router = useRouter();

const confirmCode = ref('');
const error = ref('');

const handleConfirm = async () => {
  try {
    await authService.confirmEmail({
      Email: localStorage.getItem('email'), // ← измени с email на Email
      Code: confirmCode.value, // ← измени с code на Code
    });
    alert('Email успешно подтверждён!');
    router.push('/login');
  } catch (err) {
    error.value = err.response?.data?.message || 'Ошибка подтверждения';
  }
};

</script>

<style scoped>
.auth-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 80vh;
}

.auth-form {
  background-color: white;
  padding: 2rem;
  border-radius: 8px;
  width: 100%;
  max-width: 400px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.auth-form h2 {
  margin-bottom: 1rem;
  text-align: center;
}

.auth-form p {
  margin-bottom: 1rem;
  text-align: center;
}

.auth-form input {
  width: 100%;
  padding: 0.75rem;
  margin-bottom: 1rem;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.auth-form .btn {
  width: 100%;
  padding: 0.75rem;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.auth-form .btn:hover {
  background-color: #0056b3;
}

.error-message {
  color: #ff4d4d;
  margin-bottom: 1rem;
}

.auth-form a {
  color: #007bff;
  text-decoration: none;
}

.auth-form a:hover {
  text-decoration: underline;
}
</style>