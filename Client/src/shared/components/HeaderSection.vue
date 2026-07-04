<template>
  <header class="app-header">
    <div class="header-left">
      <RouterLink to="/" class="home-link">
        ContentFlow
      </RouterLink>

      <div class="theme-toggle">
        <label class="switch">
          <input type="checkbox" v-model="isDarkTheme" />
          <span class="slider"></span>
        </label>
        <span>Тёмная тема</span>
      </div>
    </div>

    <div class="header-center">
      <input
        :value="postFeedUiStore.filters.search"
        type="text"
        class="header-search"
        placeholder="Искать по постам"
        @input="postFeedUiStore.setSearch($event.target.value)"
        @keydown.enter="runSearch"
      />

      <button
        v-if="hasSearchText"
        type="button"
        class="header-search-btn"
        @click="runSearch"
      >
        Поиск
      </button>

      <div class="filters-popover-wrap" ref="filtersPopupRef">
        <button
          type="button"
          class="header-filters-btn"
          :class="{ active: postFeedUiStore.filtersOpen }"
          @click.stop="postFeedUiStore.toggleFilters()"
        >
          Фильтры
        </button>

        <div
          v-if="postFeedUiStore.filtersOpen"
          class="filters-popover"
          @click.stop
        >
          <div class="filter-group">
            <label>Категория</label>
            <select
              :value="postFeedUiStore.filters.categoryId ?? ''"
              @change="postFeedUiStore.setField('categoryId', parseNullableNumber($event.target.value))"
            >
              <option value="">Все категории</option>
            </select>
          </div>

          <div class="filter-group">
            <label>Создано после</label>
            <input
              :value="postFeedUiStore.filters.createdFrom"
              type="date"
              @input="postFeedUiStore.setField('createdFrom', $event.target.value)"
            />
          </div>

          <div class="filter-group">
            <label>Сортировать по</label>
            <select
              :value="postFeedUiStore.filters.sort.sortBy"
              @change="postFeedUiStore.setSortField('sortBy', Number($event.target.value))"
            >
              <option :value="POST_SORT_BY.CreatedAt">CreatedAt</option>
              <option :value="POST_SORT_BY.PublishedAt">PublishedAt</option>
              <option :value="POST_SORT_BY.Title">Title</option>
              <option :value="POST_SORT_BY.CommentCount">CommentCount</option>
            </select>
          </div>

          <div class="filter-group">
            <label>Направление</label>
            <select
              :value="postFeedUiStore.filters.sort.direction"
              @change="postFeedUiStore.setSortField('direction', Number($event.target.value))"
            >
              <option :value="SORT_DIRECTION.Desc">Desc</option>
              <option :value="SORT_DIRECTION.Asc">Asc</option>
            </select>
          </div>

          <div class="filters-actions">
            <button
              type="button"
              class="primary-btn"
              @click="applyFiltersFromHeader"
            >
              Применить
            </button>

            <button
              type="button"
              class="ghost-btn"
              @click="resetFiltersFromHeader"
            >
              Сбросить
            </button>
          </div>
        </div>
      </div>
    </div>

    <div class="auth-section">
      <div id="guest-actions" v-if="!authStore.isLoggedIn">
        <button class="btn" @click="goToLogin">Войти</button>
      </div>

      <div v-else class="user-area">
        <button class="btn create-post-btn" @click="goToCreatePost">
          Новый пост
        </button>

        <NotificationBell />

        <UserMenu
          :userName="authStore.user?.userName || 'Пользователь'"
          :avatarUrl="authStore.user?.avatarUrl"
        />
      </div>
    </div>
  </header>
</template>

<script setup>
import { computed, ref, onMounted, onBeforeUnmount } from "vue";
import { useRouter, useRoute, RouterLink } from "vue-router";
import { useThemeStore } from "@/shared/stores/theme";
import { useAuthStore } from "@/features/auth/stores/authStore";
import { usePostFeedUiStore } from "@/features/post/stores/postFeedUiStore";
import {
  POST_SORT_BY,
  SORT_DIRECTION,
} from "@/features/post/api/postService";

import UserMenu from "@/shared/components/UserMenu.vue";
import NotificationBell from "@/features/notifications/components/NotificationBell.vue";

const router = useRouter();
const route = useRoute();
const themeStore = useThemeStore();
const authStore = useAuthStore();
const postFeedUiStore = usePostFeedUiStore();

const filtersPopupRef = ref(null);

const isDarkTheme = computed({
  get: () => themeStore.isDark,
  set: (value) => {
    themeStore.isDark = value;
    themeStore.applyTheme();
    localStorage.setItem("theme", value ? "dark" : "light");
  },
});

const hasSearchText = computed(() => {
  return !!postFeedUiStore.filters.search?.trim();
});

