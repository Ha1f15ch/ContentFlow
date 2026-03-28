<template>
  <div class="post-list-wrap">
    <div class="post-list">
      <article
        v-for="post in posts"
        :key="post.id"
        class="post-row"
        @click="$emit('open', post.id)"
      >
        <h3 class="post-title">
          {{ post.title ?? "(без названия)" }}
        </h3>

        <div class="post-meta">
          <span>{{ post.authorName ?? "-" }}</span>
          <span class="post-dot">•</span>
          <span>{{ formatDate(post.createdAt ?? post.createdAtUtc) }}</span>
        </div>

        <p class="post-excerpt">
          {{ post.excerpt ?? "" }}
        </p>
      </article>

      <div v-if="posts.length === 0" class="empty-state">
        Постов пока нет.
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  posts: { type: Array, default: () => [] },
});

defineEmits(["open"]);

const formatDate = (isoDate) => {
  if (!isoDate) return "";
  const date = new Date(isoDate);
  return date.toLocaleString("ru-RU", {
    year: "numeric",
    month: "short",
    day: "numeric",
  });
};
</script>

<style scoped>
.post-list-wrap {
  width: 100%;
}

.post-list {
  width: 100%;
  max-width: 720px;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.post-row {
  width: 100%;
  background: var(--card-bg);
  border: 1px solid var(--border-color);
  border-radius: 16px;
  padding: 1rem 1.1rem;
  cursor: pointer;
  transition: border-color 0.2s, transform 0.2s, box-shadow 0.2s;
}

.post-row:hover {
  border-color: var(--btn-primary-bg);
  transform: translateY(-1px);
  box-shadow: 0 8px 18px rgba(0, 0, 0, 0.12);
}

.post-title {
  margin: 0 0 0.45rem;
  color: var(--text-primary);
  font-size: 1.18rem;
  line-height: 1.3;
}

.post-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 0.4rem;
  color: var(--text-secondary);
  font-size: 0.88rem;
  margin-bottom: 0.75rem;
}

.post-dot {
  opacity: 0.65;
}

.post-excerpt {
  margin: 0;
  color: var(--text-secondary);
  line-height: 1.55;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.empty-state {
  text-align: center;
  padding: 2rem;
  color: var(--text-secondary);
}
</style>