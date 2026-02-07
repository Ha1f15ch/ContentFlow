<template>
  <div class="tag-list">
    <h3>Теги</h3>
    <ul>
      <li v-for="tag in tags" :key="tag.id">
        {{ tag.name }}
      </li>
    </ul>
  </div>
</template>

<script setup>
defineProps({
  tags: { type: Array, default: () => [] },
});
</script>

<style scoped>
.tag-list { padding: 1rem; }
</style>
