<template>
  <Teleport to="body">
    <div v-if="modelValue" class="modal-overlay" @click="close">
      <div class="modal-content" @click.stop>
        <button class="modal-close" type="button" @click="close">×</button>

        <h3 class="modal-title">Пожаловаться</h3>
        <p class="modal-subtitle">
          Выберите причину жалобы. При необходимости добавьте описание.
        </p>

        <div class="reason-list">
          <label
            v-for="reason in REPORT_REASONS"
            :key="reason.value"
            class="reason-option"
            :class="{ active: selectedReason === reason.value }"
          >
            <input
              v-model="selectedReason"
              type="radio"
              name="report-reason"
              :value="reason.value"
            />
            <span>{{ reason.label }}</span>
          </label>
        </div>

        <label class="field-label" for="report-description">
          Описание (необязательно)
        </label>
        <textarea
          id="report-description"
          v-model="description"
          class="field-textarea"
          rows="4"
          maxlength="2000"
          placeholder="Опишите, что именно нарушает правила..."
        />

        <p v-if="error" class="form-error">{{ error }}</p>
        <p v-if="success" class="form-success">{{ success }}</p>

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
            class="action-btn primary"
            :disabled="submitting || selectedReason === null"
            @click="submit"
          >
            {{ submitting ? "Отправка..." : "Отправить жалобу" }}
          </button>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import { ref, watch, onMounted, onBeforeUnmount } from "vue";
import { REPORT_REASONS } from "@/features/reports/constants/reportReasons.js";
import { reportService } from "@/features/reports/api/reportService.js";

const props = defineProps({
  modelValue: { type: Boolean, default: false },
  targetType: { type: String, required: true },
  targetId: { type: Number, required: true },
});

const emit = defineEmits(["update:modelValue", "submitted"]);

const selectedReason = ref(null);
const description = ref("");
const submitting = ref(false);
const error = ref("");
const success = ref("");

function resetForm() {
  selectedReason.value = null;
  description.value = "";
  error.value = "";
  success.value = "";
  submitting.value = false;
}

function close() {
  emit("update:modelValue", false);
}

async function submit() {
  if (selectedReason.value === null || submitting.value) return;

  submitting.value = true;
  error.value = "";
  success.value = "";

  const payload = {
    reasonType: selectedReason.value,
    description: description.value.trim() || null,
  };

  try {
    if (props.targetType === "post") {
      await reportService.reportPost(props.targetId, payload);
    } else {
      await reportService.reportComment(props.targetId, payload);
    }

    success.value = "Жалоба отправлена. Спасибо!";
    emit("submitted");
    setTimeout(() => close(), 900);
  } catch (e) {
    error.value = e?.response?.data?.message ?? "Не удалось отправить жалобу.";
  } finally {
    submitting.value = false;
  }
}

watch(
  () => props.modelValue,
  (isOpen) => {
    if (!isOpen) {
      resetForm();
      document.body.style.overflow = "";
      document.body.style.paddingRight = "";
      return;
    }

    const scrollBarWidth = window.innerWidth - document.documentElement.clientWidth;
    document.body.style.overflow = "hidden";
    document.body.style.paddingRight = `${scrollBarWidth}px`;
  }
);

const onKeydown = (e) => {
  if (e.key === "Escape" && props.modelValue) close();
};

onMounted(() => window.addEventListener("keydown", onKeydown));
onBeforeUnmount(() => {
  window.removeEventListener("keydown", onKeydown);
  document.body.style.overflow = "";
  document.body.style.paddingRight = "";
});
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.55);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 16px;
  z-index: 1100;
}

.modal-content {
  position: relative;
  width: min(520px, calc(100vw - 32px));
  background: var(--bg-primary);
  color: var(--text-primary);
  border: 1px solid var(--border-color);
  border-radius: 18px;
  padding: 22px;
  box-shadow: 0 18px 50px rgba(0, 0, 0, 0.35);
}

.modal-close {
  position: absolute;
  top: 10px;
  right: 12px;
  border: none;
  background: transparent;
  color: var(--text-secondary);
  font-size: 1.8rem;
  cursor: pointer;
  line-height: 1;
}

.modal-title {
  margin: 0 0 0.35rem;
  font-size: 1.35rem;
}

.modal-subtitle {
  margin: 0 0 1rem;
  color: var(--text-secondary);
  font-size: 0.92rem;
  line-height: 1.5;
}

.reason-list {
  display: flex;
  flex-direction: column;
  gap: 0.45rem;
  margin-bottom: 1rem;
}

.reason-option {
  display: flex;
  align-items: center;
  gap: 0.65rem;
  padding: 0.65rem 0.75rem;
  border: 1px solid var(--border-color);
  border-radius: 12px;
  cursor: pointer;
  transition: border-color 0.2s, background 0.2s;
}

.reason-option.active {
  border-color: var(--btn-primary-bg);
  background: rgba(99, 102, 241, 0.08);
}

.field-label {
  display: block;
  margin-bottom: 0.4rem;
  color: var(--text-secondary);
  font-size: 0.88rem;
}

.field-textarea {
  width: 100%;
  box-sizing: border-box;
  border: 1px solid var(--border-color);
  border-radius: 12px;
  background: var(--card-bg);
  color: var(--text-primary);
  padding: 0.75rem;
  resize: vertical;
  font: inherit;
}

.form-error {
  margin: 0.75rem 0 0;
  color: #ef4444;
  font-size: 0.9rem;
}

.form-success {
  margin: 0.75rem 0 0;
  color: #22c55e;
  font-size: 0.9rem;
}

.modal-actions {
  margin-top: 1rem;
  display: flex;
  justify-content: flex-end;
  gap: 0.65rem;
}

.action-btn {
  border: none;
  border-radius: 10px;
  padding: 0.65rem 1rem;
  cursor: pointer;
  font: inherit;
}

.action-btn.primary {
  background: var(--btn-primary-bg);
  color: var(--btn-primary-text, #fff);
}

.action-btn.secondary {
  background: var(--bg-secondary);
  color: var(--text-primary);
  border: 1px solid var(--border-color);
}

.action-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
</style>
