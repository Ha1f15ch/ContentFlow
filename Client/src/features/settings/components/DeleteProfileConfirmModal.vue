<template>
  <Teleport to="body">
    <div v-if="modelValue" class="modal-overlay" @click="close">
      <div class="modal-content" @click.stop>
        <button class="modal-close" type="button" @click="close">×</button>

        <h3 class="modal-title">Удалить профиль?</h3>
        <p class="modal-subtitle">
          Это действие деактивирует ваш аккаунт. Вы не сможете войти, пока не
          восстановите профиль с тем же email и паролем.
        </p>

        <ul class="warning-list">
          <li>Посты и комментарии останутся в системе.</li>
          <li>Повторная регистрация с теми же данными будет недоступна.</li>
          <li>Восстановление возможно через вход или регистрацию.</li>
        </ul>

        <p v-if="error" class="form-error">{{ error }}</p>

        <div class="modal-actions">
          <button
            type="button"
            class="action-btn secondary"
            :disabled="submitting"
            @click="close"
          >
            Отмена
          </button>
          <button
            type="button"
            class="action-btn danger"
            :disabled="submitting"
            @click="confirm"
          >
            {{ submitting ? "Удаление..." : "Да, удалить профиль" }}
          </button>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import { ref, watch, onMounted, onBeforeUnmount } from "vue";

const props = defineProps({
  modelValue: { type: Boolean, default: false },
});

const emit = defineEmits(["update:modelValue", "confirm"]);

const submitting = ref(false);
const error = ref("");

function close() {
  if (submitting.value) return;
  emit("update:modelValue", false);
}

function confirm() {
  error.value = "";
  emit("confirm");
}

function setSubmitting(value) {
  submitting.value = value;
}

function setError(message) {
  error.value = message;
}

defineExpose({ setSubmitting, setError });

watch(
  () => props.modelValue,
  (open) => {
    if (open) {
      error.value = "";
      submitting.value = false;
    }
  }
);

function onKeydown(event) {
  if (event.key === "Escape" && props.modelValue) close();
}

onMounted(() => window.addEventListener("keydown", onKeydown));
onBeforeUnmount(() => window.removeEventListener("keydown", onKeydown));
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.55);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 12000;
  padding: 1rem;
}

.modal-content {
  position: relative;
  width: 100%;
  max-width: 480px;
  background: var(--card-bg);
  color: var(--text-primary);
  border: 1px solid var(--border-color);
  border-radius: 16px;
  padding: 1.5rem;
  box-shadow: 0 18px 40px rgba(0, 0, 0, 0.25);
}

.modal-close {
  position: absolute;
  top: 0.75rem;
  right: 0.85rem;
  border: none;
  background: transparent;
  color: var(--text-secondary);
  font-size: 1.5rem;
  cursor: pointer;
}

.modal-title {
  margin: 0 0 0.75rem;
}

.modal-subtitle,
.warning-list {
  color: var(--text-secondary);
  line-height: 1.55;
}

.modal-subtitle {
  margin: 0 0 0.85rem;
}

.warning-list {
  margin: 0 0 1rem;
  padding-left: 1.2rem;
}

.form-error {
  color: #ff4d4d;
  margin: 0 0 0.85rem;
}

.modal-actions {
  display: flex;
  gap: 0.75rem;
  justify-content: flex-end;
  flex-wrap: wrap;
}

.action-btn {
  border: none;
  border-radius: 10px;
  padding: 0.65rem 1rem;
  cursor: pointer;
  font-weight: 600;
}

.action-btn:disabled {
  opacity: 0.65;
  cursor: not-allowed;
}

.action-btn.secondary {
  background: transparent;
  color: var(--text-primary);
  border: 1px solid var(--border-color);
}

.action-btn.danger {
  background: #b00020;
  color: #fff;
}
</style>
