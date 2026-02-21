<template>
  <div class="post-list">
    <div v-for="post in posts" :key="post.id" class="post-item">
      <h3 class="post-title">{{ post.title ?? post.name ?? '(без названия)' }}</h3>
      <p class="post-author">Автор: {{ post.authorName ?? post.author?.userName ?? '-' }}</p>
      <p class="post-date">{{ formatDate(post.createdAt ?? post.createdAtUtc) }}</p>
      <p class="post-excerpt">{{ post.excerpt ?? post.summary ?? '' }}</p>
      <button class="read-more-btn" @click="$emit('open', post.id)">Читать далее</button>
    </div>

    <div v-if="posts.length === 0" class="empty-state">
      Постов пока нет.
    </div>
  </div>
</template>

<script setup>
defineProps({
  posts: { type: Array, default: () => [] },
});
defineEmits(["open"]);

const formatDate = (isoDate) => {
  if (!isoDate) return '';
  const date = new Date(isoDate);
  return date.toLocaleString('ru-RU', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  });
};
</script>

<style scoped>
.post-list {
  max-width: 800px;
  margin: 2rem auto;
  padding: 0 1rem;
}

.post-item {
  background-color: var(--bg-secondary, #1e1e1e);
  color: var(--text-primary, #e0e0e0);
  padding: 1.5rem;
  margin-bottom: 1.5rem;
  border-radius: 8px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.15);
  transition: transform 0.2s, box-shadow 0.2s;
}

.post-item:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.post-title {
  font-size: 1.5rem;
  margin-bottom: 0.75rem;
  color: var(--text-primary, #e0e0e0);
}

.post-author,
.post-date {
  font-size: 0.9rem;
  color: var(--text-secondary, #a0a0a0);
  margin-bottom: 0.5rem;
}

.post-excerpt {
  color: var(--text-secondary, #a0a0a0);
  line-height: 1.6;
  margin-bottom: 1rem;
}

.read-more-btn {
  background: transparent;
  color: var(--btn-primary-bg, #1a73e8);
  border: 1px solid var(--btn-primary-bg, #1a73e8);
  padding: 6px 12px;
  border-radius: 4px;
  font-size: 0.9rem;
  cursor: pointer;
  transition: all 0.2s;
}

.read-more-btn:hover {
  background-color: rgba(26, 115, 232, 0.1);
}

.empty-state {
  text-align: center;
  padding: 2rem;
  color: var(--text-secondary, #a0a0a0);
  font-style: italic;
}
</style>