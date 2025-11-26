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
        :disabled="isRateLimited"
      />
      <div v-if="error" class="error-message">{{ error }}</div>
      <div v-if="isRateLimited" class="rate-limit-message">
        Лимит попыток исчерпан. Пожалуйста, подождите {{ timeLeft }} секунд.
      </div>
      <div class="btn-group">
        <button
          class="btn"
          @click="handleConfirm"
          :disabled="isRateLimited"
        >
          Подтвердить
        </button>
        <button class="btn" @click="$emit('close')">Закрыть</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onUnmounted } from 'vue';
import { authService } from '@/api/authService';

const confirmCode = ref('');
const error = ref('');
const attemptCount = ref(0); // счётчик попыток
const isRateLimited = ref(false); // флаг лимита
const timeLeft = ref(0); // оставшееся время

const props = defineProps({
  email: {
    type: String,
    required: true,
  },
});

let timer = null;

const handleConfirm = async () => {
  if (isRateLimited.value) {
    error.value = 'Пожалуйста, подождите.';
    return;
  }

  try {
    await authService.confirmEmail({
      email: props.email,
      token: confirmCode.value,
    });
    alert('Email успешно подтверждён!');
    resetAttempts();
    $emit('close');
  } catch (err) {
    attemptCount.value++;
    error.value = err.response?.data?.message || 'Ошибка подтверждения';

    if (attemptCount.value >= 3) {
      isRateLimited.value = true;
      timeLeft.value = 300; // 5 минут = 300 секунд
      startTimer();
    }
  }
};

const startTimer = () => {
  timer = setInterval(() => {
    timeLeft.value--;
    if (timeLeft.value <= 0) {
      clearInterval(timer);
      resetAttempts();
    }
  }, 1000);
};

const resetAttempts = () => {
  attemptCount.value = 0;
  isRateLimited.value = false;
  timeLeft.value = 0;
  confirmCode.value = '';
  error.value = '';
  if (timer) {
    clearInterval(timer);
    timer = null;
  }
};

const closeIfOutside = (e) => {
  if (e.target.classList.contains('modal')) {
    $emit('close');
  }
};

defineEmits(['close']);

onUnmounted(() => {
  if (timer) {
    clearInterval(timer);
  }
});
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

.modal-content input:disabled {
  background-color: #f5f5f5;
  cursor: not-allowed;
}

.error-message {
  color: #ff4d4d;
  margin-bottom: 1rem;
}

.rate-limit-message {
  color: #ff4d4d;
  margin-bottom: 1rem;
  font-weight: bold;
}

.btn-group {
  display: flex;
  gap: 1rem;
  margin-top: 1rem;
}

.btn-group button {
  flex: 1;
}

.btn-group button:disabled {
  background-color: #ccc;
  cursor: not-allowed;
}
</style>