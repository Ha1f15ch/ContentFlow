<template>
  <div class="modal" @click="closeIfOutside">
    <div class="modal-content">
      <h2>Подтверждение регистрации</h2>
      <p>Мы отправили код на вашу почту. Введите его ниже.</p>
      <input
        v-model="confirmCode"
        type="text"
        placeholder="Введите код"
        required
      />
      <div v-if="error" class="error-message">{{ error }}</div>
      <div class="btn-group">
        <button class="btn" @click="handleConfirm">Подтвердить</button>
        <button class="btn" @click="$emit('close')">Закрыть</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { authService } from '@/api/authService';

const confirmCode = ref('');
const error = ref('');

// Получаем email через props
const props = defineProps({
  email: {
    type: String,
    required: true,
  },
});

const handleConfirm = async () => {
  try {
    await authService.confirmEmail({
      email: props.email, // используем email из props
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

<style scoped>
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

.btn-group {
  display: flex;
  gap: 1rem;
  margin-top: 1rem;
}

.btn-group button {
  flex: 1;
}
</style>