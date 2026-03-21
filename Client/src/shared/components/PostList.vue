<template>
  <div class="post-list">
    <article
      v-for="post in posts"
      :key="post.id"
      class="post-card"
      @click="$emit('open', post.id)"
    >
      <h3 class="post-title">
        {{ post.title ?? post.name ?? '(без названия)' }}
      </h3>

      <div class="post-meta">
        <span class="post-author">
          {{ post.authorName ?? post.author?.userName ?? '-' }}
        </span>
        <span class="post-dot">•</span>
        <span class="post-date">
          {{ formatDate(post.createdAt ?? post.createdAtUtc) }}
        </span>
      </div>

      <p class="post-excerpt">
        {{ post.excerpt ?? post.summary ?? '' }}
      </p>
    </article>

    <div v-if="posts.length === 0" class="empty-state">
      Постов пока нет.
    </div>
  </div>
</template>

// Логика для компонента PostList.vue
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

// Стили для компонента PostList.vue
<style scoped>

.post-list {
  max-width: 720px;
  margin: 2rem auto;
  padding: 0 1rem;
}

.post-card {
  background-color: var(--bg-secondary, #1e1e1e);
  color: var(--text-primary, #e0e0e0);
  padding: 1.1rem 1.1rem 1rem;
  margin-bottom: 1rem;
  border-radius: 14px;
  box-shadow: 0 4px 14px rgba(0, 0, 0, 0.12);
  transition: transform 0.18s ease, box-shadow 0.18s ease;
  cursor: pointer;
  max-width: 640px;
  margin-left: auto;
  margin-right: auto;
}

.post-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 8px 18px rgba(0, 0, 0, 0.16);
}

.post-title {
  font-size: 1.25rem;
  line-height: 1.3;
  margin-bottom: 0.55rem;
  color: var(--text-primary, #e0e0e0);
}

.post-meta {
  display: flex;
  align-items: center;
  gap: 0.4rem;
  font-size: 0.88rem;
  color: var(--text-secondary, #a0a0a0);
  margin-bottom: 0.8rem;
  flex-wrap: wrap;
}

.post-dot {
  opacity: 0.65;
}

.post-excerpt {
  color: var(--text-secondary, #a0a0a0);
  line-height: 1.5;
  margin: 0;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
  text-overflow: ellipsis;
}

.empty-state {
  text-align: center;
  padding: 2rem;
  color: var(--text-secondary, #a0a0a0);
  font-style: italic;
}

</style>