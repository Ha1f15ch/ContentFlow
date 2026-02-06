<template>
  <div class="home">
    <h1>Добро пожаловать!</h1>

    <div v-if="authStore.isAuthenticated">
      <div v-if="loading">
        <p>Загрузка...</p>
      </div>

      <div v-else>
        <h2>Привет, {{ userProfile?.firstName || 'Пользователь' }}!</h2>

        <PostList :posts="posts" @open="id => router.push(`/post/${id}`)" />

        <CategoryListSmart />

        <TagList :tags="tags" />
      </div>
    </div>

    <div v-else>
      <p>Пожалуйста, авторизуйтесь.</p>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import { useRouter } from "vue-router";
import { useAuthStore } from "@/features/auth/stores/authStore";

import PostList from "@/shared/components/PostList.vue";
import TagList from "@/shared/components/TagList.vue";

// ВАЖНО: импортируем Smart, а не старый CategoryList
import CategoryListSmart from "@/features/category/components/CategoryListSmart.vue";
import { userProfileService } from "@/features/userProfile/api/userProfileService";
import { tagService } from "@/features/tag/api/tagService";
import { postService } from "@/features/post/api/postService";

const router = useRouter();
const authStore = useAuthStore();

const userProfile = ref(null);
const tags = ref([]);
const posts = ref([]);
const loading = ref(true);

onMounted(async () => {
  loading.value = true;

  if (!authStore.isAuthenticated) {
    loading.value = false;
    return;
  }

  try {
    const unwrapItems = (data) => (Array.isArray(data) ? data : (data?.items ?? []));

    const [userResp, tagsResp, postsResp] = await Promise.all([
      userProfileService.getMe(),
      tagService.getTags({ page: 1, pageSize: 50 }),
      postService.getPosts({ page: 1, pageSize: 10 }),
    ]);

    userProfile.value = userResp.data;
    tags.value = unwrapItems(tagsResp.data);
    posts.value = unwrapItems(postsResp.data);
  } catch (err) {
    console.error("Ошибка загрузки данных:", err);
  } finally {
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
