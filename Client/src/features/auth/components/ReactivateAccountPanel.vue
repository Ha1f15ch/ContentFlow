<template>
  <div v-if="visible" class="reactivate-box">
    <p class="reactivate-text">{{ message }}</p>
    <button
      type="button"
      class="reactivate-btn"
      :disabled="submitting"
      @click="handleReactivate"
    >
      {{ submitting ? "Восстановление..." : "Восстановить аккаунт" }}
    </button>
    <p v-if="error" class="reactivate-error">{{ error }}</p>
  </div>
</template>

<script setup>
import { ref } from "vue";
import { useRouter } from "vue-router";
import { authService } from "@/features/auth/api/authApi.js";
import { useAuthStore } from "@/features/auth/stores/authStore.js";

const props = defineProps({
  visible: { type: Boolean, default: false },
  email: { type: String, required: true },
  password: { type: String, required: true },
  message: {
    type: String,
    default: "Этот аккаунт был удалён. Вы можете восстановить его с тем же паролем.",
  },
});

const emit = defineEmits(["restored"]);

const router = useRouter();
const authStore = useAuthStore();

const submitting = ref(false);
const error = ref("");

async function handleReactivate() {
  if (!props.email || !props.password) {
    error.value = "Введите email и пароль от удалённого аккаунта.";
    return;
  }

  submitting.value = true;
  error.value = "";

  try {
    const data = await authService.reactivateAccount({
      email: props.email,
      password: props.password,
    });

    const token = data?.accessToken ?? data?.token;
    if (!token) {
      error.value = "Не удалось получить токен после восстановления.";
      return;
    }

    authStore.setToken(token);
    await authStore.bootstrap();
    emit("restored");
    await router.push("/");
  } catch (err) {
    error.value =
      err?.response?.data?.message ||
      err?.response?.data?.errors ||
      "Не удалось восстановить аккаунт.";
  } finally {
    submitting.value = false;
  }
}
</script>

<style scoped>
.reactivate-box {
  margin: 0 0 1rem;
  padding: 0.85rem 1rem;
  border-radius: 10px;
  border: 1px solid rgba(234, 179, 8, 0.35);
  background: rgba(234, 179, 8, 0.1);
}

.reactivate-text {
  margin: 0 0 0.75rem;
  color: var(--text-primary);
  line-height: 1.5;
}

.reactivate-btn {
  border: none;
  border-radius: 8px;
  padding: 0.65rem 1rem;
  background: var(--btn-primary-bg);
  color: #fff;
  cursor: pointer;
  font-weight: 600;
}

.reactivate-btn:disabled {
  opacity: 0.65;
  cursor: not-allowed;
}

.reactivate-error {
  margin: 0.65rem 0 0;
  color: #ff4d4d;
}
</style>
