<!-- src/components/PostList.vue -->
<template>
  <div class="post-list">
    <div v-for="post in posts" :key="post.id" class="post-item">
      <h3>{{ post.title }}</h3>
      <p>Автор: {{ post.authorName }}</p>
      <p>{{ post.createdAt }}</p>
      <p>{{ post.excerpt }}</p>
      <button @click="viewPost(post.id)">Читать далее</button>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import { authService } from '@/features/auth/api/authService';

const posts = ref([]);

// Загрузка постов
const loadPosts = async () => {
  try {
    const response = await authService.getPosts();
    posts.value = response.data.items;
  } catch (err) {
    console.error('Ошибка загрузки постов:', err);
  }
};

loadPosts();

const viewPost = (postId) => {
  // Перенаправление на страницу поста
  router.push(`/post/${postId}`);
};
</script>

<style scoped>
.post-list {
  padding: 2rem;
}

.post-item {
  background-color: white;
  padding: 1rem;
  margin-bottom: 1rem;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}
</style>