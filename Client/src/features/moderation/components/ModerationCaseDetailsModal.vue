<template>
  <Teleport to="body">
    <div v-if="modelValue" class="modal-overlay" @click="close">
      <div class="modal-content" @click.stop>
        <button class="modal-close" type="button" @click="close">×</button>

        <div v-if="loading" class="modal-state">Загрузка тикета...</div>
        <div v-else-if="error" class="modal-state modal-error">{{ error }}</div>

        <div v-else-if="caseDetails" class="details-body">
          <div class="details-header">
            <h2 class="details-title">Тикет #{{ caseDetails.id }}</h2>
            <div class="badges">
              <span class="priority-badge" :class="priorityMeta.className">
                {{ priorityMeta.label }}
              </span>
              <span class="status-badge">
                {{ MODERATION_STATUS[caseDetails.status] ?? "—" }}
              </span>
            </div>
          </div>

          <div class="meta-grid">
            <div>
              <span class="meta-label">Объект</span>
              <span class="meta-value">{{ targetLabel }}</span>
            </div>
            <div>
              <span class="meta-label">Жалоб</span>
              <span class="meta-value">{{ caseDetails.reportCount }}</span>
            </div>
            <div>
              <span class="meta-label">Уникальных авторов</span>
              <span class="meta-value">{{ caseDetails.uniqueReporterCount }}</span>
            </div>
            <div>
              <span class="meta-label">Последняя жалоба</span>
              <span class="meta-value">{{ formatDate(caseDetails.lastReportedAt) }}</span>
            </div>
          </div>

          <div v-if="contentPreview" class="content-preview">
            <h3 class="section-title">Контент</h3>
            <p v-if="contentPreview.title" class="content-title">{{ contentPreview.title }}</p>
            <p class="content-text">{{ contentPreview.text }}</p>
          </div>

          <div class="reports-section">
            <h3 class="section-title">Жалобы ({{ caseDetails.reports?.length ?? 0 }})</h3>
            <div v-if="!caseDetails.reports?.length" class="empty-reports">
              Жалоб пока нет.
            </div>
            <article
              v-for="report in caseDetails.reports"
              :key="report.id"
              class="report-item"
            >
              <div class="report-top">
                <span class="report-reason">{{ getReportReasonLabel(report.reasonType) }}</span>
                <span class="report-date">{{ formatDate(report.createdAt) }}</span>
              </div>
              <p v-if="report.description" class="report-description">
                {{ report.description }}
              </p>
            </article>
          </div>

          <div v-if="isActionable" class="actions-section">
            <h3 class="section-title">Действия модератора</h3>

            <div v-if="caseDetails.status === 0" class="action-row">
              <button
                type="button"
                class="action-btn secondary"
                :disabled="actionPending"
                @click="takeInReview"
              >
                Взять в работу
              </button>
            </div>

            <template v-if="caseDetails.status === 0 || caseDetails.status === 1">
              <label class="field-label" for="moderation-decision">Решение</label>
              <select
                id="moderation-decision"
                v-model="selectedDecision"
                class="field-select"
              >
                <option
                  v-for="decision in MODERATION_DECISIONS"
                  :key="decision.value"
                  :value="decision.value"
                >
                  {{ decision.label }}
                </option>
              </select>

              <label class="field-label" for="moderation-note">Комментарий (необязательно)</label>
              <textarea
                id="moderation-note"
                v-model="moderatorNote"
                class="field-textarea"
                rows="3"
                maxlength="2000"
                placeholder="Заметка для истории модерации..."
              />

              <p v-if="actionError" class="form-error">{{ actionError }}</p>

              <div class="action-row">
                <button
                  type="button"
                  class="action-btn danger"
                  :disabled="actionPending"
                  @click="dismissCase"
                >
                  Отклонить тикет
                </button>
                <button
                  type="button"
                  class="action-btn primary"
                  :disabled="actionPending"
                  @click="resolveCase"
                >
                  Применить решение
                </button>
              </div>
            </template>
          </div>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import { ref, watch, computed, onMounted, onBeforeUnmount } from "vue";
