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

      <div class="comment-date">
        {{ formatDate(comment.createdAt) }}
      </div>
    </div>

    <div
      class="comment-content"
      :class="{ 'comment-content-deleted': comment.isDeleted }"
    >
      {{ comment.content }}
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
      />
    </div>
  </article>
</template>

<script setup>
import CommentThreadChild from "./CommentThreadItem.vue";

defineProps({
  comment: { type: Object, required: true },
  depth: { type: Number, default: 0 },
});

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

.comment-children {
  margin-top: 0.8rem;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}
</style>