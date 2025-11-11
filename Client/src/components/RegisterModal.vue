<template>
  <div class="modal" @click="closeIfOutside">
    <div class="modal-content">
      <h2>Регистрация</h2>
      <form @submit.prevent="handleRegister">
        <div class="input-group">
          <input
            v-model="firstName"
            type="text"
            id="reg-name"
            placeholder="Ваше имя (4-32 символа)"
            required
          />
          <span id="name-error" class="error-text"></span>
        </div>
        <div class="input-group">
          <input
            v-model="email"
            type="email"
            id="reg-email"
            placeholder="Email (@gmail.com, @mail.ru, @yandex.ru)"
            required
          />
          <span id="email-error" class="error-text"></span>
        </div>
        <div class="input-group">
          <input
            v-model="password"
            type="password"
            id="reg-password"
            placeholder="Пароль (мин. 6 символов)"
            required
          />
          <span id="password-error" class="error-text"></span>
        </div>
        <div v-if="error" id="error-message" class="error-message">{{ error }}</div>
        <button type="submit" class="btn">Зарегистрироваться</button>
        <button type="button" class="btn" @click="$emit('close')">Закрыть</button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { authService } from '@/api/authService';

const email = ref('');
const password = ref('');
const firstName = ref('');
const lastName = ref('');
const error = ref('');

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