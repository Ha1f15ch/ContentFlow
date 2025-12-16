<template>
  <div class="category-list">
    <h3>Категории</h3>
    <ul>
      <li v-for="category in categories" :key="category.id">
        {{ category.name }}
      </li>
    </ul>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { categoryService } from '@/features/category/api/categoryService';

const categories = ref([]);

onMounted(async () => {
  try {
    const response = await categoryService.getCategories();
    categories.value = response.data;
  } catch (err) {
    console.error('Ошибка загрузки категорий:', err);
  }
});
</script>

<style scoped>
.category-list {
  padding: 1rem;
}
</style>