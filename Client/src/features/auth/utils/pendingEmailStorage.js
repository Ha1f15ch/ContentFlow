const PENDING_EMAIL_KEY = "cf_pending_confirmation_email";
const CONFIRMATION_WARNING_KEY = "cf_confirmation_warning";
const CONFIRMATION_SUCCESS_KEY = "cf_confirmation_success";
const CODE_SENT_KEY = "cf_confirmation_code_sent";

export function savePendingConfirmationEmail(email) {
  if (!email) return;
  localStorage.setItem(PENDING_EMAIL_KEY, email.trim());
}

export function getPendingConfirmationEmail() {
  return localStorage.getItem(PENDING_EMAIL_KEY)?.trim() ?? "";
}

export function clearPendingConfirmationEmail() {
  localStorage.removeItem(PENDING_EMAIL_KEY);
}

export function saveConfirmationWarning(message) {
  if (!message) return;
  sessionStorage.setItem(CONFIRMATION_WARNING_KEY, message);
}

export function consumeConfirmationWarning() {
  const message = sessionStorage.getItem(CONFIRMATION_WARNING_KEY)?.trim() ?? "";
  sessionStorage.removeItem(CONFIRMATION_WARNING_KEY);
  return message;
}

export function saveConfirmationSuccess(message) {
  if (!message) return;
  sessionStorage.setItem(CONFIRMATION_SUCCESS_KEY, message);
}

export function consumeConfirmationSuccess() {
  const message = sessionStorage.getItem(CONFIRMATION_SUCCESS_KEY)?.trim() ?? "";
  sessionStorage.removeItem(CONFIRMATION_SUCCESS_KEY);
  return message;
}

export function markConfirmationCodeSent() {
  sessionStorage.setItem(CODE_SENT_KEY, "1");
}

export function consumeConfirmationCodeSent() {
  const sent = sessionStorage.getItem(CODE_SENT_KEY) === "1";
  sessionStorage.removeItem(CODE_SENT_KEY);
  return sent;
}

export function requiresEmailConfirmationFromError(err) {
  return err?.response?.data?.requiresEmailConfirmation === true;
}
