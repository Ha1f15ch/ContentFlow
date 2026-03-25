<template>
  <aside class="sidebar-panel">
    <div class="sidebar-tabs">
      <button
        class="tab-btn"
        :class="{ active: activeTab === 'drafts' }"
        @click="$emit('change-tab', 'drafts')"
      >
        Черновики
      </button>

      <button
        class="tab-btn"
        :class="{ active: activeTab === 'published' }"
        @click="$emit('change-tab', 'published')"
      >
        Опубликованные
      </button>
    </div>

    <div class="sidebar-content">
      <template v-if="activeTab === 'drafts'">
        <div v-if="loadingDrafts" class="sidebar-state">Загрузка черновиков...</div>

        <div v-else-if="!drafts.length" class="sidebar-state">
          Черновиков пока нет.
        </div>

        <div v-else class="post-mini-list">
          <button
            v-for="post in drafts"
            :key="post.id"
            class="mini-post-card"
            :class="{active: isDraftActive(post)}"
            @click="$emit('select-draft', post)"
          >
            <div class="mini-post-title">{{ post.title || "(без названия)" }}</div>
            <div class="mini-post-date">{{ formatDate(post.createdAt) }}</div>
            <div class="mini-post-excerpt">{{ post.excerpt }}</div>
          </button>
        </div>
      </template>

      <template v-else>
        <div v-if="loadingPublished" class="sidebar-state">Загрузка опубликованных...</div>

        <div v-else-if="!publishedPosts.length" class="sidebar-state">
          Опубликованных постов пока нет.
        </div>

        <div v-else class="post-mini-list">
          <button
            v-for="post in publishedPosts"
            :key="post.id"
            class="mini-post-card"
            :class="{ active: isPublishedActive(post) }"
            @click="$emit('select-published', post)"
          >
            <div class="mini-post-title">{{ post.title || "(без названия)" }}</div>
            <div class="mini-post-date">{{ formatDate(post.createdAt) }}</div>
            <div class="mini-post-excerpt">{{ post.excerpt }}</div>
          </button>
        </div>
      </template>
    </div>
  </aside>
</template>

<script setup>
const props = defineProps({
  activeTab: { type: String, default: "drafts" },
  drafts: { type: Array, default: () => [] },
  publishedPosts: { type: Array, default: () => [] },
  loadingDrafts: { type: Boolean, default: false },
  loadingPublished: { type: Boolean, default: false },
  activeDraftId: { type: Number, default: null },
  activeLocalDraftKey: { type: String, default: null },
  activePublishedId: { type: Number, default: null },
});

defineEmits([
  "change-tab",
  "select-draft",
  "select-published",
]);

const formatDate = (isoDate) => {
  if (!isoDate) return "";
  return new Date(isoDate).toLocaleString("ru-RU", {
    year: "numeric",
    month: "short",
    day: "numeric",
  });
};

const isDraftActive = (post) => {
  if (post.isLocal) {
    return props.activeLocalDraftKey === post.localKey;
  }

  return props.activeDraftId === post.id;
};

const isPublishedActive = (post) => {
  return props.activePublishedId === post.id;
};

</script>

<style scoped>
.sidebar-panel {
  width: 100%;
  min-width: 0;
  background: var(--card-bg);
  border: 1px solid var(--border-color);
  border-radius: 18px;
  padding: 1rem;
  min-height: 640px;
}

.sidebar-tabs {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.tab-btn {
  flex: 1;
  border: 1px solid var(--border-color);
  background: transparent;
  color: var(--text-primary);
  border-radius: 10px;
  padding: 0.75rem;
  cursor: pointer;
}

.tab-btn.active {
  border-color: var(--btn-primary-bg);
  color: var(--btn-primary-bg);
}

.sidebar-content {
  display: flex;
  flex-direction: column;
}

.sidebar-state {
  color: var(--text-secondary);
  padding: 1rem 0.25rem;
}

.post-mini-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.mini-post-card {
  width: 100%;
  display: block;
  text-align: left;
  border: 1px solid var(--border-color);
  background: var(--bg-primary);
  color: var(--text-primary);
  border-radius: 12px;
  padding: 0.85rem;
  cursor: pointer;
  transition: border-color 0.2s, box-shadow 0.2s, background-color 0.2s;
}

.mini-post-card:hover {
  border-color: var(--btn-primary-bg);
}

.mini-post-card.active {
  border-color: var(--btn-primary-bg);
  box-shadow: 0 0 0 1px var(--btn-primary-bg);
  background: rgba(26, 115, 232, 0.08);
}

.mini-post-title {
  font-weight: 600;
  margin-bottom: 0.35rem;
}

.mini-post-date {
  color: var(--text-secondary);
  font-size: 0.85rem;
  margin-bottom: 0.5rem;
}

.mini-post-excerpt {
  color: var(--text-secondary);
  font-size: 0.92rem;
  line-height: 1.4;
}
</style>