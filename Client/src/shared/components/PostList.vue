<template>
  <div class="post-list-wrap">
    <div class="post-list">
      <article
        v-for="post in posts"
        :key="post.id"
        class="post-row"
        @click="$emit('open', post.id)"
      >
        <h3 class="post-title">
          {{ post.title ?? "(без названия)" }}
        </h3>

        <div class="post-meta">
          <RouterLink
            v-if="post.authorProfileId"
            class="author-link"
            :to="{ name: 'userProfile', params: { profileId: post.authorProfileId } }"
            @click.stop
          >
            {{ post.authorName ?? "-" }}
          </RouterLink>
          <span v-else>{{ post.authorName ?? "-" }}</span>
          <span class="post-dot">•</span>
          <span>{{ formatDate(post.createdAt ?? post.createdAtUtc) }}</span>
        </div>

        <p class="post-excerpt">
          {{ post.excerpt ?? "" }}
        </p>

        <div class="post-footer">
          <ReactionBar
            :likes-count="post.likesCount ?? 0"
            :dislikes-count="post.dislikesCount ?? 0"
            :current-user-reaction="post.currentUserReaction"
            :is-authenticated="isAuthenticated"
            :pending="pendingPostId === post.id"
            @set="(reactionType) => setReaction(post.id, reactionType)"
            @remove="removeReaction(post.id)"
            @request-auth="openLoginModal"
          />
          <span class="post-more-link">Читать далее</span>
        </div>
      </article>

      <div v-if="posts.length === 0" class="empty-state">
        Постов пока нет.
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";
import { reactionService } from "@/features/reactions/api/reactionService";
import { useModalStore } from "@/shared/stores/modalStore";
import ReactionBar from "@/shared/components/ReactionBar.vue";

defineProps({
  posts: { type: Array, default: () => [] },
  isAuthenticated: { type: Boolean, default: false },
});

const emit = defineEmits(["open", "reaction-updated"]);
const modalStore = useModalStore();
const pendingPostId = ref(null);

function openLoginModal() {
  modalStore.openLoginModal();
}

async function setReaction(postId, reactionType) {
  if (pendingPostId.value) return;

  pendingPostId.value = postId;
  try {
    const response = await reactionService.setPostReaction(postId, reactionType);
    emit("reaction-updated", response.data);
  } finally {
    pendingPostId.value = null;
  }
}

async function removeReaction(postId) {
  if (pendingPostId.value) return;

  pendingPostId.value = postId;
  try {
    const response = await reactionService.removePostReaction(postId);
    emit("reaction-updated", response.data);
  } finally {
    pendingPostId.value = null;
  }
}

const formatDate = (isoDate) => {
  if (!isoDate) return "";
  const date = new Date(isoDate);
  return date.toLocaleString("ru-RU", {
    year: "numeric",
    month: "short",
    day: "numeric",
  });
};
</script>

<style scoped>
.post-list-wrap {
  width: 100%;
  box-sizing: border-box;
}

.post-list {
  width: 100%;
  box-sizing: border-box;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.post-row {
  width: 100%;
  box-sizing: border-box;
  background: var(--card-bg);
  border: 1px solid var(--border-color);
  border-radius: 18px;
  padding: 1.15rem 1.15rem 1rem;
  cursor: pointer;
  transition: border-color 0.2s, transform 0.2s, box-shadow 0.2s;
}

.post-row:hover {
  border-color: var(--btn-primary-bg);
  transform: translateY(-1px);
  box-shadow: 0 10px 24px rgba(0, 0, 0, 0.14);
}

.post-title {
  margin: 0 0 0.55rem;
  color: var(--text-primary);
  font-size: 1.15rem;
  line-height: 1.35;
}

.post-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 0.4rem;
  color: var(--text-secondary);
  font-size: 0.88rem;
  margin-bottom: 0.85rem;
}

.post-dot {
  opacity: 0.65;
}

.author-link {
  color: inherit;
  text-decoration: none;
  font-weight: 600;
}

.author-link:hover {
  color: var(--btn-primary-bg);
  text-decoration: underline;
}

.post-excerpt {
  margin: 0;
  color: var(--text-secondary);
  line-height: 1.6;
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}

.post-footer {
  margin-top: 0.85rem;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  flex-wrap: wrap;
}

.post-more-link {
  color: var(--btn-primary-bg);
  font-size: 0.92rem;
  font-weight: 600;
  opacity: 0.95;
}

.empty-state {
  width: 100%;
  box-sizing: border-box;
  text-align: center;
  padding: 3rem 1rem;
  color: var(--text-secondary);
}
</style>