import { moderationService } from "@/features/moderation/api/moderationService.js";
import { postService } from "@/features/post/api/postService.js";
import {
  MODERATION_DECISIONS,
  MODERATION_STATUS,
  getPriorityMeta,
} from "@/features/moderation/constants/moderationEnums.js";
import { getReportReasonLabel } from "@/features/reports/constants/reportReasons.js";

const props = defineProps({
  modelValue: { type: Boolean, default: false },
  caseId: { type: Number, default: null },
});

const emit = defineEmits(["update:modelValue", "updated"]);

const loading = ref(false);
const error = ref("");
const caseDetails = ref(null);
const contentPreview = ref(null);
const selectedDecision = ref(0);
const moderatorNote = ref("");
const actionPending = ref(false);
const actionError = ref("");

const priorityMeta = computed(() =>
  caseDetails.value ? getPriorityMeta(caseDetails.value.priority) : getPriorityMeta(0)
);

const targetLabel = computed(() => {
  if (!caseDetails.value) return "";
  if (caseDetails.value.postId) return `Пост #${caseDetails.value.postId}`;
  if (caseDetails.value.commentId) return `Комментарий #${caseDetails.value.commentId}`;
  return "—";
});

const isActionable = computed(
  () => caseDetails.value && (caseDetails.value.status === 0 || caseDetails.value.status === 1)
);

function close() {
  emit("update:modelValue", false);
}

function resetState() {
  caseDetails.value = null;
  contentPreview.value = null;
  error.value = "";
  loading.value = false;
  selectedDecision.value = 0;
  moderatorNote.value = "";
  actionPending.value = false;
  actionError.value = "";
}

async function loadContentPreview(details) {
  contentPreview.value = null;
  if (!details?.postId) return;

  try {
    const response = await postService.getPostById(details.postId);
    const post = response.data;
    contentPreview.value = {
      title: post.title,
      text: post.content,
    };
  } catch {
    contentPreview.value = null;
  }
}

async function loadCase() {
  if (!props.caseId) return;

  loading.value = true;
  error.value = "";

  try {
    const response = await moderationService.getCaseById(props.caseId);
    caseDetails.value = response.data;
    await loadContentPreview(response.data);
  } catch (e) {
    error.value = e?.response?.data?.message ?? "Не удалось загрузить тикет.";
  } finally {
    loading.value = false;
  }
}

async function takeInReview() {
  if (!props.caseId || actionPending.value) return;

  actionPending.value = true;
  actionError.value = "";

  try {
    await moderationService.takeInReview(props.caseId);
    await loadCase();
    emit("updated");
  } catch (e) {
    actionError.value = e?.response?.data?.message ?? "Не удалось взять тикет в работу.";
  } finally {
    actionPending.value = false;
  }
}

async function resolveCase() {
  if (!props.caseId || actionPending.value) return;

  actionPending.value = true;
  actionError.value = "";

  try {
    await moderationService.resolve(props.caseId, {
      decision: selectedDecision.value,
      note: moderatorNote.value.trim() || null,
    });
    emit("updated");
    close();
  } catch (e) {
    actionError.value = e?.response?.data?.message ?? "Не удалось применить решение.";
  } finally {
    actionPending.value = false;
  }
}

async function dismissCase() {
  if (!props.caseId || actionPending.value) return;

  actionPending.value = true;
  actionError.value = "";

  try {
    await moderationService.dismiss(props.caseId, {
      note: moderatorNote.value.trim() || null,
    });
    emit("updated");
    close();
  } catch (e) {
    actionError.value = e?.response?.data?.message ?? "Не удалось отклонить тикет.";
  } finally {
    actionPending.value = false;
  }
}

