<template>
  <div class="create-post-container">
    <CreatePostForm />
  </div>
</template>

<script setup>
import CreatePostForm from '@/features/post/components/CreatePost.vue';
</script>

<style scoped>
.create-post-container {
  width: 100%;
  max-width: 1600px;
  margin: 0 auto;
  padding: 1rem;
}
</style>