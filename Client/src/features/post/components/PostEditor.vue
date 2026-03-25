<template>
  <section class="editor-panel">
    <div class="editor-header">
      <div>
        <h2>{{ editorTitle }}</h2>
        <p class="editor-hint">
          {{ editorHint }}
        </p>
      </div>
    </div>

    <div class="editor-form">
      <label class="field-label" for="post-title">Заголовок</label>
      <input
        id="post-title"
        class="title-input"
        :value="title"
        type="text"
        placeholder="Введите заголовок поста"
        @input="$emit('update:title', $event.target.value)"
      />

      <label class="field-label" for="post-content">Содержание</label>
      <textarea
        id="post-content"
        class="content-textarea"
        :value="content"
        placeholder="Введите содержание поста"
        @input="$emit('update:content', $event.target.value)"
      ></textarea>

      <div v-if="saveError" class="save-error">
        {{ saveError }}
      </div>

      <div class="editor-actions">
        <template v-if="isPublished">
          <button class="primary-btn" @click="$emit('save')" :disabled="saving || publishing">
            {{ saving ? "Сохраняю..." : "Сохранить изменения" }}
          </button>
        </template>

        <template v-else>
          <button class="primary-btn" @click="$emit('save')" :disabled="saving || publishing">
            {{ saving ? "Сохраняю..." : "Сохранить черновик" }}
          </button>

          <button class="publish-btn" @click="$emit('publish')" :disabled="saving || publishing">
            {{ publishing ? "Публикую..." : "Опубликовать" }}
          </button>
        </template>

        <button class="ghost-btn" @click="$emit('reset')" :disabled="saving || publishing">
          Очистить
        </button>
      </div>
    </div>
  </section>
</template>

<script setup>
import { computed } from "vue";
import { POST_STATUS } from "@/features/post/api/postService";

const props = defineProps({
  title: { type: String, default: "" },
  content: { type: String, default: "" },
  currentPostId: { type: Number, default: null },
  status: { type: Number, default: null },
  saving: { type: Boolean, default: false },
  publishing: { type: Boolean, default: false },
  saveError: { type: String, default: "" },
});

defineEmits([
  "update:title",
  "update:content",
  "save",
  "publish",
  "reset",
]);

const isPublished = computed(() => props.status === POST_STATUS.Published);

const editorTitle = computed(() => {
  if (isPublished.value) return "Редактирование опубликованного поста";
  return props.currentPostId ? "Редактирование поста" : "Новый пост";
});

const editorHint = computed(() => {
  if (isPublished.value) {
    return "Этот пост уже опубликован. Можно сохранить изменения без повторной публикации.";
  }

  return "Сначала сохрани как черновик, затем публикуй отдельно.";
});
</script>

<style scoped>
.editor-panel {
  width: 100%;
  min-width: 0;
  min-height: 640px;
  background: var(--card-bg);
  border: 1px solid var(--border-color);
  border-radius: 18px;
  padding: 1.5rem;
  display: flex;
  flex-direction: column;
}

.editor-header {
  margin-bottom: 1.25rem;
  flex-shrink: 0;
}

.editor-header h2 {
  margin: 0 0 0.35rem;
  color: var(--text-primary);
}

.editor-hint {
  margin: 0;
  color: var(--text-secondary);
}

.editor-form {
  display: flex;
  flex-direction: column;
  gap: 0.85rem;
  flex: 1;
  min-height: 0;
}

.field-label {
  font-weight: 600;
  color: var(--text-primary);
}

.title-input,
.content-textarea {
  width: 100%;
  border: 1px solid var(--border-color);
  background: var(--bg-primary);
  color: var(--text-primary);
  border-radius: 12px;
  padding: 0.9rem 1rem;
  font: inherit;
}

.title-input {
  font-size: 1.05rem;
  flex-shrink: 0;
}

.content-textarea {
  flex: 1;
  min-height: 260px;
  max-height: 100%;
  resize: none;
  line-height: 1.6;
  overflow-y: auto;
}

.editor-actions {
  display: flex;
  gap: 0.75rem;
  flex-wrap: wrap;
  margin-top: auto;
  padding-top: 0.5rem;
  flex-shrink: 0;
}

.primary-btn,
.publish-btn,
.ghost-btn {
  border: none;
  border-radius: 10px;
  padding: 0.85rem 1.1rem;
  font: inherit;
  cursor: pointer;
}

.primary-btn {
  background: var(--btn-primary-bg);
  color: white;
}

.publish-btn {
  background: #2f7d32;
  color: white;
}

.ghost-btn {
  background: transparent;
  color: var(--text-primary);
  border: 1px solid var(--border-color);
}

.save-error {
  color: #ff6b6b;
}
</style>