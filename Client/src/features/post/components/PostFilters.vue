<template>
  <aside class="post-filters">
    <div class="filters-card">
      <button class="filters-toggle" @click="isOpen = !isOpen">
        <span>Фильтры</span>
        <span class="filters-toggle-icon">{{ isOpen ? "−" : "+" }}</span>
      </button>

      <div v-show="isOpen" class="filters-body">

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
    </div>
  </aside>
</template>

<script setup>
import { ref } from "vue";
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
});

const emit = defineEmits([
  "update:modelValue",
  "apply",
  "reset",
]);

const isOpen = ref(false);

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
  box-sizing: border-box;
}

.filters-card {
  width: 100%;
  box-sizing: border-box;

  background: var(--card-bg);
  border: 1px solid var(--border-color);
  border-radius: 16px;
  padding: 0.9rem 1rem;
}

.filters-toggle {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: space-between;
  border: none;
  background: transparent;
  color: var(--text-primary);
  font: inherit;
  font-size: 1.05rem;
  font-weight: 700;
  cursor: pointer;
  padding: 0;
}

.filters-toggle-icon {
  font-size: 1.2rem;
  color: var(--text-secondary);
}

.filters-body {
  width: 100%;
  margin-top: 1rem;
  box-sizing: border-box;
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
  box-sizing: border-box;
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