<template>
  <div class="auth-container">
    <div class="auth-form">
      <h2>Вход</h2>
      <form @submit.prevent="handleLogin">
        <input
          v-model="email"
          type="text"
          placeholder="Email"
          required
        />
        <input
          v-model="password"
          type="password"
          placeholder="Пароль"
          required
        />
        <div v-if="error" class="error-message">{{ error }}</div>
        <button type="submit" class="btn">Войти</button>
      </form>
      <p>
        Нет аккаунта? <router-link to="/register">Зарегистрироваться</router-link>
      </p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/features/auth/stores/authStore';
import { authService } from '@/features/auth/api/authApi';

const router = useRouter();
const authStore = useAuthStore();

const email = ref('');
const password = ref('');
const error = ref('');

const handleLogin = async () => {
  error.value = "";

  try {
    const data = await authService.login({ email: email.value, password: password.value });
    const token = data?.accessToken ?? data?.token;

    if (!token) {
      console.error("Login response:", data);
      error.value = "Ошибка входа: токен не получен.";
      return;
    }

    authStore.setToken(token);

    await authStore.bootstrap();

    router.push("/");
  } catch (err) {
    console.error("Ошибка входа:", err);
    error.value = err.response?.data?.message || err.message || "Ошибка входа";
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

.auth-form p {
  margin-top: 1rem;
  text-align: center;
}

.auth-form a {
  color: #007bff;
  text-decoration: none;
}

.auth-form a:hover {
  text-decoration: underline;
}
</style>