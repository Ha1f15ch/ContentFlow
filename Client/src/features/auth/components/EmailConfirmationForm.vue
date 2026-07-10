<template>
  <div class="confirmation-form">
    <div v-if="emailAlreadyConfirmed" class="already-confirmed">
      <p class="lead">
        Email <strong>{{ email }}</strong> уже подтверждён.
        Войдите в аккаунт с этим адресом.
      </p>
      <router-link to="/login" class="btn primary login-link">Войти</router-link>
    </div>

    <template v-else>
      <p class="lead">
        <template v-if="initialSendDone">
          Мы отправили шестизначный код на
          <strong>{{ email }}</strong>.
        </template>
        <template v-else>
          Отправляем код на <strong>{{ email }}</strong>...
        </template>
        Введите его ниже, чтобы завершить регистрацию.
      </p>

      <p v-if="warningMessage" class="warning-message">{{ warningMessage }}</p>
      <p v-if="successBanner" class="success-banner">{{ successBanner }}</p>

      <div class="help-box">
        <p class="help-title">Не пришло письмо?</p>
        <ul>
          <li>Проверьте папку «Спам» или «Промоакции».</li>
          <li>Подождите 1–2 минуты и запросите код повторно.</li>
          <li>Убедитесь, что адрес указан верно.</li>
        </ul>
      </div>

      <form @submit.prevent="handleConfirm">
        <label class="field-label" for="confirm-code">Код подтверждения</label>
        <input
          id="confirm-code"
          v-model="confirmCode"
          type="text"
          inputmode="numeric"
          autocomplete="one-time-code"
          maxlength="6"
          placeholder="123456"
          required
          :disabled="initialSendPending"
        />

        <p v-if="successMessage" class="success-message">{{ successMessage }}</p>
        <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>

        <button type="submit" class="btn primary" :disabled="submitting || initialSendPending">
          {{ submitting ? "Проверяем..." : "Подтвердить email" }}
        </button>
      </form>

      <button
        type="button"
        class="btn secondary"
        :disabled="resending || resendCooldownSeconds > 0 || initialSendPending"
        @click="handleResend"
      >
        {{
          resending
            ? "Отправляем..."
            : resendCooldownSeconds > 0
              ? `Отправить код повторно (${resendCooldownSeconds} сек.)`
              : "Отправить код повторно"
        }}
      </button>

      <p v-if="resendMessage" class="info-message">{{ resendMessage }}</p>
    </template>
  </div>
</template>

<script setup>
import { onMounted, onUnmounted, ref } from "vue";
import { authService } from "@/features/auth/api/authApi.js";
import { clearPendingConfirmationEmail } from "@/features/auth/utils/pendingEmailStorage.js";
import {
  extractAuthErrorMessage,
  isEmailAlreadyConfirmedError,
  isEmailAlreadyConfirmedResponse,
  isResendCooldownError,
  getRetryAfterSeconds,
} from "@/features/auth/utils/authResponseUtils.js";

const props = defineProps({
  email: { type: String, required: true },
  initialWarning: { type: String, default: "" },
  initialInfo: { type: String, default: "" },
  skipAutoSend: { type: Boolean, default: false },
});

const emit = defineEmits(["confirmed", "already-confirmed"]);

const confirmCode = ref("");
const submitting = ref(false);
const resending = ref(false);
const initialSendPending = ref(false);
const initialSendDone = ref(false);
const emailAlreadyConfirmed = ref(false);
const errorMessage = ref("");
const successMessage = ref("");
const resendMessage = ref("");
const warningMessage = ref(props.initialWarning || "");
const successBanner = ref("");
const resendCooldownSeconds = ref(0);

let cooldownTimer = null;
let successBannerTimer = null;

const RESEND_COOLDOWN_SECONDS = 30;

function showSuccessBanner(message, autoHideMs = 6000) {
  if (!message) return;
  successBanner.value = message;
  if (successBannerTimer) {
    window.clearTimeout(successBannerTimer);
  }
  successBannerTimer = window.setTimeout(() => {
    successBanner.value = "";
    successBannerTimer = null;
  }, autoHideMs);
}

function handleResendCooldown(err, { isInitial = false } = {}) {
  const retryAfterSeconds = getRetryAfterSeconds(err) || RESEND_COOLDOWN_SECONDS;
  initialSendDone.value = true;
  errorMessage.value = "";

  if (isInitial || props.skipAutoSend) {
    resendMessage.value = `Код уже отправлен на ${props.email}. Проверьте входящие.`;
  } else {
    resendMessage.value = `Повторная отправка будет доступна через ${retryAfterSeconds} сек.`;
  }

  startCooldown(retryAfterSeconds);
}

function startCooldown(seconds = RESEND_COOLDOWN_SECONDS) {
  resendCooldownSeconds.value = seconds;
  cooldownTimer = window.setInterval(() => {
    if (resendCooldownSeconds.value <= 1) {
      resendCooldownSeconds.value = 0;
      window.clearInterval(cooldownTimer);
      cooldownTimer = null;
      return;
    }
    resendCooldownSeconds.value -= 1;
  }, 1000);
}

function markAlreadyConfirmed() {
  emailAlreadyConfirmed.value = true;
  clearPendingConfirmationEmail();
  emit("already-confirmed");
}

