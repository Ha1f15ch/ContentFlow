<template>
  <div class="home">
    <div v-if="loading" class="home-loading">
      <p>Загрузка...</p>
    </div>

    <div v-else class="home-layout">
      <div class="home-content">
        <PostSectionsTabs
          v-model="viewMode"
          :is-authenticated="authStore.isAuthenticated"
          @update:modelValue="handleViewModeChange"
        />
      
        <div class="posts-area">
          <PostList :posts="posts" @open="openPost" />
        </div>
      </div>
    </div>

    <PostDetailsModal
      v-model="isPostModalOpen"
      :post-id="selectedPostId"
      :is-authenticated="authStore.isAuthenticated"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, computed } from "vue";
import { useRouter, useRoute } from "vue-router";
import { useAuthStore } from "@/features/auth/stores/authStore";
import { watch } from "vue";
import { usePostFeedUiStore } from "@/features/post/stores/postFeedUiStore";

import PostList from "@/shared/components/PostList.vue";
import PostDetailsModal from "@/features/post/components/PostDetailsModal.vue";
import PostSectionsTabs from "@/features/post/components/PostSectionsTabs.vue";
import TagList from "@/shared/components/TagList.vue";
import CategoryListSmart from "@/features/category/components/CategoryListSmart.vue";

import { userProfileService } from "@/features/userProfile/api/userProfileService";
import { tagService } from "@/features/tag/api/tagService";
import {
  postService,
  POST_SORT_BY,
  SORT_DIRECTION,
  POST_STATUS,
} from "@/features/post/api/postService";

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();

const postFeedUiStore = usePostFeedUiStore();
const userProfile = ref(null);
const tags = ref([]);
const posts = ref([]);
const categories = ref([]);
const loading = ref(true);

const viewMode = ref("feed");

const filters = computed(() => postFeedUiStore.filters);

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

const currentAuthorId = computed(() => {
  return authStore.user?.userId ?? null;
});

const filtersOpen = ref(false);

function buildPostFilter() {
  const filter = {
    search: postFeedUiStore.filters.search || null,
    categoryId: postFeedUiStore.filters.categoryId,
    createdFrom: postFeedUiStore.filters.createdFrom || null,
    sort: {
      sortBy: postFeedUiStore.filters.sort.sortBy,
      direction: postFeedUiStore.filters.sort.direction,
    },
  };

  if (viewMode.value === "feed") {
    filter.status = POST_STATUS.Published;
  }

  if (viewMode.value === "myPublished") {
    filter.status = POST_STATUS.Published;
    filter.authorId = currentAuthorId.value;
  }

  if (viewMode.value === "myDrafts") {
    filter.status = POST_STATUS.Draft;
    filter.authorId = currentAuthorId.value;
  }

  if (viewMode.value === "myArchived") {
    filter.status = POST_STATUS.Archived;
    filter.authorId = currentAuthorId.value;
  }

  return filter;
}

async function loadPosts() {
  try {
    const response = await postService.getPosts({
      page: 1,
      pageSize: 10,
      filter: buildPostFilter(),
    });

    console.log("GET /posts response:", response.data);
    posts.value = response.data?.items ?? [];
  } catch (err) {
    console.error("Ошибка загрузки постов:", err);
  }
}

function handleViewModeChange(mode) {
  viewMode.value = mode;
  loadPosts();
}

function resetFilters() {
  filters.value = {
    search: "",
    categoryId: null,
    createdFrom: "",
    sort: {
      sortBy: POST_SORT_BY.CreatedAt,
      direction: SORT_DIRECTION.Desc,
    },
  };

  loadPosts();
}

watch(
  () => postFeedUiStore.applyVersion,
  () => {
    loadPosts();
  }
);

onMounted(async () => {
  loading.value = true;

  try {
    const requests = [
      tagService.getTags({ page: 1, pageSize: 50 }),
    ];

    if (authStore.isAuthenticated) {
      requests.unshift(userProfileService.getMe());
    }

    const responses = await Promise.all(requests);

    const unwrapItems = (data) =>
      Array.isArray(data) ? data : (data?.items ?? []);

    if (authStore.isAuthenticated) {
      const [userResp, tagsResp] = responses;
      userProfile.value = userResp.data;
      tags.value = unwrapItems(tagsResp.data);
    } else {
      const [tagsResp] = responses;
      tags.value = unwrapItems(tagsResp.data);
    }

    categories.value = [];
    await loadPosts();
  } catch (err) {
    console.error("Ошибка загрузки данных:", err);
  } finally {
    loading.value = false;
  }
});
</script>

<style scoped>
.home {
  max-width: 1280px;
  margin: 0 auto;
  padding: 2rem 1.5rem;
  box-sizing: border-box;
}

.home-loading {
  text-align: center;
  padding: 2rem;
}

.home-layout {
  display: flex;
  justify-content: center;
  width: 100%;
}

.home-content {
  width: 760px;
  max-width: 100%;
  display: flex;
  flex-direction: column;
  gap: 1rem;
  box-sizing: border-box;
}

.home-content > * {
  width: 100%;
  box-sizing: border-box;
}

.posts-area {
  width: 100%;
  box-sizing: border-box;
}

@media (max-width: 820px) {
  .home {
    padding: 1rem;
  }

  .home-content {
    width: 100%;
  }
}
</style>