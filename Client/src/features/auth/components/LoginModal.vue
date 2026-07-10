<template>
  <div class="modal" @click="closeIfOutside">
    <div class="modal-content">
      <h2>{{ isLoginMode ? 'Вход' : isCodeInputMode ? 'Подтверждение регистрации' : 'Регистрация' }}</h2>

      <!-- Переключение режимов -->
      <div class="tabs" v-if="!isCodeInputMode">
        <button
          :class="{ active: isLoginMode }"
          @click="isLoginMode = true"
        >
          Войти
        </button>
        <button
          :class="{ active: !isLoginMode }"
          @click="isLoginMode = false"
        >
          Регистрация
        </button>
      </div>

      <!-- Форма входа -->
      <form v-if="isLoginMode && !isCodeInputMode" @submit.prevent="handleLogin">
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
        <p class="helper-link">
          <button type="button" class="link-btn" @click="openConfirmationFromLogin">
            Не пришёл код / подтвердить email
          </button>
        </p>
        <div class="btn-group">
          <button type="submit" class="btn">Войти</button>
          <button type="button" class="btn" @click="closeModal">Закрыть</button>
        </div>
      </form>

      <!-- Форма регистрации -->
      <form v-else-if="!isCodeInputMode" @submit.prevent="handleRegister">
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
        <div class="btn-group">
          <button type="submit" class="btn">Зарегистрироваться</button>
          <button type="button" class="btn" @click="closeModal">Закрыть</button>
        </div>
      </form>

      <!-- Подтверждение email после регистрации -->
      <div v-else class="confirmation-wrap">
        <EmailConfirmationForm
          :email="email"
          :initial-warning="confirmationWarning"
          :initial-info="confirmationInfo"
          :skip-auto-send="skipConfirmationAutoSend"
          @confirmed="handleEmailConfirmed"
          @already-confirmed="handleEmailAlreadyConfirmed"
        />
        <div class="btn-group">
          <button type="button" class="btn" @click="closeModal">Закрыть</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '@/features/auth/stores/authStore';
import { useModalStore } from '@/shared/stores/modalStore';
import { authService } from '@/features/auth/api/authApi';
import EmailConfirmationForm from '@/features/auth/components/EmailConfirmationForm.vue';
import {
  requiresEmailConfirmationFromError,
  savePendingConfirmationEmail,
} from '@/features/auth/utils/pendingEmailStorage.js';
import { showEmailConfirmationInModal } from '@/features/auth/utils/emailConfirmationNavigation.js';

const authStore = useAuthStore();
const modalStore = useModalStore();
const router = useRouter();

const isLoginMode = ref(true);
const isCodeInputMode = ref(false); // новое состояние
const email = ref('');
const password = ref('');
const userName = ref('');
const error = ref('');
const confirmationWarning = ref('');
const confirmationInfo = ref('');
const skipConfirmationAutoSend = ref(true);

const handleLogin = async () => {
  try {
    const data = await authService.login({
      email: email.value,
      password: password.value,
    });

    const token = data.accessToken ?? data.token;
    if (token) {
      authStore.setToken(token);
      await authStore.bootstrap();
    }

    closeModal();
  } catch (err) {
    if (requiresEmailConfirmationFromError(err)) {
      savePendingConfirmationEmail(email.value);
      confirmationWarning.value =
        err.response?.data?.message ||
        "Подтвердите email, чтобы войти. Если код не пришёл, запросите его повторно.";
      isCodeInputMode.value = true;
      isLoginMode.value = false;
      error.value = '';
      return;
    }

    error.value = err.response?.data?.message || "Ошибка входа";
  }
};

// TODO Получать данные из authService после авторизации пользователя. Обновлять authStore.user

const handleRegister = async () => {
  try {
    const result = await authService.register({
      email: email.value,
      password: password.value,
      userName: userName.value,
    });

    const { warning, info, skipAutoSend } = showEmailConfirmationInModal(email.value, result);
    confirmationWarning.value = warning;
    confirmationInfo.value = info;
    skipConfirmationAutoSend.value = skipAutoSend;
    isCodeInputMode.value = true;
    error.value = '';
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

const handleEmailConfirmed = () => {
  isCodeInputMode.value = false;
  isLoginMode.value = true;
  password.value = '';
  userName.value = '';
  confirmationWarning.value = '';
  confirmationInfo.value = '';
  skipConfirmationAutoSend.value = true;
  error.value = '';
};

const handleEmailAlreadyConfirmed = () => {
  isCodeInputMode.value = false;
  isLoginMode.value = true;
  confirmationWarning.value = '';
  error.value = 'Этот email уже подтверждён. Войдите в аккаунт.';
};

async function openConfirmationFromLogin() {
  savePendingConfirmationEmail(email.value);
  closeModal();
  await router.push({
    path: '/confirm-email',
    query: email.value ? { email: email.value } : undefined,
  });
}

const closeIfOutside = (e) => {
  if (e.target.classList.contains('modal')) {
    closeModal();
  }
};

// Закрываем через store — единообразно
const closeModal = () => {
  isCodeInputMode.value = false;
  isLoginMode.value = true;
  email.value = '';
  password.value = '';
  userName.value = '';
  confirmationWarning.value = '';
  confirmationInfo.value = '';
  skipConfirmationAutoSend.value = true;
  error.value = '';
  modalStore.closeModal();
};
</script>

<style scoped>
.tabs {
  display: flex;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.tabs button {
  padding: 0.75rem 1.5rem;
  background-color: var(--bg-secondary);
  color: var(--text-secondary);
  border: 1px solid var(--border-color);
  cursor: pointer;
  border-radius: 6px;
  font-size: 1rem;
  transition: background-color 0.2s, color 0.2s, border-color 0.2s;
}

.tabs button.active {
  background-color: var(--btn-primary-bg);
  border-color: var(--btn-primary-bg);
  color: white;
}

.tabs button:hover {
  background-color: var(--menu-hover);
  color: var(--text-primary);
}

.btn-group {
  display: flex;
  gap: 1rem; /* ← Увеличиваем расстояние между кнопками */
  margin-top: 1rem;
}

.btn-group button {
  flex: 1; /* ← Делаем кнопки равной ширины */
}

/* Остальные стили — без изменений */
.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.6);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 2000;
}

.confirmation-wrap {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.modal-content {
  background-color: var(--card-bg);
  color: var(--text-primary);
  border: 1px solid var(--border-color);
  padding: 2rem;
  border-radius: 12px;
  width: 90%;
  max-width: 400px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.modal-content h2 {
  margin-bottom: 1rem;
  color: var(--text-primary);
}

.modal-content input {
  width: 100%;
  padding: 0.75rem;
  margin-bottom: 1rem;
  border: 1px solid var(--border-color);
  border-radius: 4px;
  background: var(--bg-primary);
  color: var(--text-primary);
}

.modal-content input::placeholder {
  color: var(--text-secondary);
}

.modal-content input.error {
  border-color: #ff4d4d;
}

.error-message {
  color: #ff4d4d;
  margin-bottom: 1rem;
}

.helper-link {
  margin: -0.35rem 0 0.75rem;
  text-align: center;
}

.link-btn {
  border: none;
  background: transparent;
  color: var(--btn-primary-bg);
  cursor: pointer;
  font: inherit;
  text-decoration: underline;
  padding: 0;
}

.input-group {
  position: relative;
  margin-bottom: 1rem;
}

.error-text {
  color: #ff4d4d;
  font-size: 0.8rem;
  display: block;
}
</style>