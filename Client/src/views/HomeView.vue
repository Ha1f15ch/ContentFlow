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
          <PostList
            :posts="posts"
            :is-authenticated="authStore.isAuthenticated"
            @open="openPost"
            @reaction-updated="applyPostReaction"
          />
          <div ref="postsSentinel" class="posts-sentinel" aria-hidden="true"></div>
          <p v-if="isLoadingPosts && posts.length > 0" class="posts-status">
            Загрузка ещё...
          </p>
          <p v-else-if="!hasMorePosts && posts.length > 0" class="posts-status">
            Все посты загружены.
          </p>
        </div>
      </div>
    </div>

    <PostDetailsModal
      v-model="isPostModalOpen"
      :post-id="selectedPostId"
      :is-authenticated="authStore.isAuthenticated"
      @reaction-updated="applyPostReaction"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, computed, watch } from "vue";
import { useRouter, useRoute } from "vue-router";
import { useAuthStore } from "@/features/auth/stores/authStore";
import { usePostFeedUiStore } from "@/features/post/stores/postFeedUiStore";
import { useInfiniteScroll } from "@/shared/composables/useInfiniteScroll";

import PostList from "@/shared/components/PostList.vue";
import PostDetailsModal from "@/features/post/components/PostDetailsModal.vue";
import PostSectionsTabs from "@/features/post/components/PostSectionsTabs.vue";

import { tagService } from "@/features/tag/api/tagService";
import {
  postService,
  POST_STATUS,
} from "@/features/post/api/postService";

const POSTS_PAGE_SIZE = 5;

const router = useRouter();
const route = useRoute();
const authStore = useAuthStore();

const postFeedUiStore = usePostFeedUiStore();
const userProfile = ref(null);
const tags = ref([]);
const posts = ref([]);
const loading = ref(true);
const isLoadingPosts = ref(false);
const postsPage = ref(0);
const postsTotalPages = ref(0);
const postsSentinel = ref(null);

const viewMode = ref("feed");

const hasMorePosts = computed(() => postsPage.value < postsTotalPages.value);

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

async function loadPosts({ reset = false } = {}) {
  if (isLoadingPosts.value) return;
  if (!reset && !hasMorePosts.value) return;

  const nextPage = reset ? 1 : postsPage.value + 1;

  isLoadingPosts.value = true;

  try {
    const response = await postService.getPosts({
      page: nextPage,
      pageSize: POSTS_PAGE_SIZE,
      filter: buildPostFilter(),
    });

    console.log("GET /posts response:", response.data);

    const result = response.data ?? {};
    const items = result.items ?? [];

    posts.value = reset ? items : [...posts.value, ...items];
    postsPage.value = result.page ?? nextPage;
    postsTotalPages.value = result.totalPages ?? postsTotalPages.value;
  } catch (err) {
    console.error("Ошибка загрузки постов:", err);
  } finally {
    isLoadingPosts.value = false;
  }
}

async function resetAndLoadPosts() {
  posts.value = [];
  postsPage.value = 0;
  postsTotalPages.value = 0;
  await loadPosts({ reset: true });
}

function applyPostReaction(result) {
  const target = posts.value.find((post) => post.id === result.entityId);
  if (!target) return;

  target.likesCount = result.likesCount;
  target.dislikesCount = result.dislikesCount;
  target.currentUserReaction = result.currentUserReaction;
}

function handleViewModeChange(mode) {
  viewMode.value = mode;
  resetAndLoadPosts();
}

watch(
  () => postFeedUiStore.applyVersion,
  () => {
    resetAndLoadPosts();
  }
);

useInfiniteScroll({
  sentinelRef: postsSentinel,
  canLoadMore: () => !loading.value && !isLoadingPosts.value && hasMorePosts.value,
  loadMore: () => loadPosts(),
});

onMounted(async () => {
  loading.value = true;

  try {
    const tagsResp = await tagService.getTags({ page: 1, pageSize: 50 });
    const unwrapItems = (data) =>
      Array.isArray(data) ? data : (data?.items ?? []);

    tags.value = unwrapItems(tagsResp.data);

    if (authStore.isAuthenticated) {
      userProfile.value = authStore.user;
    }

    await resetAndLoadPosts();
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

.posts-sentinel {
  width: 100%;
  height: 1px;
}

.posts-status {
  margin: 0;
  padding: 1rem;
  text-align: center;
  color: var(--text-secondary);
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