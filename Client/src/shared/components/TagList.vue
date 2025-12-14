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
import { ref, onMounted } from 'vue';
import { tagService } from '@/features/tag/api/tagService';

const tags = ref([]);

onMounted(async () => {
  try {
    const response = await tagService.getTags();
    tags.value = response.data;
  } catch (err) {
    console.error('Ошибка загрузки тегов:', err);
  }
});
</script>

<style scoped>
.tag-list {
  padding: 1rem;
}
</style>