<template>
  <aside class="post-filters">
    <div class="filters-card">
      <h3 class="filters-title">Фильтры</h3>

      <div class="mode-tabs">
        <button
          class="mode-btn"
          :class="{ active: viewMode === 'feed' }"
          @click="$emit('update:viewMode', 'feed')"
        >
          Лента
        </button>

        <button
          v-if="isAuthenticated"
          class="mode-btn"
          :class="{ active: viewMode === 'myPublished' }"
          @click="$emit('update:viewMode', 'myPublished')"
        >
          Мои опубликованные
        </button>

        <button
          v-if="isAuthenticated"
          class="mode-btn"
          :class="{ active: viewMode === 'myDrafts' }"
          @click="$emit('update:viewMode', 'myDrafts')"
        >
          Мои черновики
        </button>

        <button
          v-if="isAuthenticated"
          class="mode-btn"
          :class="{ active: viewMode === 'myArchived' }"
          @click="$emit('update:viewMode', 'myArchived')"
        >
          Мои архивные
        </button>
      </div>

      <div class="filter-group">
        <label>Поиск</label>
        <input
          :value="modelValue.search"
          type="text"
          placeholder="Искать по постам"
          @input="updateField('search', $event.target.value)"
        />
      </div>

      <div class="filter-group">
        <label>Категория</label>
        <select
          :value="modelValue.categoryId ?? ''"
          @change="updateField('categoryId', parseNullableNumber($event.target.value))"
        >
          <option value="">Все категории</option>
          <option
            v-for="category in categories"
            :key="category.id"
            :value="category.id"
          >
            {{ category.name }}
          </option>
        </select>
      </div>

      <div class="filter-group">
        <label>Создано после</label>
        <input
          :value="modelValue.createdFrom"
          type="date"
          @input="updateField('createdFrom', $event.target.value)"
        />
      </div>

      <div class="filter-group">
        <label>Сортировать по</label>
        <select
          :value="modelValue.sort.sortBy"
          @change="updateSort('sortBy', Number($event.target.value))"
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
          :value="modelValue.sort.direction"
          @change="updateSort('direction', Number($event.target.value))"
        >
          <option :value="SORT_DIRECTION.Desc">Desc</option>
          <option :value="SORT_DIRECTION.Asc">Asc</option>
        </select>
      </div>

      <div class="filters-actions">
        <button class="primary-btn" @click="$emit('apply')">Применить</button>
        <button class="ghost-btn" @click="$emit('reset')">Сбросить</button>
      </div>
    </div>
  </aside>
</template>

<script setup>
import {
  POST_SORT_BY,
  SORT_DIRECTION,
} from "@/features/post/api/postService";

const props = defineProps({
  modelValue: {
    type: Object,
    required: true,
  },
  categories: {
    type: Array,
    default: () => [],
  },
  viewMode: {
    type: String,
    default: "feed",
  },
  isAuthenticated: {
    type: Boolean,
    default: false,
  },
});

const emit = defineEmits([
  "update:modelValue",
  "update:viewMode",
  "apply",
  "reset",
]);

function updateField(field, value) {
  emit("update:modelValue", {
    ...props.modelValue,
    [field]: value,
  });
}

function updateSort(field, value) {
  emit("update:modelValue", {
    ...props.modelValue,
    sort: {
      ...props.modelValue.sort,
      [field]: value,
    },
  });
}

function parseNullableNumber(value) {
  return value === "" ? null : Number(value);
}
</script>

<style scoped>
.post-filters {
  width: 100%;
}

.filters-card {
  background: var(--card-bg);
  border: 1px solid var(--border-color);
  border-radius: 16px;
  padding: 1rem;
  position: sticky;
  top: 1rem;
}

.filters-title {
  margin: 0 0 1rem;
  color: var(--text-primary);
}

.mode-tabs {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.mode-btn {
  border: 1px solid var(--border-color);
  background: transparent;
  color: var(--text-primary);
  border-radius: 10px;
  padding: 0.7rem 0.8rem;
  cursor: pointer;
  text-align: left;
  transition: border-color 0.2s, box-shadow 0.2s;
}

.mode-btn.active {
  border-color: var(--btn-primary-bg);
  box-shadow: 0 0 0 1px var(--btn-primary-bg);
}

.filter-group {
  display: flex;
  flex-direction: column;
  gap: 0.45rem;
  margin-bottom: 0.9rem;
}

.filter-group label {
  color: var(--text-primary);
  font-size: 0.95rem;
  font-weight: 600;
}

.filter-group input,
.filter-group select {
  width: 100%;
  border: 1px solid var(--border-color);
  background: var(--bg-primary);
  color: var(--text-primary);
  border-radius: 10px;
  padding: 0.75rem 0.8rem;
  font: inherit;
}

.filters-actions {
  display: flex;
  gap: 0.75rem;
  margin-top: 1rem;
}

.primary-btn,
.ghost-btn {
  border-radius: 10px;
  padding: 0.75rem 1rem;
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
</style>