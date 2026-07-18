<template>
  <nav class="post-sections">
    <button
      class="section-tab"
      :class="{ active: modelValue === 'feed' }"
      @click="$emit('update:modelValue', 'feed')"
    >
      Лента
    </button>

    <button
      v-if="isAuthenticated"
      class="section-tab"
      :class="{ active: modelValue === 'myPublished' }"
      @click="$emit('update:modelValue', 'myPublished')"
    >
      Мои опубликованные
    </button>

    <button
      v-if="isAuthenticated"
      class="section-tab"
      :class="{ active: modelValue === 'myDrafts' }"
      @click="$emit('update:modelValue', 'myDrafts')"
    >
      Мои черновики
    </button>

    <button
      v-if="isAuthenticated"
      class="section-tab"
      :class="{ active: modelValue === 'myArchived' }"
      @click="$emit('update:modelValue', 'myArchived')"
    >
      Мои архивные
    </button>
  </nav>
</template>

<script setup>
defineProps({
  modelValue: {
    type: String,
    default: "feed",
  },
  isAuthenticated: {
    type: Boolean,
    default: false,
  },
});

defineEmits(["update:modelValue"]);
</script>

<style scoped>
.post-sections {
  width: 100%;
  box-sizing: border-box;

  display: flex;
  flex-wrap: wrap;
  gap: 1.25rem;
  align-items: center;
  padding: 0.85rem 0 0.75rem;
  margin-bottom: 1.25rem;
  border-bottom: 1px solid var(--border-color);

  position: sticky;
  top: var(--app-header-height, 4.5rem);
  z-index: 900;
  background: var(--bg-primary);
}

.section-tab {
  border: none;
  background: transparent;
  color: var(--text-secondary);
  font: inherit;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  padding: 0.25rem 0;
  position: relative;
  transition: color 0.2s ease;
}

.section-tab:hover {
  color: var(--text-primary);
}

.section-tab.active {
  color: var(--text-primary);
}

.section-tab.active::after {
  content: "";
  position: absolute;
  left: 0;
  bottom: -0.8rem;
  width: 100%;
  height: 2px;
  background: var(--btn-primary-bg);
  border-radius: 999px;
}
</style>