function parseNullableNumber(value) {
  return value === "" ? null : Number(value);
}

async function runSearch() {
  if (!hasSearchText.value) return;

  if (route.path !== "/") {
    await router.push("/");
  }

  postFeedUiStore.applyFilters();
}

async function applyFiltersFromHeader() {
  if (route.path !== "/") {
    await router.push("/");
  }

  postFeedUiStore.applyFilters();
  postFeedUiStore.closeFilters();
}

async function resetFiltersFromHeader() {
  postFeedUiStore.resetFilters();

  if (route.path !== "/") {
    await router.push("/");
  }

  postFeedUiStore.closeFilters();
}

function handleOutsideClick(event) {
  if (!filtersPopupRef.value) return;
  if (!filtersPopupRef.value.contains(event.target)) {
    postFeedUiStore.closeFilters();
  }
}

const goToLogin = () => {
  router.push("/login");
};

const goToCreatePost = () => {
  if (!authStore.isLoggedIn) {
    router.push("/login");
    return;
  }

  router.push("/create-post");
};

onMounted(() => {
  document.addEventListener("click", handleOutsideClick);
});

onBeforeUnmount(() => {
  document.removeEventListener("click", handleOutsideClick);
});
</script>

<style scoped>
.app-header {
  display: grid;
  grid-template-columns: auto minmax(320px, 1fr) auto;
  align-items: center;
  gap: 1rem;
}

.header-left {
  display: flex;
  align-items: center;
  gap: 1.25rem;
}

.header-center {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  min-width: 0;
}

.home-link {
  text-decoration: none;
  color: var(--text-primary);
  font-weight: 600;
  padding: 0.45rem 0.8rem;
  border: 1px solid var(--border-color);
  border-radius: 8px;
  transition: background-color 0.2s, color 0.2s;
}

.home-link:hover {
  background: var(--bg-secondary);
}

.header-search {
  flex: 1;
  min-width: 0;
  border: 1px solid var(--border-color);
  background: var(--bg-primary);
  color: var(--text-primary);
  border-radius: 10px;
  padding: 0.75rem 0.9rem;
  font: inherit;
  box-sizing: border-box;
}

.header-search-btn,
.header-filters-btn {
  border: 1px solid var(--border-color);
  background: var(--card-bg);
  color: var(--text-primary);
  border-radius: 10px;
  padding: 0.75rem 1rem;
  font: inherit;
  cursor: pointer;
  white-space: nowrap;
}

.header-filters-btn.active {
  border-color: var(--btn-primary-bg);
}

.filters-popover-wrap {
  position: relative;
  flex-shrink: 0;
}

.filters-popover {
  position: absolute;
  top: calc(100% + 10px);
  right: 0;
  width: 320px;
  background: var(--card-bg);
  border: 1px solid var(--border-color);
  border-radius: 14px;
  padding: 1rem;
  box-shadow: 0 14px 34px rgba(0, 0, 0, 0.28);
  z-index: 100;
  box-sizing: border-box;
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
  margin-bottom: 0.9rem;
}

.filter-group label {
  color: var(--text-primary);
  font-size: 0.92rem;
  font-weight: 600;
}

.filter-group input,
.filter-group select {
  width: 100%;
  box-sizing: border-box;
  border: 1px solid var(--border-color);
  background: var(--bg-primary);
  color: var(--text-primary);
  border-radius: 10px;
  padding: 0.7rem 0.8rem;
  font: inherit;
}

.filters-actions {
  display: flex;
  gap: 0.75rem;
  margin-top: 0.25rem;
}

.primary-btn,
.ghost-btn {
  border-radius: 10px;
  padding: 0.7rem 1rem;
  font: inherit;
  cursor: pointer;
}

.primary-btn {
  border: none;
  background: var(--btn-primary-bg);
  color: white;
}

.ghost-btn {
  border: 1px solid var(--border-color);
  background: transparent;
  color: var(--text-primary);
}

.create-post-btn {
  margin-right: 10px;
  background-color: #007bff;
  color: white;
  border: none;
  padding: 8px 12px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
}

.create-post-btn:hover {
  background-color: #0056b3;
}

.auth-section {
  display: flex;
  align-items: center;
  justify-content: flex-end;
}

.user-area {
  display: flex;
  align-items: center;
  gap: 10px;
}

.theme-toggle span {
  color: var(--text-primary);
}

@media (max-width: 1100px) {
  .app-header {
    grid-template-columns: 1fr;
  }

  .header-center {
    order: 3;
  }

  .auth-section {
    justify-content: flex-start;
  }

  .filters-popover {
    right: auto;
    left: 0;
    width: min(320px, calc(100vw - 2rem));
  }
}
</style>