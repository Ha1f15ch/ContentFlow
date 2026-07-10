<template>
  <article
    class="comment-item"
    :class="{ 'comment-item-deleted': comment.isDeleted }"
    :style="{ marginLeft: `${depth * 20}px` }"
  >
    <div class="comment-top">
      <div class="comment-author">
        {{ comment.userName || "Пользователь" }}
      </div>

      <div class="comment-top-right">
        <ReportButton
          v-if="!comment.isDeleted"
          target-type="comment"
          :target-id="comment.id"
          :is-authenticated="isAuthenticated"
        />

        <div class="comment-date">
          {{ formatDate(comment.createdAt) }}
        </div>
      </div>
    </div>

    <div
      class="comment-content"
      :class="{ 'comment-content-deleted': comment.isDeleted }"
    >
      {{ comment.content }}
    </div>

    <div class="comment-actions">
      <ReactionBar
        :likes-count="comment.likesCount ?? 0"
        :dislikes-count="comment.dislikesCount ?? 0"
        :current-user-reaction="comment.currentUserReaction"
        :is-authenticated="isAuthenticated"
        :pending="pendingCommentId === comment.id"
        @set="(reactionType) => emit('set-reaction', { commentId: comment.id, reactionType })"
        @remove="emit('remove-reaction', comment.id)"
        @request-auth="emit('request-auth')"
      />

      <button
        v-if="isAuthenticated && !comment.isDeleted"
        type="button"
        class="reply-btn"
        @click="toggleReply"
      >
        {{ isReplying ? "Отмена" : "Ответить" }}
      </button>
    </div>

    <div v-if="isReplying" class="reply-editor">
      <textarea
        v-model="replyText"
        class="reply-textarea"
        rows="3"
        maxlength="2000"
        :placeholder="`Ответ для ${comment.userName || 'пользователя'}...`"
      />

      <div class="reply-actions">
        <button
          type="button"
          class="reply-btn reply-btn-primary"
          :disabled="!canSubmitReply || submittingReplyToId === comment.id"
          @click="submitReply"
        >
          {{ submittingReplyToId === comment.id ? "Отправка..." : "Отправить ответ" }}
        </button>
      </div>
    </div>

    <div
      v-if="comment.comments && comment.comments.length"
      class="comment-children"
    >
      <CommentThreadChild
        v-for="child in comment.comments"
        :key="child.id"
        :comment="child"
        :depth="depth + 1"
        :is-authenticated="isAuthenticated"
        :pending-comment-id="pendingCommentId"
        :submitting-reply-to-id="submittingReplyToId"
        @set-reaction="emit('set-reaction', $event)"
        @remove-reaction="emit('remove-reaction', $event)"
        @request-auth="emit('request-auth')"
        @submit-reply="emit('submit-reply', $event)"
      />
    </div>
  </article>
</template>

<script setup>
import { computed, ref } from "vue";
import CommentThreadChild from "./CommentThreadItem.vue";
import ReactionBar from "@/shared/components/ReactionBar.vue";
import ReportButton from "@/features/reports/components/ReportButton.vue";

const props = defineProps({
  comment: { type: Object, required: true },
  depth: { type: Number, default: 0 },
  isAuthenticated: { type: Boolean, default: false },
  pendingCommentId: { type: [Number, String, null], default: null },
  submittingReplyToId: { type: [Number, String, null], default: null },
});

const emit = defineEmits(["set-reaction", "remove-reaction", "request-auth", "submit-reply"]);

const isReplying = ref(false);
const replyText = ref("");

const canSubmitReply = computed(() => replyText.value.trim().length > 0);

function toggleReply() {
  isReplying.value = !isReplying.value;
  if (!isReplying.value) {
    replyText.value = "";
  }
}

function submitReply() {
  const content = replyText.value.trim();
  if (!content || props.submittingReplyToId === props.comment.id) return;

  emit("submit-reply", {
    parentCommentId: props.comment.id,
    content,
  });
}

function formatDate(isoDate) {
  if (!isoDate) return "";
  return new Date(isoDate).toLocaleString("ru-RU", {
    year: "numeric",
    month: "long",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
}
</script>

<style scoped>
.comment-item {
  width: 100%;
  box-sizing: border-box;
  border: 1px solid var(--border-color);
  border-radius: 14px;
  padding: 0.9rem 1rem;
  background: var(--bg-primary);
  transition: background 0.2s, border-color 0.2s, transform 0.2s, box-shadow 0.2s;
}

.comment-item:hover {
  background: var(--bg-secondary);
  border-color: var(--btn-primary-bg);
  transform: translateY(-1px);
  box-shadow: 0 8px 18px rgba(0, 0, 0, 0.12);
}

.comment-item-deleted {
  opacity: 0.72;
  border-style: dashed;
}

.comment-item-deleted:hover {
  background: var(--bg-primary);
  border-color: var(--border-color);
  transform: none;
  box-shadow: none;
}

.comment-top {
  display: flex;
  justify-content: space-between;
  gap: 1rem;
  flex-wrap: wrap;
  margin-bottom: 0.5rem;
}

.comment-top-right {
  display: flex;
  align-items: center;
  gap: 0.65rem;
  flex-wrap: wrap;
}

.comment-author {
  color: var(--text-primary);
  font-weight: 700;
}

.comment-date {
  color: var(--text-secondary);
  font-size: 0.88rem;
}

.comment-content {
  color: var(--text-primary);
  line-height: 1.6;
  white-space: pre-line;
  word-break: break-word;
  overflow-wrap: anywhere;
}

.comment-content-deleted {
  text-decoration: line-through;
  color: var(--text-secondary);
}

.comment-actions {
  margin-top: 0.7rem;
  display: flex;
  align-items: center;
  gap: 0.75rem;
  flex-wrap: wrap;
}

.reply-btn {
  border: none;
  background: transparent;
  color: var(--btn-primary-bg);
  cursor: pointer;
  font: inherit;
  font-weight: 600;
  padding: 0.15rem 0;
}

.reply-btn:hover:not(:disabled) {
  text-decoration: underline;
}

.reply-btn:disabled {
  opacity: 0.65;
  cursor: not-allowed;
}

.reply-editor {
  margin-top: 0.75rem;
  display: flex;
  flex-direction: column;
  gap: 0.65rem;
}

.reply-textarea {
  width: 100%;
  box-sizing: border-box;
  min-height: 84px;
  resize: vertical;
  border-radius: 12px;
  border: 1px solid var(--border-color);
  background: var(--bg-secondary);
  color: var(--text-primary);
  padding: 0.75rem 0.85rem;
  font: inherit;
  outline: none;
}

.reply-textarea:focus {
  border-color: var(--btn-primary-bg);
  box-shadow: 0 0 0 3px rgba(0, 123, 255, 0.12);
}

.reply-actions {
  display: flex;
  gap: 0.65rem;
}

.reply-btn-primary {
  border-radius: 10px;
  padding: 0.55rem 0.9rem;
  background: var(--btn-primary-bg);
  color: #fff;
  text-decoration: none;
}

.comment-children {
  margin-top: 0.8rem;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}
</style>