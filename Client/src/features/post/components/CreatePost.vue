<template>
  <form @submit.prevent="submitPost" class="create-post-form">
    <div class="form-group">
      <label for="title">Заголовок:</label>
      <input
        id="title"
        v-model="title"
        type="text"
        required
        placeholder="Введите заголовок"
      />
    </div>

    <div class="form-group">
      <label for="content">Описание:</label>
      <textarea
        id="content"
        v-model="content"
        rows="6"
        required
        placeholder="Введите описание..."
      ></textarea>
    </div>

    <button type="submit" :disabled="loading">
      {{ loading ? 'Создаю...' : 'Создать пост' }}
    </button>

    <div v-if="error" class="error">{{ error }}</div>
  </form>
</template>

<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { postService } from '../api/postService';

const router = useRouter();

const title = ref('');
const content = ref('');
const loading = ref(false);
const error = ref('');

const submitPost = async () => {
  if (!title.value.trim() || !content.value.trim()) {
    error.value = 'Заголовок и описание обязательны';
    return;
  }

  loading.value = true;
  error.value = '';

  try {
    await postService.createPost({
      title: title.value.trim(),
      content: content.value.trim()
    });
    router.push('/'); // Перенаправить на главную после создания
  } catch (err) {
    console.error(err);
    error.value = err.response?.data?.message || 'Ошибка при создании поста';
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.create-post-form {
  max-width: 600px;
  margin: 2rem auto;
  padding: 1rem;
  background: #222;
  border-radius: 8px;
}

.form-group {
  margin-bottom: 1rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: bold;
}

input, textarea {
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #444;
  border-radius: 4px;
  background: #333;
  color: white;
}

button {
  background-color: #007bff;
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 4px;
  cursor: pointer;
  font-size: 1rem;
}

button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error {
  color: #ff6b6b;
  margin-top: 0.5rem;
}
</style>