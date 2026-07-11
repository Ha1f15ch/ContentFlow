export function extractAuthErrorMessage(err, fallback) {
  const data = err?.response?.data;
  if (data?.errors === "ResendCooldown") {
    return fallback;
  }
  return data?.errors ?? data?.message ?? fallback;
}

export function isEmailAlreadyConfirmedError(err) {
  return err?.response?.data?.emailAlreadyConfirmed === true;
}

export function isEmailAlreadyConfirmedResponse(data) {
  return data?.emailAlreadyConfirmed === true;
}

export function isResendCooldownError(err) {
  return err?.response?.data?.errors === "ResendCooldown";
}

export function getRetryAfterSeconds(err) {
  const value = err?.response?.data?.retryAfterSeconds;
  return typeof value === "number" && value > 0 ? value : 0;
}

export function isAccountDeletedError(err) {
  return err?.response?.data?.accountDeleted === true;
}

export function getAccountDeletedMessage(err, fallback) {
  return err?.response?.data?.message ?? fallback;
}
