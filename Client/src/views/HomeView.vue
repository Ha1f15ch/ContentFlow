<template>
  <div class="home">
    <h1>Добро пожаловать!</h1>
    <div v-if="authStore.isAuthenticated">
      <div v-if="loading">
        <p>Загрузка...</p>
      </div>
      <div v-else>
        <h2>Привет, {{ userProfile?.firstName || 'Пользователь' }}!</h2>
        <PostList :posts="posts" />
        <CategoryList :categories="categories" />
        <TagList :tags="tags" />
      </div>
    </div>
    <div v-else>
      <p>Пожалуйста, авторизуйтесь.</p>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import { useAuthStore } from '@/features/auth/stores/authStore';
import PostList from '@/shared/components/PostList.vue';
import CategoryList from '@/shared/components/CategoryList.vue';
import TagList from '@/shared/components/TagList.vue';
import { userProfileService } from '@/features/userProfile/api/userProfileService';
import { categoryService } from '@/features/category/api/categoryService';
import { tagService } from '@/features/tag/api/tagService';
import { postService } from '@/features/post/api/postService';

const authStore = useAuthStore();

const userProfile = ref(null);
const categories = ref([]);
const tags = ref([]);
const posts = ref([]);
const loading = ref(true);

onMounted(async () => {
  if (authStore.isAuthenticated) {
    try {
      // Загружаем данные параллельно
      const [userResponse, categoriesResponse, tagsResponse, postsResponse] = await Promise.all([
        userProfileService.getProfile(),
        categoryService.getCategories(),
        tagService.getTags(),
        postService.getPosts(),
      ]);

      userProfile.value = userResponse.data;
      categories.value = categoriesResponse.data;
      tags.value = tagsResponse.data;
      posts.value = postsResponse.data;
    } catch (err) {
      console.error('Ошибка загрузки данных:', err);
    } finally {
      loading.value = false;
    }
  } else {
    loading.value = false;
  }
});
</script>

<style scoped>
.home {
  text-align: center;
  padding: 2rem;
}
</style>