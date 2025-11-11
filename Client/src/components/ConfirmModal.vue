<template>
  <div class="modal" @click="closeIfOutside">
    <div class="modal-content">
      <h2>Подтверждение регистрации</h2>
      <p>Мы отправили код на вашу почту. Введите его ниже.</p>
      <input v-model="confirmCode" type="text" placeholder="Введите код" required />
      <div v-if="error" class="error-message">{{ error }}</div>
      <button class="btn" @click="handleConfirm">Подтвердить</button>
      <button class="btn" @click="$emit('close')">Закрыть</button>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { authService } from '@/api/authService';

const confirmCode = ref('');
const error = ref('');
const email = ref(''); // предполагаем, что email будет передан при открытии модального окна

const handleConfirm = async () => {
  try {
    await authService.confirmEmail({
      email: email.value, // передай email, например, через props
      token: confirmCode.value,
    });
    alert('Email успешно подтверждён!');
    $emit('close');
  } catch (err) {
    error.value = err.response?.data?.message || 'Ошибка подтверждения';
  }
};

const closeIfOutside = (e) => {
  if (e.target.classList.contains('modal')) {
    $emit('close');
  }
};

defineEmits(['close']);
</script>