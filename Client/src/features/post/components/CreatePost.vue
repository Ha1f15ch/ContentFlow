<template>
  <div class="post-workspace">
    <div class="workspace-header">
      <div>
        <h1>Пространство создания поста</h1>
        <p class="workspace-subtitle">
          Черновики, редактирование и публикация — в одном месте.
        </p>
      </div>

      <div class="workspace-actions">
        <button class="secondary-btn" @click="resetEditor" :disabled="saving || publishing">
          Новый черновик
        </button>
      </div>
    </div>

    <div class="workspace-layout">
      <PostEditor
        :title="editor.title"
        :content="editor.content"
        :current-post-id="editor.id"
        :status="editor.status"
        :saving="saving"
        :publishing="publishing"
        :save-error="saveError"
        @update:title="editor.title = $event"
        @update:content="editor.content = $event"
        @save="saveDraft"
        @publish="publishCurrentPost"
        @reset="resetEditor"
      />

      <PostWorkspaceSidebar
        :active-tab="activeSidebarTab"
        :drafts="sidebarDrafts"
        :published-posts="publishedPosts"
        :loading-drafts="loadingDrafts"
        :loading-published="loadingPublished"
        :active-draft-id="activeDraftId"
        :active-local-draft-key="activeLocalDraftKey"
        :active-published-id="editor.status === POST_STATUS.Published ? editor.id : null"
        @change-tab="activeSidebarTab = $event"
        @select-draft="openDraft"
        @select-published="openPublished"
      />
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, reactive, ref, watch } from "vue";
import { useAuthStore } from "@/features/auth/stores/authStore";
import {
  postService,
  POST_STATUS,
} from "@/features/post/api/postService";

import PostEditor from "@/features/post/components/PostEditor.vue";
import PostWorkspaceSidebar from "@/features/post/components/PostWorkspaceSidebar.vue";

const authStore = useAuthStore();

const activeSidebarTab = ref("drafts");

const drafts = ref([]);
const localDrafts = ref([]);

const publishedPosts = ref([]);

const loadingDrafts = ref(false);
const loadingPublished = ref(false);

const saving = ref(false);
const publishing = ref(false);
const saveError = ref("");

const editor = reactive({
  id: null,
  localKey: null,
  title: "",
  content: "",
  status: POST_STATUS.Draft,
});

function resetEditor() {
  if (!editor.id && editor.localKey) {
    const existingIndex = localDrafts.value.findIndex(x => x.localKey === editor.localKey);

    const currentDraft = existingIndex >= 0 ? localDrafts.value[existingIndex] : null;
    const isEmptyCurrentDraft =
      currentDraft &&
      !(currentDraft.title && currentDraft.title !== "(без названия)") &&
      !currentDraft.content?.trim();

    if (isEmptyCurrentDraft) {
      localDrafts.value.splice(existingIndex, 1);
    }
  }

  const localDraft = createEmptyLocalDraft();

  editor.id = null;
  editor.localKey = localDraft.localKey;
  editor.title = "";
  editor.content = "";
  editor.status = POST_STATUS.Draft;
  saveError.value = "";

  localDrafts.value.unshift(localDraft);
}

const sidebarDrafts = computed(() => {
  return [...localDrafts.value, ...drafts.value];
});

const activeDraftId = computed(() => {
  return editor.id ? editor.id : null;
});

const activeLocalDraftKey = computed(() => {
  return !editor.id ? editor.localKey : null;
});

function createEmptyLocalDraft() {
  return {
    id: null,
    localKey: `local-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`,
    title: "",
    content: "",
    status: POST_STATUS.Draft,
    createdAt: new Date().toISOString(),
    excerpt: "",
    isLocal: true,
  };
}

function upsertCurrentLocalDraft() {
  if (editor.id || !editor.localKey) return;

  const existingIndex = localDrafts.value.findIndex(x => x.localKey === editor.localKey);

  const existingDraft = existingIndex >= 0 ? localDrafts.value[existingIndex] : null;

  const draftModel = {
    id: null,
    localKey: editor.localKey,
    title: editor.title.trim() || "(без названия)",
    content: editor.content,
    status: POST_STATUS.Draft,
    createdAt: existingDraft?.createdAt ?? new Date().toISOString(),
    excerpt: buildExcerpt(editor.content),
    isLocal: true,
  };

  if (existingIndex >= 0) {
    localDrafts.value[existingIndex] = draftModel;
  } else {
    localDrafts.value.unshift(draftModel);
  }
}

function buildExcerpt(content) {
  if (!content) return "";
  return content.length <= 140 ? content : content.slice(0, 140) + "...";
}

async function loadDrafts() {
  if (!authStore.user?.id) return;

  loadingDrafts.value = true;
  try {
    const response = await postService.getMyDrafts(authStore.user.id);
    drafts.value = response.data?.items ?? [];
  } catch (err) {
    console.error("Ошибка загрузки черновиков", err);
  } finally {
    loadingDrafts.value = false;
  }
}

