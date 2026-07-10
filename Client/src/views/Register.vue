<template>
  <div class="auth-container">
    <div class="auth-form">
      <h2>Регистрация</h2>
      <form @submit.prevent="handleRegister">
        <input
          v-model="email"
          type="email"
          placeholder="Email"
          required
        />
        <input
          v-model="password"
          type="password"
          placeholder="Пароль (мин. 6 символов)"
          required
        />
        <input
          v-model="userName"
          type="text"
          placeholder="Имя пользователя"
          required
        />
        <div v-if="error" class="error-message">{{ error }}</div>
        <button type="submit" class="btn">Зарегистрироваться</button>
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
import { authService } from '@/features/auth/api/authApi';
import { openEmailConfirmationAfterRegister } from '@/features/auth/utils/emailConfirmationNavigation.js';

const router = useRouter();

const email = ref('');
const password = ref('');
const userName = ref('');
const error = ref('');

const handleRegister = async () => {
  try {
    const result = await authService.register({
      email: email.value,
      password: password.value,
      userName: userName.value,
    });
    await openEmailConfirmationAfterRegister(router, email.value, result);
  } catch (err) {
    const status = err.response?.status;
    const data = err.response?.data;

    if (status === 409) {
      error.value = 'Пользователь с таким email уже зарегистрирован.';
    } else if (status === 400) {
      if (data?.errors?.length > 0) {
        // Переводим ошибки
        const translatedErrors = data.errors.map(msg => {
          if (msg.includes('already exists')) {
            return 'Пользователь с таким email уже зарегистрирован.';
          } else if (msg.includes('UserName')) {
            return 'Пользователь с таким именем пользователя уже существует.';
          } else if (msg.includes('non alphanumeric')) {
            return 'В пароле не хватает символа для обеспечения безопасности.';
          } else {
            return msg; // если нет перевода — оставляем как есть
          }
        });
        error.value = translatedErrors.join(', ');
      } else {
        error.value = data?.message || 'Ошибка регистрации.';
      }
    } else {
      error.value = 'Ошибка регистрации. Пожалуйста, попробуйте снова.';
    }
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
  background-color: var(--card-bg);
  color: var(--text-primary);
  border: 1px solid var(--border-color);
  padding: 2rem;
  border-radius: 12px;
  width: 100%;
  max-width: 400px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.auth-form h2 {
  margin-bottom: 1rem;
  text-align: center;
  color: var(--text-primary);
}

.auth-form input {
  width: 100%;
  padding: 0.75rem;
  margin-bottom: 1rem;
  border: 1px solid var(--border-color);
  border-radius: 4px;
  background: var(--bg-primary);
  color: var(--text-primary);
}

.auth-form input::placeholder {
  color: var(--text-secondary);
}

.auth-form .btn {
  width: 100%;
  padding: 0.75rem;
  background-color: var(--btn-primary-bg);
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

.auth-form .btn:hover {
  background-color: var(--btn-primary-hover);
}

.error-message {
  color: #ff4d4d;
  margin-bottom: 1rem;
}

.auth-form p {
  margin-top: 1rem;
  text-align: center;
  color: var(--text-secondary);
}

.auth-form a {
  color: var(--btn-primary-bg);
  text-decoration: none;
}

.auth-form a:hover {
  text-decoration: underline;
}
</style>