function formatDate(isoDate) {
  if (!isoDate) return "—";
  return new Date(isoDate).toLocaleString("ru-RU", {
    year: "numeric",
    month: "short",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}

watch(
  () => [props.modelValue, props.caseId],
  async ([isOpen, id]) => {
    if (!isOpen || !id) {
      resetState();
      document.body.style.overflow = "";
      document.body.style.paddingRight = "";
      return;
    }

    const scrollBarWidth = window.innerWidth - document.documentElement.clientWidth;
    document.body.style.overflow = "hidden";
    document.body.style.paddingRight = `${scrollBarWidth}px`;
    await loadCase();
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
  width: min(760px, calc(100vw - 32px));
  max-height: 92vh;
  overflow-y: auto;
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

.modal-state {
  padding: 2rem 0;
  text-align: center;
  color: var(--text-secondary);
}

.modal-error {
  color: #ef4444;
}

.details-header {
  display: flex;
  justify-content: space-between;
  gap: 1rem;
  flex-wrap: wrap;
  margin-bottom: 1rem;
  padding-right: 2rem;
}

.details-title {
  margin: 0;
  font-size: 1.4rem;
}

.badges {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
}

.priority-badge,
.status-badge {
  border-radius: 999px;
  padding: 0.25rem 0.7rem;
  font-size: 0.78rem;
  font-weight: 600;
}

.priority-badge.priority-critical {
  background: rgba(220, 38, 38, 0.15);
  color: #ef4444;
}

.priority-badge.priority-high {
  background: rgba(234, 88, 12, 0.15);
  color: #f97316;
}

.priority-badge.priority-medium {
  background: rgba(217, 119, 6, 0.15);
  color: #f59e0b;
}

.priority-badge.priority-low {
  background: rgba(100, 116, 139, 0.15);
  color: #94a3b8;
}

.status-badge {
  background: var(--bg-secondary);
  color: var(--text-secondary);
}

.meta-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
  gap: 0.75rem;
  margin-bottom: 1.25rem;
}

.meta-label {
  display: block;
  color: var(--text-secondary);
  font-size: 0.82rem;
  margin-bottom: 0.15rem;
}

.meta-value {
  color: var(--text-primary);
  font-weight: 600;
}

.section-title {
  margin: 0 0 0.75rem;
  font-size: 1rem;
}

.content-preview {
  margin-bottom: 1.25rem;
  padding: 0.9rem 1rem;
  border: 1px solid var(--border-color);
  border-radius: 12px;
  background: var(--card-bg);
}

.content-title {
  margin: 0 0 0.5rem;
  font-weight: 700;
}

.content-text {
  margin: 0;
  white-space: pre-line;
  line-height: 1.6;
  color: var(--text-secondary);
}

.reports-section {
  margin-bottom: 1.25rem;
}

.empty-reports {
  color: var(--text-secondary);
  font-size: 0.92rem;
}

.report-item {
  border: 1px solid var(--border-color);
  border-radius: 12px;
  padding: 0.75rem 0.9rem;
  background: var(--card-bg);
  margin-bottom: 0.55rem;
}

.report-top {
  display: flex;
  justify-content: space-between;
  gap: 0.75rem;
  flex-wrap: wrap;
  margin-bottom: 0.35rem;
}

.report-reason {
  font-weight: 600;
}

.report-date {
  color: var(--text-secondary);
  font-size: 0.85rem;
}

.report-description {
  margin: 0;
  color: var(--text-secondary);
  line-height: 1.5;
  white-space: pre-line;
}

.actions-section {
  border-top: 1px solid var(--border-color);
  padding-top: 1rem;
}

.field-label {
  display: block;
  margin: 0.75rem 0 0.35rem;
  color: var(--text-secondary);
  font-size: 0.88rem;
}

.field-select,
.field-textarea {
  width: 100%;
  box-sizing: border-box;
  border: 1px solid var(--border-color);
  border-radius: 12px;
  background: var(--card-bg);
  color: var(--text-primary);
  padding: 0.65rem 0.75rem;
  font: inherit;
}

.field-textarea {
  resize: vertical;
}

.form-error {
  margin: 0.75rem 0 0;
  color: #ef4444;
  font-size: 0.9rem;
}

.action-row {
  margin-top: 1rem;
  display: flex;
  justify-content: flex-end;
  gap: 0.65rem;
  flex-wrap: wrap;
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

.action-btn.danger {
  background: rgba(239, 68, 68, 0.12);
  color: #ef4444;
  border: 1px solid rgba(239, 68, 68, 0.35);
}

.action-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
</style>
