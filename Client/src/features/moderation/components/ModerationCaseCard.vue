<template>
  <article
    class="case-card"
    :class="priorityMeta.className"
    role="button"
    tabindex="0"
    @click="emit('open', moderationCase.id)"
    @keydown.enter="emit('open', moderationCase.id)"
  >
    <div class="case-card-header">
      <span class="priority-badge">{{ priorityMeta.label }}</span>
      <span class="status-badge">{{ statusLabel }}</span>
    </div>

    <h3 class="case-title">
      {{ targetLabel }}
    </h3>

    <div class="case-stats">
      <span>{{ moderationCase.reportCount }} жалоб</span>
      <span class="dot">•</span>
      <span>{{ moderationCase.uniqueReporterCount }} авторов жалоб</span>
    </div>

    <div class="case-dates">
      <span>Последняя: {{ formatDate(moderationCase.lastReportedAt) }}</span>
      <span class="dot">•</span>
      <span>Первая: {{ formatDate(moderationCase.firstReportedAt) }}</span>
    </div>
  </article>
</template>

<script setup>
import { computed } from "vue";
import {
  getPriorityMeta,
  MODERATION_STATUS,
} from "@/features/moderation/constants/moderationEnums.js";

const props = defineProps({
  moderationCase: { type: Object, required: true },
});

const emit = defineEmits(["open"]);

const priorityMeta = computed(() => getPriorityMeta(props.moderationCase.priority));

const statusLabel = computed(
  () => MODERATION_STATUS[props.moderationCase.status] ?? "Неизвестно"
);

const targetLabel = computed(() => {
  if (props.moderationCase.postId) {
    return `Пост #${props.moderationCase.postId}`;
  }
  if (props.moderationCase.commentId) {
    return `Комментарий #${props.moderationCase.commentId}`;
  }
  return `Тикет #${props.moderationCase.id}`;
});

function formatDate(isoDate) {
  if (!isoDate) return "—";
  return new Date(isoDate).toLocaleString("ru-RU", {
    day: "2-digit",
    month: "short",
    hour: "2-digit",
    minute: "2-digit",
  });
}
</script>

<style scoped>
.case-card {
  width: 100%;
  box-sizing: border-box;
  border-radius: 16px;
  border: 1px solid var(--border-color);
  padding: 1rem 1.1rem;
  background: var(--card-bg);
  cursor: pointer;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.case-card:hover {
  transform: translateY(-1px);
  box-shadow: 0 10px 24px rgba(0, 0, 0, 0.12);
}

.case-card.priority-critical {
  border-left: 5px solid #dc2626;
  background: linear-gradient(90deg, rgba(220, 38, 38, 0.1), transparent 40%);
}

.case-card.priority-high {
  border-left: 5px solid #ea580c;
  background: linear-gradient(90deg, rgba(234, 88, 12, 0.1), transparent 40%);
}

.case-card.priority-medium {
  border-left: 5px solid #d97706;
  background: linear-gradient(90deg, rgba(217, 119, 6, 0.08), transparent 40%);
}

.case-card.priority-low {
  border-left: 5px solid #64748b;
}

.case-card-header {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  flex-wrap: wrap;
  margin-bottom: 0.65rem;
}

.priority-badge,
.status-badge {
  display: inline-flex;
  align-items: center;
  border-radius: 999px;
  padding: 0.2rem 0.65rem;
  font-size: 0.78rem;
  font-weight: 600;
}

.priority-critical .priority-badge {
  background: rgba(220, 38, 38, 0.15);
  color: #ef4444;
}

.priority-high .priority-badge {
  background: rgba(234, 88, 12, 0.15);
  color: #f97316;
}

.priority-medium .priority-badge {
  background: rgba(217, 119, 6, 0.15);
  color: #f59e0b;
}

.priority-low .priority-badge {
  background: rgba(100, 116, 139, 0.15);
  color: #94a3b8;
}

.status-badge {
  background: var(--bg-secondary);
  color: var(--text-secondary);
}

.case-title {
  margin: 0 0 0.5rem;
  font-size: 1.05rem;
  color: var(--text-primary);
}

.case-stats,
.case-dates {
  display: flex;
  flex-wrap: wrap;
  gap: 0.35rem;
  color: var(--text-secondary);
  font-size: 0.88rem;
}

.case-stats {
  margin-bottom: 0.35rem;
}

.dot {
  opacity: 0.6;
}
</style>
