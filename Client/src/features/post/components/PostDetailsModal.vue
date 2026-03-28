<template>
  <Teleport to="body">
    <div v-if="modelValue" class="modal-overlay" @click="close">
      <div class="modal-content" @click.stop>
        <button class="modal-close" @click="close">×</button>

        <div v-if="loading" class="modal-state">
          Загрузка поста...
        </div>

        <div v-else-if="error" class="modal-state modal-error">
          {{ error }}
        </div>

        <div v-else-if="post" class="details-layout">
          <article class="post-card">
            <h2 class="post-title">{{ post.title }}</h2>
          
            <div class="post-meta">
              <span>{{ post.authorName }}</span>
              <span>•</span>
              <span>{{ formatDate(post.createdAt) }}</span>
            </div>
          
            <div v-if="post.tags?.length" class="post-tags">
              <span v-for="tag in post.tags" :key="tag.id" class="post-tag">
                {{ tag.name }}
              </span>
            </div>
          
            <div class="post-body">{{ post.content }}</div>

            <div class="post-comments-toggle-row">
              <button
                type="button"
                class="post-comments-toggle"
                @click="commentsExpanded = !commentsExpanded"
              >
                <span class="post-comments-toggle-icon">💬</span>
                <span class="post-comments-toggle-text">
                  {{ commentsExpanded ? "Скрыть комментарии" : "Показать комментарии" }}
                </span>
                <span class="post-comments-toggle-count">
                  {{ post.commentCount ?? 0 }}
                </span>
              </button>
            </div>
          </article>
        
          <PostCommentsSection
            v-if="commentsExpanded"
            class="comments-section"
            :post-id="post.id"
            :is-authenticated="isAuthenticated"
            :initial-count="post.commentCount ?? 0"
          />
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import { ref, watch, onMounted, onBeforeUnmount } from "vue";
import { postService } from "@/features/post/api/postService";
import PostCommentsSection from "@/features/comments/components/PostCommentsSection.vue";

const props = defineProps({
  modelValue: { type: Boolean, default: false },
  postId: { type: Number, default: null },
  isAuthenticated: { type: Boolean, default: false },
});

const emit = defineEmits(["update:modelValue"]);

const loading = ref(false);
const error = ref("");
const post = ref(null);
const commentsExpanded = ref(false);

const close = () => {
  post.value = null;
  error.value = "";
  loading.value = false;
  emit("update:modelValue", false);
};

const loadPost = async () => {
  if (!props.modelValue || !props.postId) return;

  loading.value = true;
  error.value = "";
  post.value = null;

  try {
    const response = await postService.getPostById(props.postId);
    post.value = response.data;
  } catch (e) {
    error.value = "Не удалось загрузить пост.";
    console.error(e);
  } finally {
    loading.value = false;
  }
};

watch(
  () => [props.modelValue, props.postId],
  async ([isOpen, id]) => {
    if (!isOpen || !id) {
      post.value = null;
      error.value = "";
      loading.value = false;
      commentsExpanded.value = false;
      return;
    }

    commentsExpanded.value = false;
    await loadPost();
  },
  { immediate: true }
);

watch(
  () => props.modelValue,
  (isOpen) => {
    if (isOpen) {
      const scrollBarWidth = window.innerWidth - document.documentElement.clientWidth;
      document.body.style.overflow = "hidden";
      document.body.style.paddingRight = `${scrollBarWidth}px`;
    } else {
      document.body.style.overflow = "";
      document.body.style.paddingRight = "";
    }
  },
  { immediate: true }
);

const onKeydown = (e) => {
  if (e.key === "Escape" && props.modelValue) {
    close();
  }
};

onMounted(() => {
  window.addEventListener("keydown", onKeydown);
});

onBeforeUnmount(() => {
  window.removeEventListener("keydown", onKeydown);
  document.body.style.overflow = "";
  document.body.style.paddingRight = "";
});

const formatDate = (isoDate) => {
  if (!isoDate) return "";
  return new Date(isoDate).toLocaleString("ru-RU", {
    year: "numeric",
    month: "long",
    day: "numeric",
    hour: "2-digit",
    minute: "2-digit",
  });
};
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.55);
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 16px;
  z-index: 1000;
}

.modal-content {
  position: relative;
  width: calc(100vw - 32px);
  max-width: 1040px;
  max-height: 92vh;
  overflow-y: auto;
  background: var(--bg-primary, #121212);
  color: var(--text-primary, #e0e0e0);
  border-radius: 18px;
  padding: 20px;
  box-shadow: 0 18px 50px rgba(0, 0, 0, 0.35);
  box-sizing: border-box;
}

.modal-close {
  position: absolute;
  top: 10px;
  right: 12px;
  border: none;
  background: transparent;
  color: var(--text-secondary, #aaa);
  font-size: 1.8rem;
  cursor: pointer;
  line-height: 1;
  z-index: 2;
}

.details-layout {
  display: flex;
  flex-direction: column;
  gap: 16px;
  width: 100%;
  padding-top: 12px;
  box-sizing: border-box;
}

.post-card {
  width: 100%;
  max-width: none;
  background: var(--card-bg);
  border: 1px solid var(--border-color);
  border-radius: 18px;
  padding: 24px 26px;
  box-sizing: border-box;
}

.comments-section {
  width: 100%;
}

.post-title {
  margin: 0 0 12px;
  color: var(--text-primary);
  font-size: 2.2rem;
  line-height: 1.2;
  word-break: break-word;
}

.post-meta {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
  color: var(--text-secondary, #a0a0a0);
  font-size: 0.95rem;
  margin-bottom: 16px;
}

.post-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin-bottom: 16px;
}

.post-tag {
  padding: 0.3rem 0.7rem;
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.08);
  font-size: 0.85rem;
}

.post-body {
  line-height: 1.9;
  font-size: 1.04rem;
  color: var(--text-primary, #e0e0e0);
  white-space: pre-line;
  word-break: break-word;
  overflow-wrap: anywhere;
}

.post-comments-toggle-row {
  margin-top: 1rem;
  display: flex;
  justify-content: flex-start;
}

.post-comments-toggle {
  display: inline-flex;
  align-items: center;
  gap: 0.55rem;
  border: none;
  background: transparent;
  color: var(--text-secondary);
  cursor: pointer;
  padding: 0.25rem 0;
  font: inherit;
  transition: color 0.2s ease, opacity 0.2s ease;
}

.post-comments-toggle:hover {
  color: var(--text-primary);
}

.post-comments-toggle-icon {
  font-size: 1rem;
  line-height: 1;
}

.post-comments-toggle-text {
  font-size: 0.95rem;
}

.post-comments-toggle-count {
  min-width: 24px;
  height: 24px;
  padding: 0 0.45rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  border-radius: 999px;
  border: 1px solid var(--border-color);
  background: var(--bg-secondary);
  color: var(--text-secondary);
  font-size: 0.82rem;
}

@media (max-width: 768px) {
  .modal-content {
    width: calc(100vw - 20px);
    padding: 14px;
  }

  .post-card {
    padding: 16px;
  }

  .post-title {
    font-size: 1.6rem;
    padding-right: 28px;
  }
}
</style>