<template>
  <div class="create-post">
    <h1>Создать пост</h1>
    <form @submit.prevent="submitPost">
      <div class="form-group">
        <label for="title">Заголовок:</label>
        <input id="title" v-model="title" type="text" required />
      </div>

      <div class="form-group">
        <label for="content">Содержимое:</label>
        <textarea id="content" v-model="content" rows="6" required></textarea>
      </div>

      <div class="form-group">
        <label for="categoryId">Категория:</label>
        <select id="categoryId" v-model="categoryId" required>
          <option value="">Выберите категорию</option>
          <option v-for="category in categories" :key="category.id" :value="category.id">
            {{ category.name }}
          </option>
        </select>
      </div>

      <button type="submit" :disabled="loading">Создать пост</button>
    </form>

    <div v-if="error" class="error">{{ error }}</div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { categoryService } from '@/features/category/api/categoryService';
import { postService } from '@/features/post/api/postService';

const router = useRouter();

const title = ref('');
const content = ref('');
const categoryId = ref('');
const categories = ref([]);
const loading = ref(false);
const error = ref('');

onMounted(async () => {
  try {
    const response = await categoryService.getCategories();
    categories.value = response.data;
  } catch (err) {
    error.value = 'Ошибка загрузки категорий';
    console.error(err);
  }
});

const submitPost = async () => {
  loading.value = true;
  error.value = '';

  try {
    await postService.createPost({
      title: title.value,
      content: content.value,
      categoryId: parseInt(categoryId.value, 10),
    });

    router.push('/'); // перенаправить на главную
  } catch (err) {
    error.value = 'Ошибка создания поста';
    console.error(err);
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.create-post {
  max-width: 600px;
  margin: 2rem auto;
  padding: 1rem;
}

.form-group {
  margin-bottom: 1rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
}

input, textarea, select {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ccc;
  border-radius: 4px;
}

button {
  padding: 0.75rem 1.5rem;
  background-color: #007bff;
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}

button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error {
  color: red;
  margin-top: 1rem;
}
</style>