async function requestConfirmationCode({ isInitial = false } = {}) {
  if (emailAlreadyConfirmed.value) return;

  if (isInitial) {
    initialSendPending.value = true;
  } else {
    resending.value = true;
  }

  errorMessage.value = "";
  if (!isInitial) {
    resendMessage.value = "";
  }

  try {
    const result = await authService.resendConfirmation({ email: props.email });

    if (isEmailAlreadyConfirmedResponse(result)) {
      markAlreadyConfirmed();
      return;
    }

    initialSendDone.value = true;
    if (isInitial) {
      resendMessage.value = `Код отправлен на ${props.email}.`;
      showSuccessBanner(`Код отправлен на ${props.email}.`);
    } else {
      resendMessage.value = `Новый код отправлен на ${props.email}.`;
      showSuccessBanner(`Новый код отправлен на ${props.email}.`);
    }
    startCooldown(RESEND_COOLDOWN_SECONDS);
  } catch (err) {
    if (isEmailAlreadyConfirmedError(err)) {
      markAlreadyConfirmed();
      return;
    }

    if (isResendCooldownError(err)) {
      handleResendCooldown(err, { isInitial });
      return;
    }

    errorMessage.value = extractAuthErrorMessage(
      err,
      isInitial
        ? "Не удалось отправить код. Попробуйте запросить его повторно."
        : "Не удалось отправить код повторно."
    );
  } finally {
    if (isInitial) {
      initialSendPending.value = false;
    } else {
      resending.value = false;
    }
  }
}

async function handleConfirm() {
  if (submitting.value || initialSendPending.value || emailAlreadyConfirmed.value) return;

  submitting.value = true;
  errorMessage.value = "";
  successMessage.value = "";

  try {
    const result = await authService.confirmEmail({
      email: props.email,
      code: confirmCode.value.trim(),
    });

    if (isEmailAlreadyConfirmedResponse(result)) {
      markAlreadyConfirmed();
      return;
    }

    clearPendingConfirmationEmail();
    successMessage.value = "Email подтверждён. Теперь можно войти.";
    emit("confirmed");
  } catch (err) {
    if (isEmailAlreadyConfirmedError(err)) {
      markAlreadyConfirmed();
      return;
    }

    errorMessage.value = extractAuthErrorMessage(err, "Неверный или просроченный код.");
  } finally {
    submitting.value = false;
  }
}

function handleResend() {
  if (resending.value || resendCooldownSeconds.value > 0 || initialSendPending.value) return;
  requestConfirmationCode({ isInitial: false });
}

onMounted(() => {
  if (props.initialInfo) {
    showSuccessBanner(props.initialInfo);
    initialSendDone.value = true;
    resendMessage.value = props.initialInfo;
    startCooldown(RESEND_COOLDOWN_SECONDS);
  }

  if (!props.skipAutoSend) {
    requestConfirmationCode({ isInitial: true });
  } else if (!props.initialInfo) {
    initialSendDone.value = true;
  }
});

onUnmounted(() => {
  if (cooldownTimer) {
    window.clearInterval(cooldownTimer);
  }
  if (successBannerTimer) {
    window.clearTimeout(successBannerTimer);
  }
});
</script>

<style scoped>
.confirmation-form {
  display: flex;
  flex-direction: column;
  gap: 0.85rem;
}

.lead {
  margin: 0;
  line-height: 1.55;
  color: var(--text-secondary);
}

.already-confirmed {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.login-link {
  display: block;
  text-align: center;
  text-decoration: none;
  box-sizing: border-box;
}

.help-box {
  border: 1px solid var(--border-color);
  border-radius: 10px;
  padding: 0.85rem 1rem;
  background: var(--bg-secondary);
}

.help-title {
  margin: 0 0 0.35rem;
  font-weight: 600;
  color: var(--text-primary);
}

.help-box ul {
  margin: 0;
  padding-left: 1.1rem;
  color: var(--text-secondary);
  line-height: 1.45;
}

.field-label {
  display: block;
  margin-bottom: 0.35rem;
  color: var(--text-secondary);
  font-size: 0.9rem;
}

input {
  width: 100%;
  box-sizing: border-box;
  padding: 0.75rem;
  margin-bottom: 0.35rem;
  border: 1px solid var(--border-color);
  border-radius: 6px;
  background: var(--bg-primary);
  color: var(--text-primary);
  letter-spacing: 0.08em;
  font-size: 1.05rem;
}

.btn {
  width: 100%;
  padding: 0.75rem;
  border-radius: 6px;
  border: none;
  cursor: pointer;
  font: inherit;
}

.btn.primary {
  background: var(--btn-primary-bg);
  color: #fff;
}

.btn.secondary {
  background: transparent;
  color: var(--text-primary);
  border: 1px solid var(--border-color);
}

.btn:disabled {
  opacity: 0.65;
  cursor: not-allowed;
}

.warning-message {
  margin: 0;
  padding: 0.75rem 0.9rem;
  border-radius: 8px;
  background: rgba(234, 179, 8, 0.12);
  border: 1px solid rgba(234, 179, 8, 0.35);
  color: #ca8a04;
  line-height: 1.45;
}

.success-banner {
  margin: 0;
  padding: 0.75rem 0.9rem;
  border-radius: 8px;
  background: rgba(34, 197, 94, 0.12);
  border: 1px solid rgba(34, 197, 94, 0.35);
  color: #16a34a;
  line-height: 1.45;
}

.error-message {
  color: #ff4d4d;
  margin: 0;
}

.success-message {
  color: #22c55e;
  margin: 0;
}

.info-message {
  color: var(--text-secondary);
  margin: 0;
  font-size: 0.92rem;
}
</style>
