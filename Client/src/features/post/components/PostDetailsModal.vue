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

        <article v-else-if="post" class="post-details">
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

            <section class="post-comments">
                <h3 class="comments-title">Комментарии</h3>

                <div class="comments-placeholder">
                    Здесь будет список комментариев.
                </div>

                <div v-if="props.isAuthenticated" class="comment-form-placeholder">
                  Здесь будет форма добавления комментария.
                </div>

                <div v-else class="auth-hint">
                    Войдите, чтобы оставлять комментарии и ставить оценки.
                </div>
            </section>
        </article>
      </div>
    </div>
  </Teleport>
</template>

<script setup>
import { ref, watch, onMounted, onBeforeUnmount } from "vue";
import { postService } from "@/features/post/api/postService";

const props = defineProps({
  modelValue: { type: Boolean, default: false },
  postId: { type: Number, default: null },
  isAuthenticated: { type: Boolean, default: false },
});

const emit = defineEmits(["update:modelValue"]);

const loading = ref(false);
const error = ref("");
const post = ref(null);

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
      return;
    }

    await loadPost();
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
  padding: 1rem;
  z-index: 1000;
}

.modal-content {
  position: relative;
  width: min(900px, 100%);
  max-height: 90vh;
  overflow-y: auto;
  background: var(--bg-primary, #121212);
  color: var(--text-primary, #e0e0e0);
  border-radius: 16px;
  padding: 2rem 1.5rem;
  box-shadow: 0 18px 50px rgba(0, 0, 0, 0.35);
}

.modal-close {
  position: absolute;
  top: 0.8rem;
  right: 0.9rem;
  border: none;
  background: transparent;
  color: var(--text-secondary, #aaa);
  font-size: 1.8rem;
  cursor: pointer;
}

.modal-state {
  padding: 2rem 1rem;
  text-align: center;
}

.modal-error {
  color: #ff6b6b;
}

.post-details {
  padding-top: 0.5rem;
}

.post-title {
  margin: 0 0 0.8rem;
  font-size: 2rem;
  line-height: 1.2;
}

.post-meta {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
  color: var(--text-secondary, #a0a0a0);
  font-size: 0.95rem;
  margin-bottom: 1rem;
}

.post-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  margin-bottom: 1.2rem;
}

.post-tag {
  padding: 0.3rem 0.7rem;
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.08);
  font-size: 0.85rem;
}

.post-body {
  line-height: 1.7;
  color: var(--text-primary, #e0e0e0);
  word-break: break-word;
  white-space: pre-line;
}


</style>