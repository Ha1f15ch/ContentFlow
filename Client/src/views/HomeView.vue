<template>
  <div class="home">
    <h1>Добро пожаловать!</h1>

    <div v-if="loading">
      <p>Загрузка...</p>
    </div>

    <div v-else>
      <div v-if="authStore.isAuthenticated" class="welcome-block">
        <h2>Привет, {{ userProfile?.firstName || 'Пользователь' }}!</h2>
      </div>

      <div v-else class="guest-block">
        <p>Вы вошли как гость. Посты и комментарии доступны для чтения.</p>
      </div>

      <PostList :posts="posts" @open="openPost" />

      <CategoryListSmart />

      <TagList :tags="tags" />

      <PostDetailsModal
        v-model="isPostModalOpen"
        :post-id="selectedPostId"
        :is-authenticated="authStore.isAuthenticated"
      />
    </div>
  </div>
</template>

// Логика для HomeView.vue
<script setup>
import { ref, onMounted, computed } from "vue";
import { useRouter, useRoute } from "vue-router";
import { useAuthStore } from "@/features/auth/stores/authStore";

import PostList from "@/shared/components/PostList.vue";
import PostDetailsModal from "@/features/post/components/PostDetailsModal.vue";
import TagList from "@/shared/components/TagList.vue";
import CategoryListSmart from "@/features/category/components/CategoryListSmart.vue";

import { userProfileService } from "@/features/userProfile/api/userProfileService";
import { tagService } from "@/features/tag/api/tagService";
import { postService } from "@/features/post/api/postService";

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();

const userProfile = ref(null);
const tags = ref([]);
const posts = ref([]);
const loading = ref(true);

const selectedPostId = computed(() => {
  const raw = route.query.postId;
  if (!raw) return null;

  const id = Number(raw);
  return Number.isNaN(id) ? null : id;
});

const isPostModalOpen = computed({
  get: () => !!selectedPostId.value,
  set: async (value) => {
    if (!value) {
      const nextQuery = { ...route.query };
      delete nextQuery.postId;

      await router.push({
        path: route.path,
        query: nextQuery,
      });
    }
  },
});

const openPost = async (id) => {
  await router.push({
    path: route.path,
    query: {
      ...route.query,
      postId: id,
    },
  });
};

onMounted(async () => {
  loading.value = true;

  try {
    const requests = [
      tagService.getTags({ page: 1, pageSize: 50 }),
      postService.getPosts({ page: 1, pageSize: 10 }),
    ];

    if (authStore.isAuthenticated) {
      requests.unshift(userProfileService.getMe());
    }

    const responses = await Promise.all(requests);

    const unwrapItems = (data) =>
      Array.isArray(data) ? data : (data?.items ?? []);

    if (authStore.isAuthenticated) {
      const [userResp, tagsResp, postsResp] = responses;
      userProfile.value = userResp.data;
      tags.value = unwrapItems(tagsResp.data);
      posts.value = unwrapItems(postsResp.data);
    } else {
      const [tagsResp, postsResp] = responses;
      tags.value = unwrapItems(tagsResp.data);
      posts.value = unwrapItems(postsResp.data);
    }
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

.welcome-block,
.guest-block {
  margin-bottom: 1.5rem;
}
</style>