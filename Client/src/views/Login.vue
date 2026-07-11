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
        <ReactivateAccountPanel
          :visible="showReactivatePanel"
          :email="email"
          :password="password"
          :message="reactivateMessage"
          @restored="clearReactivateState"
        />
        <button type="submit" class="btn">Войти</button>
      </form>
      <p class="helper-link">
        <router-link :to="confirmEmailLink">Не пришёл код / подтвердить email</router-link>
      </p>
      <p>
        Нет аккаунта? <router-link to="/register">Зарегистрироваться</router-link>
      </p>
    </div>
  </div>
</template>

<script setup>
import { computed, ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/features/auth/stores/authStore';
import { authService } from '@/features/auth/api/authApi';
import {
  requiresEmailConfirmationFromError,
  savePendingConfirmationEmail,
  saveConfirmationWarning,
} from '@/features/auth/utils/pendingEmailStorage.js';
import {
  isAccountDeletedError,
  getAccountDeletedMessage,
} from '@/features/auth/utils/authResponseUtils.js';
import ReactivateAccountPanel from '@/features/auth/components/ReactivateAccountPanel.vue';

const router = useRouter();
const authStore = useAuthStore();

const email = ref('');
const password = ref('');
const error = ref('');
const showReactivatePanel = ref(false);
const reactivateMessage = ref('');

const confirmEmailLink = computed(() => ({
  path: '/confirm-email',
  query: email.value ? { email: email.value } : undefined,
}));

const handleLogin = async () => {
  error.value = "";
  showReactivatePanel.value = false;

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
    if (requiresEmailConfirmationFromError(err)) {
      savePendingConfirmationEmail(email.value);
      saveConfirmationWarning(
        err.response?.data?.message ||
          "Подтвердите email, чтобы войти. Если код не пришёл, запросите его повторно."
      );
      await router.push(confirmEmailLink.value);
      return;
    }

    if (isAccountDeletedError(err)) {
      reactivateMessage.value = getAccountDeletedMessage(
        err,
        "Этот аккаунт был удалён. Вы можете восстановить его с тем же паролем."
      );
      showReactivatePanel.value = true;
      error.value = "";
      return;
    }

    console.error("Ошибка входа:", err);
    error.value = err.response?.data?.message || err.message || "Ошибка входа";
  }
};

function clearReactivateState() {
  showReactivatePanel.value = false;
  reactivateMessage.value = "";
  error.value = "";
}
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

.helper-link {
  margin-top: 0.75rem;
  text-align: center;
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