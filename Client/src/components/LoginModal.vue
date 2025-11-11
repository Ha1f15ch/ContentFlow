<template>
  <div class="modal" @click="closeIfOutside">
    <div class="modal-content">
      <h2>{{ isLoginMode ? 'Вход' : 'Регистрация' }}</h2>

      <!-- Переключение режимов -->
      <div class="tabs">
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
      <form v-if="isLoginMode" @submit.prevent="handleLogin">
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
        <div class="btn-group">
          <button type="submit" class="btn">Войти</button>
          <button type="button" class="btn" @click="$emit('close')">Закрыть</button>
        </div>
      </form>

      <!-- Форма регистрации -->
      <form v-else @submit.prevent="handleRegister">
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
          v-model="firstName"
          type="text"
          placeholder="Имя"
          required
        />
        <input
          v-model="lastName"
          type="text"
          placeholder="Фамилия"
          required
        />
        <div v-if="error" class="error-message">{{ error }}</div>
        <div class="btn-group">
          <button type="submit" class="btn">Зарегистрироваться</button>
          <button type="button" class="btn" @click="$emit('close')">Закрыть</button>
        </div>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { useAuthStore } from '@/stores/auth';
import { authService } from '@/api/authService';

const authStore = useAuthStore();

const isLoginMode = ref(true);

const email = ref('');
const password = ref('');
const firstName = ref('');
const lastName = ref('');
const error = ref('');

const handleLogin = async () => {
  try {
    let result = await authService.login({ email: email.value, password: password.value });
    
    console.log("Результат - ",result);
    
    //$emit('close');
  } catch (err) {
    error.value = err.response?.data?.message || 'Ошибка входа';
  }
};

const handleRegister = async () => {
  try {
    await authService.register({
      email: email.value,
      password: password.value,
      firstName: firstName.value,
      lastName: lastName.value,
    });
    alert('Регистрация успешна! Проверьте email для подтверждения.');
    $emit('close');
  } catch (err) {
    error.value = err.response?.data?.message || 'Ошибка регистрации';
  }
};

const closeIfOutside = (e) => {
  if (e.target.classList.contains('modal')) {
    $emit('close');
  }
};

defineEmits(['close']);
</script>

<style scoped>
.tabs {
  display: flex;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.tabs button {
  padding: 0.75rem 1.5rem;
  background-color: #eee;
  border: none;
  cursor: pointer;
  border-radius: 6px;
  font-size: 1rem;
  transition: background-color 0.2s;
}

.tabs button.active {
  background-color: #007bff;
  color: white;
}

.tabs button:hover {
  background-color: #ddd;
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
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background-color: white;
  padding: 2rem;
  border-radius: 8px;
  width: 90%;
  max-width: 400px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.modal-content h2 {
  margin-bottom: 1rem;
}

.modal-content input {
  width: 100%;
  padding: 0.75rem;
  margin-bottom: 1rem;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.modal-content input.error {
  border-color: #ff4d4d;
}

.error-message {
  color: #ff4d4d;
  margin-bottom: 1rem;
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