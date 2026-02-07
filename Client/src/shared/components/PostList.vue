<template>
  <div class="post-list">
    <div v-for="post in posts" :key="post.id" class="post-item">
      <h3>{{ post.title ?? post.name ?? '(без названия)' }}</h3>
      <p>Автор: {{ post.authorName ?? post.author?.userName ?? '-' }}</p>
      <p>{{ post.createdAt ?? post.createdAtUtc ?? '' }}</p>
      <p>{{ post.excerpt ?? post.summary ?? '' }}</p>
      <button @click="$emit('open', post.id)">Читать далее</button>
    </div>
  </div>
</template>

<script setup>
defineProps({
  posts: { type: Array, default: () => [] },
});
defineEmits(["open"]);
</script>

<style scoped>
.post-list { padding: 2rem; }
.post-item {
  background-color: white;
  padding: 1rem;
  margin-bottom: 1rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}
</style>
