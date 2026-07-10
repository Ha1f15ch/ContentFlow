<template>
  <div class="auth-container">
    <div class="auth-form">
      <h2>Подтверждение email</h2>

      <div v-if="!activeEmail" class="email-entry">
        <p class="lead">
          Введите email, который вы использовали при регистрации.
          Мы отправим код подтверждения на указанный адрес.
        </p>

        <label class="field-label" for="confirm-email-input">Email</label>
        <input
          id="confirm-email-input"
          v-model="emailInput"
          type="email"
          autocomplete="email"
          placeholder="you@example.com"
          required
        />

        <p v-if="entryError" class="error-message">{{ entryError }}</p>
        <p v-if="emailAlreadyConfirmed" class="info-message">
          Этот email уже подтверждён.
          <router-link to="/login">Войти в аккаунт</router-link>
        </p>

        <button type="button" class="btn primary" :disabled="submitting" @click="startConfirmation">
          {{ submitting ? "Отправляем код..." : "Продолжить" }}
        </button>
      </div>

      <EmailConfirmationForm
        v-else
        :email="activeEmail"
        :initial-warning="initialWarning"
        :initial-info="initialInfo"
        :skip-auto-send="codeAlreadyRequested"
        @confirmed="handleConfirmed"
        @already-confirmed="handleAlreadyConfirmed"
      />

      <p class="footer-link">
        Уже подтвердили?
        <router-link to="/login">Войти</router-link>
      </p>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import EmailConfirmationForm from "@/features/auth/components/EmailConfirmationForm.vue";
import { authService } from "@/features/auth/api/authApi.js";
import {
  getPendingConfirmationEmail,
  savePendingConfirmationEmail,
  consumeConfirmationWarning,
  consumeConfirmationSuccess,
  consumeConfirmationCodeSent,
} from "@/features/auth/utils/pendingEmailStorage.js";
import {
  extractAuthErrorMessage,
  isEmailAlreadyConfirmedError,
  isEmailAlreadyConfirmedResponse,
  isResendCooldownError,
} from "@/features/auth/utils/authResponseUtils.js";

const route = useRoute();
const router = useRouter();

const emailInput = ref("");
const activeEmail = ref("");
const initialWarning = ref("");
const initialInfo = ref("");
const entryError = ref("");
const emailAlreadyConfirmed = ref(false);
const submitting = ref(false);
const codeAlreadyRequested = ref(false);

function resolveEmailFromRoute() {
  const fromQuery = typeof route.query.email === "string" ? route.query.email.trim() : "";
  return fromQuery || getPendingConfirmationEmail();
}

function syncFromRoute() {
  const resolved = resolveEmailFromRoute();
  if (resolved) {
    activeEmail.value = resolved;
    emailInput.value = resolved;
    savePendingConfirmationEmail(resolved);
  }
}

onMounted(() => {
  initialWarning.value = consumeConfirmationWarning();
  initialInfo.value = consumeConfirmationSuccess();
  if (consumeConfirmationCodeSent()) {
    codeAlreadyRequested.value = true;
  }
  syncFromRoute();
});

watch(
  () => route.query.email,
  () => syncFromRoute()
);

async function startConfirmation() {
  entryError.value = "";
  emailAlreadyConfirmed.value = false;
  const trimmed = emailInput.value.trim();

  if (!trimmed) {
    entryError.value = "Укажите email.";
    return;
  }

  submitting.value = true;

  try {
    const result = await authService.resendConfirmation({ email: trimmed });

    if (isEmailAlreadyConfirmedResponse(result)) {
      emailAlreadyConfirmed.value = true;
      return;
    }

    codeAlreadyRequested.value = true;
    activeEmail.value = trimmed;
    savePendingConfirmationEmail(trimmed);
    router.replace({ path: "/confirm-email", query: { email: trimmed } });
  } catch (err) {
    if (isEmailAlreadyConfirmedError(err)) {
      emailAlreadyConfirmed.value = true;
      return;
    }

    if (isResendCooldownError(err)) {
      codeAlreadyRequested.value = true;
      activeEmail.value = trimmed;
      savePendingConfirmationEmail(trimmed);
      initialInfo.value = "Код уже отправлен на вашу почту. Проверьте входящие.";
      router.replace({ path: "/confirm-email", query: { email: trimmed } });
      return;
    }

    entryError.value = extractAuthErrorMessage(err, "Не удалось отправить код подтверждения.");
  } finally {
    submitting.value = false;
  }
}

function handleAlreadyConfirmed() {
  activeEmail.value = "";
  codeAlreadyRequested.value = false;
  emailAlreadyConfirmed.value = true;
}

function handleConfirmed() {
  setTimeout(() => router.push("/login"), 1200);
}
</script>

<style scoped>
.auth-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 80vh;
  padding: 1rem;
}

.auth-form {
  background-color: var(--card-bg);
  color: var(--text-primary);
  border: 1px solid var(--border-color);
  padding: 2rem;
  border-radius: 12px;
  width: 100%;
  max-width: 460px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.auth-form h2 {
  margin: 0 0 1rem;
  text-align: center;
}

.lead,
.footer-link {
  text-align: center;
  color: var(--text-secondary);
  line-height: 1.5;
}

.lead {
  margin: 0 0 1rem;
}

.footer-link {
  margin-top: 1.25rem;
}

.email-entry {
  display: flex;
  flex-direction: column;
  gap: 0.65rem;
}

.field-label {
  color: var(--text-secondary);
  font-size: 0.9rem;
}

input {
  width: 100%;
  box-sizing: border-box;
  padding: 0.75rem;
  border: 1px solid var(--border-color);
  border-radius: 6px;
  background: var(--bg-primary);
  color: var(--text-primary);
}

.btn {
  width: 100%;
  padding: 0.75rem;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font: inherit;
}

.btn.primary {
  background: var(--btn-primary-bg);
  color: #fff;
}

.error-message {
  color: #ff4d4d;
  margin: 0;
}

.info-message {
  color: var(--text-secondary);
  margin: 0;
  text-align: center;
  line-height: 1.5;
}

.auth-form a {
  color: var(--btn-primary-bg);
  text-decoration: none;
}

.auth-form a:hover {
  text-decoration: underline;
}
</style>
