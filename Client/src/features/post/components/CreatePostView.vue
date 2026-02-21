<template>
  <div class="create-post-container">
    <h1>Создать пост</h1>
    <CreatePostForm />
  </div>
</template>

<script setup>
import CreatePostForm from './CreatePost.vue';
</script>

<style scoped>
.create-post-container {
  max-width: 800px;
  margin: 2rem auto;
  padding: 1rem;
}
</style>