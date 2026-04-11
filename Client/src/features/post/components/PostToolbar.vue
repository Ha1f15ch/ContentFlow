<template>
  <div class="post-toolbar">
    <input
      :value="search"
      type="text"
      class="toolbar-search"
      placeholder="Искать по постам"
      @input="$emit('update:search', $event.target.value)"
      @keydown.enter="$emit('applySearch')"
    />

    <button
      type="button"
      class="toolbar-filters-btn"
      :class="{ active: filtersOpen }"
      @click="$emit('update:filtersOpen', !filtersOpen)"
    >
      Фильтры
    </button>
  </div>
</template>

<script setup>
defineProps({
  search: {
    type: String,
    default: "",
  },
  filtersOpen: {
    type: Boolean,
    default: false,
  },
});

defineEmits([
  "update:search",
  "update:filtersOpen",
  "applySearch",
]);
</script>

<style scoped>
.post-toolbar {
  width: 100%;
  display: flex;
  gap: 0.75rem;
  align-items: center;
  box-sizing: border-box;
}

.toolbar-search {
  flex: 1;
  min-width: 0;
  border: 1px solid var(--border-color);
  background: var(--bg-primary);
  color: var(--text-primary);
  border-radius: 12px;
  padding: 0.8rem 0.9rem;
  font: inherit;
  box-sizing: border-box;
}

.toolbar-filters-btn {
  border: 1px solid var(--border-color);
  background: var(--card-bg);
  color: var(--text-primary);
  border-radius: 12px;
  padding: 0.8rem 1rem;
  font: inherit;
  cursor: pointer;
  white-space: nowrap;
}

.toolbar-filters-btn.active {
  border-color: var(--btn-primary-bg);
  color: var(--text-primary);
}

@media (max-width: 640px) {
  .post-toolbar {
    flex-direction: column;
    align-items: stretch;
  }

  .toolbar-filters-btn {
    width: 100%;
  }
}
</style>