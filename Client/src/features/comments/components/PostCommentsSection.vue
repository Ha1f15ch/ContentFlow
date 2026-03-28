<template>
  <section class="comments-card">
    <div class="comments-body">
      <div v-if="loading" class="comments-state">
        Загрузка комментариев...
      </div>

      <div v-else-if="error" class="comments-state comments-error">
        {{ error }}
      </div>

      <template v-else>
        <div v-if="isAuthenticated" class="comment-editor">
          <label class="comment-editor-label" for="new-comment">
            Добавить комментарий
          </label>

          <textarea
            id="new-comment"
            v-model="newCommentText"
            class="comment-textarea"
            rows="4"
            maxlength="2000"
            placeholder="Напишите комментарий..."
          />

          <div v-if="hasCommentText" class="comment-actions">
            <button
              class="comment-btn comment-btn-primary"
              :disabled="submitting"
              @click="submitComment"
            >
              {{ submitting ? "Отправка..." : "Отправить" }}
            </button>

            <button
              class="comment-btn comment-btn-secondary"
              :disabled="submitting"
              @click="clearComment"
            >
              Очистить
            </button>
          </div>
        </div>

        <div v-else class="auth-hint">
          Войдите, чтобы оставлять комментарии.
        </div>

        <div v-if="comments.length === 0" class="comments-empty">
          Комментариев пока нет.
        </div>

        <div v-else class="comments-list">
          <CommentThreadItemComponent
            v-for="comment in comments"
            :key="comment.id"
            :comment="comment"
            :depth="0"
          />
        </div>
      </template>
    </div>
  </section>
</template>

<script setup>
import { computed, ref, watch, onMounted } from "vue";
import { commentService } from "@/features/comments/api/commentService";
import CommentThreadItemComponent from "@/features/comments/components/CommentThreadItem.vue";

const props = defineProps({
  postId: { type: Number, required: true },
  isAuthenticated: { type: Boolean, default: false },
  initialCount: { type: Number, default: 0 },
});

const comments = ref([]);
const loaded = ref(false);
const loading = ref(false);
const submitting = ref(false);
const error = ref("");
const newCommentText = ref("");

const hasCommentText = computed(() => newCommentText.value.trim().length > 0);

function normalizeComments(items) {
  return [...(items ?? [])].sort((a, b) => {
    const first = new Date(b.createdAt ?? b.createdAtUtc ?? 0).getTime();
    const second = new Date(a.createdAt ?? a.createdAtUtc ?? 0).getTime();
    return first - second;
  });
}

async function loadComments() {
  if (!props.postId) return;

  loading.value = true;
  error.value = "";

  try {
    const response = await commentService.getComments(props.postId);
    comments.value = normalizeComments(response.data);
    loaded.value = true;
  } catch (e) {
    console.error("Ошибка загрузки комментариев", e);
    error.value =
      e?.response?.data?.message || "Не удалось загрузить комментарии.";
  } finally {
    loading.value = false;
  }
}

onMounted(() => {
  loadComments();
});

function clearComment() {
  newCommentText.value = "";
}

async function submitComment() {
  const content = newCommentText.value.trim();
  if (!content) return;

  submitting.value = true;
  error.value = "";

  try {
    await commentService.createComment(props.postId, {
      content,
      parentCommentId: null,
    });

    const optimisticComment = {
      id: `tmp-${Date.now()}`,
      content,
      createdAt: new Date().toISOString(),
      userName: "Вы",
      comments: [],
      isDeleted: false,
    };

    comments.value = [optimisticComment, ...comments.value];
    newCommentText.value = "";

    await loadComments();
  } catch (e) {
    console.error("Ошибка отправки комментария", e);
    error.value =
      e?.response?.data?.message || "Не удалось отправить комментарий.";
  } finally {
    submitting.value = false;
  }
}

watch(
  () => props.postId,
  () => {
    comments.value = [];
    loaded.value = false;
    loading.value = false;
    submitting.value = false;
    error.value = "";
    newCommentText.value = "";
    loadComments();
  }
);
</script>

<style scoped>
.comments-card {
  width: 100%;
  background: var(--card-bg);
  border: 1px solid var(--border-color);
  border-radius: 18px;
  padding: 1rem 1.1rem;
  box-sizing: border-box;
}

.comments-body {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  width: 100%;
}

.comments-state {
  padding: 1rem 0.25rem;
  color: var(--text-secondary);
}

.comments-error {
  color: #ff6b6b;
}

.comment-editor {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  padding: 1rem;
  border: 1px solid var(--border-color);
  border-radius: 14px;
  background: var(--bg-secondary);
}

.comment-editor-label {
  color: var(--text-primary);
  font-weight: 600;
}

.comment-textarea {
  width: 100%;
  min-height: 110px;
  resize: vertical;
  border-radius: 12px;
  border: 1px solid var(--border-color);
  background: var(--bg-primary);
  color: var(--text-primary);
  padding: 0.85rem 0.95rem;
  font: inherit;
  outline: none;
  transition: border-color 0.2s, box-shadow 0.2s;
}

.comment-textarea:focus {
  border-color: var(--btn-primary-bg);
  box-shadow: 0 0 0 3px rgba(0, 123, 255, 0.12);
}

.comment-actions {
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
}

.comment-btn {
  border: none;
  border-radius: 10px;
  padding: 0.65rem 1rem;
  cursor: pointer;
  font-weight: 600;
  transition: transform 0.2s, opacity 0.2s;
}

.comment-btn:hover:not(:disabled) {
  transform: translateY(-1px);
}

.comment-btn:disabled {
  opacity: 0.65;
  cursor: not-allowed;
}

.comment-btn-primary {
  background: var(--btn-primary-bg);
  color: white;
}

.comment-btn-secondary {
  background: transparent;
  color: var(--text-primary);
  border: 1px solid var(--border-color);
}

.comments-empty {
  color: var(--text-secondary);
  padding: 0.4rem 0.2rem;
}

.comments-list {
  display: flex;
  flex-direction: column;
  gap: 0.85rem;
  width: 100%;
}

.auth-hint {
  color: var(--text-secondary);
}
</style>