async function loadPublishedPosts() {
  if (!authStore.user?.id) return;

  loadingPublished.value = true;
  try {
    const response = await postService.getMyPublished(authStore.user.id);
    publishedPosts.value = response.data?.items ?? [];
  } catch (err) {
    console.error("Ошибка загрузки опубликованных постов", err);
  } finally {
    loadingPublished.value = false;
  }
}

async function openPostInEditor(id) {
  upsertCurrentLocalDraft();

  try {
    const response = await postService.getPostById(id);
    const post = response.data;

    editor.id = post.id;
    editor.localKey = null;
    editor.title = post.title ?? "";
    editor.content = post.content ?? "";
    editor.status = post.status;
    saveError.value = "";
  } catch (err) {
    console.error("Ошибка загрузки поста", err);
    saveError.value = "Не удалось загрузить пост для редактирования.";
  }
}

async function openDraft(post) {
  if (post.isLocal) {
    editor.id = null;
    editor.localKey = post.localKey;
    editor.title = post.title === "(Без названия)" ? "" : (post.title ?? "");
    editor.content = post.content ?? "";
    editor.status = POST_STATUS.Draft;
    saveError.value = "";
    return;
  }

  await openPostInEditor(post.id);
}

async function openPublished(post) {
  await openPostInEditor(post.id);
}

async function saveDraft() {
  saveError.value = "";

  if (!editor.title.trim() || !editor.content.trim()) {
    saveError.value = "Заголовок и содержание обязательны.";
    return;
  }

  saving.value = true;

  try {
    if (!editor.id) {
      const oldLocalKey = editor.localKey;
        
      const response = await postService.createPost({
        title: editor.title.trim(),
        content: editor.content.trim(),
      });
    
      editor.id = response.data?.id ?? response.data?.postId ?? null;
    
      if (oldLocalKey) {
        localDrafts.value = localDrafts.value.filter(x => x.localKey !== oldLocalKey);
      }
    
      editor.localKey = null;
    } else {
      await postService.updatePost(editor.id, {
        title: editor.title.trim(),
        content: editor.content.trim(),
        tagIds: [],
      });
    }

    await loadDrafts();
    await loadPublishedPosts();
  } catch (err) {
    console.error("Ошибка сохранения черновика", err);
    saveError.value = err?.response?.data?.message || "Не удалось сохранить черновик.";
  } finally {
    saving.value = false;
  }
}

async function publishCurrentPost() {
  saveError.value = "";

  if (!editor.title.trim() || !editor.content.trim()) {
    saveError.value = "Перед публикацией заполните заголовок и содержание.";
    return;
  }

  publishing.value = true;

  try {
    if (!editor.id) {
      const oldLocalKey = editor.localKey;

      const createResponse = await postService.createPost({
        title: editor.title.trim(),
        content: editor.content.trim(),
      });
    
      editor.id = createResponse.data?.id ?? createResponse.data?.postId ?? null;
    
      if (oldLocalKey) {
        localDrafts.value = localDrafts.value.filter(x => x.localKey !== oldLocalKey);
      }
    
      editor.localKey = null;
    } else {
      await postService.updatePost(editor.id, {
        title: editor.title.trim(),
        content: editor.content.trim(),
        tagIds: [],
      });
    }

    await postService.publishPost(editor.id);

    await loadDrafts();
    await loadPublishedPosts();
    resetEditor();
  } catch (err) {
    console.error("Ошибка публикации поста", err);
    saveError.value = err?.response?.data?.message || "Не удалось опубликовать пост.";
  } finally {
    publishing.value = false;
  }
}

onMounted(async () => {
  await authStore.bootstrap();

  if (!authStore.user?.id) {
    return;
  }

  resetEditor();

  await Promise.all([
    loadDrafts(),
    loadPublishedPosts(),
  ]);
});

watch(
  () => [editor.title, editor.content],
  () => {
    upsertCurrentLocalDraft();
  }
);
</script>

<style scoped>
.post-workspace {
  width: 100%;
  max-width: 1500px;
  margin: 0 auto;
  padding: 1.5rem;
}

.workspace-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.workspace-header h1 {
  margin: 0 0 0.4rem;
  color: var(--text-primary);
}

.workspace-subtitle {
  margin: 0;
  color: var(--text-secondary);
}

.workspace-actions {
  display: flex;
  gap: 0.75rem;
}

.workspace-layout {
  display: grid;
  width: 100%;
  grid-template-columns: minmax(0, 1.9fr) 380px;
  gap: 1.5rem;
  align-items: start;
}

.secondary-btn {
  border: 1px solid var(--border-color);
  background: transparent;
  color: var(--text-primary);
  padding: 0.8rem 1rem;
  border-radius: 10px;
  cursor: pointer;
}

@media (max-width: 1100px) {
  .workspace-layout {
    grid-template-columns: 1fr;
  }
}
</style>