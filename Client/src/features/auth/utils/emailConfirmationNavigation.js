import {
  savePendingConfirmationEmail,
  saveConfirmationWarning,
  saveConfirmationSuccess,
  markConfirmationCodeSent,
} from "@/features/auth/utils/pendingEmailStorage.js";

const DEFAULT_SUCCESS_MESSAGE =
  "Код подтверждения отправлен на вашу почту. Проверьте входящие и папку «Спам».";

export function goToEmailConfirmation(router, email = "") {
  const trimmedEmail = email?.trim() ?? "";
  if (trimmedEmail) {
    savePendingConfirmationEmail(trimmedEmail);
  }

  return router.push({
    path: "/confirm-email",
    query: trimmedEmail ? { email: trimmedEmail } : undefined,
  });
}

export function openEmailConfirmationAfterRegister(router, email, result = {}) {
  savePendingConfirmationEmail(email);

  if (result.emailSent === false) {
    saveConfirmationWarning(
      result.message ||
        "Аккаунт создан, но письмо не удалось отправить. Запросите код повторно."
    );
  } else {
    markConfirmationCodeSent();
    saveConfirmationSuccess(DEFAULT_SUCCESS_MESSAGE);
  }

  return router.push({
    path: "/confirm-email",
    query: { email },
  });
}

export function showEmailConfirmationInModal(email, result = {}) {
  savePendingConfirmationEmail(email);

  if (result.emailSent === false) {
    return {
      warning:
        result.message ||
        "Аккаунт создан, но письмо не удалось отправить. Запросите код повторно.",
      info: "",
      skipAutoSend: false,
    };
  }

  markConfirmationCodeSent();
  return {
    warning: "",
    info: DEFAULT_SUCCESS_MESSAGE,
    skipAutoSend: true,
  };
}
