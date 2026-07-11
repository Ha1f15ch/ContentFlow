<template>
  <div class="settings-page">
    <div class="settings-card">
      <h1>Настройки</h1>
      <p class="lead">Управление аккаунтом и безопасностью.</p>

      <section class="settings-section">
        <h2>Аккаунт</h2>
        <p class="section-text">
          Email и имя пользователя можно посмотреть в
          <router-link to="/me">профиле</router-link>.
        </p>
      </section>

      <section class="settings-section danger-zone">
        <h2>Опасная зона</h2>
        <p class="section-text">
          Удаление профиля деактивирует аккаунт. Войти снова можно будет только
          после восстановления с тем же email и паролем.
        </p>

        <p v-if="error" class="error-message">{{ error }}</p>
        <p v-if="successMessage" class="success-message">{{ successMessage }}</p>

        <button
          type="button"
          class="danger-btn"
          :disabled="deleting"
          @click="showDeleteModal = true"
        >
          Удалить профиль
        </button>
      </section>
    </div>

    <DeleteProfileConfirmModal
      ref="deleteModalRef"
      v-model="showDeleteModal"
      @confirm="handleDeleteProfile"
    />
  </div>
</template>

<script setup>
import { ref } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/features/auth/stores/authStore";
import { userProfileService } from "@/features/userProfile/api/userProfileService";
import DeleteProfileConfirmModal from "@/features/settings/components/DeleteProfileConfirmModal.vue";

const router = useRouter();
const authStore = useAuthStore();

const showDeleteModal = ref(false);
const deleteModalRef = ref(null);
const deleting = ref(false);
const error = ref("");
const successMessage = ref("");

async function handleDeleteProfile() {
  const userId = authStore.user?.userId;
  if (!userId) {
    deleteModalRef.value?.setError("Не удалось определить пользователя. Обновите страницу.");
    return;
  }

  deleting.value = true;
  error.value = "";
  deleteModalRef.value?.setSubmitting(true);
  deleteModalRef.value?.setError("");

  try {
    const response = await userProfileService.deleteMe(userId);
    const result = response.data;

    if (!result?.isSuccess) {
      const message = result?.errorMessage || "Не удалось удалить профиль.";
      deleteModalRef.value?.setError(message);
      return;
    }

    showDeleteModal.value = false;
    await authStore.logout();
    await router.push("/");
  } catch (err) {
    const message =
      err?.response?.data?.message ||
      err?.response?.data?.errorMessage ||
      "Не удалось удалить профиль.";
    deleteModalRef.value?.setError(message);
  } finally {
    deleting.value = false;
    deleteModalRef.value?.setSubmitting(false);
  }
}
</script>

<style scoped>
.settings-page {
  display: flex;
  justify-content: center;
  padding: 2rem 1rem 3rem;
}

.settings-card {
  width: 100%;
  max-width: 720px;
  background: var(--card-bg);
  border: 1px solid var(--border-color);
  border-radius: 18px;
  padding: 1.5rem;
  box-sizing: border-box;
}

h1 {
  margin: 0 0 0.35rem;
}

.lead {
  margin: 0 0 1.5rem;
  color: var(--text-secondary);
}

.settings-section {
  padding: 1rem 0;
  border-top: 1px solid var(--border-color);
}

.settings-section:first-of-type {
  border-top: none;
  padding-top: 0;
}

h2 {
  margin: 0 0 0.65rem;
  font-size: 1.1rem;
}

.section-text {
  margin: 0;
  color: var(--text-secondary);
  line-height: 1.55;
}

.danger-zone {
  margin-top: 0.5rem;
  padding: 1rem;
  border: 1px solid rgba(176, 0, 32, 0.25);
  border-radius: 14px;
  background: rgba(176, 0, 32, 0.05);
}

.danger-zone h2 {
  color: #b00020;
}

.danger-btn {
  margin-top: 1rem;
  border: none;
  border-radius: 10px;
  padding: 0.7rem 1rem;
  background: #b00020;
  color: #fff;
  font-weight: 600;
  cursor: pointer;
}

.danger-btn:disabled {
  opacity: 0.65;
  cursor: not-allowed;
}

.error-message {
  color: #ff4d4d;
  margin: 0.75rem 0 0;
}

.success-message {
  color: #22c55e;
  margin: 0.75rem 0 0;
}

a {
  color: var(--btn-primary-bg);
}
</style>
