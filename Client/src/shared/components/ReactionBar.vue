<template>
  <div class="reaction-bar" @click.stop>
    <button
      type="button"
      class="reaction-btn"
      :class="{ active: normalizedReaction === REACTION_TYPE.Like }"
      :disabled="pending"
      :aria-pressed="normalizedReaction === REACTION_TYPE.Like"
      @click="handleClick(REACTION_TYPE.Like)"
    >
      <span class="reaction-icon">▲</span>
      <span class="reaction-count">{{ normalizedLikes }}</span>
    </button>

    <button
      type="button"
      class="reaction-btn"
      :class="{ active: normalizedReaction === REACTION_TYPE.Dislike }"
      :disabled="pending"
      :aria-pressed="normalizedReaction === REACTION_TYPE.Dislike"
      @click="handleClick(REACTION_TYPE.Dislike)"
    >
      <span class="reaction-icon">▼</span>
      <span class="reaction-count">{{ normalizedDislikes }}</span>
    </button>
  </div>
</template>

<script setup>
import { computed } from "vue";
import { REACTION_TYPE } from "@/features/reactions/api/reactionService";

const props = defineProps({
  likesCount: { type: Number, default: 0 },
  dislikesCount: { type: Number, default: 0 },
  currentUserReaction: { type: [Number, String, null], default: null },
  isAuthenticated: { type: Boolean, default: false },
  pending: { type: Boolean, default: false },
});

const emit = defineEmits(["set", "remove", "request-auth"]);

const normalizedLikes = computed(() => Number(props.likesCount ?? 0));
const normalizedDislikes = computed(() => Number(props.dislikesCount ?? 0));

const normalizedReaction = computed(() => {
  if (props.currentUserReaction === "Like") return REACTION_TYPE.Like;
  if (props.currentUserReaction === "Dislike") return REACTION_TYPE.Dislike;
  if (props.currentUserReaction === REACTION_TYPE.Like) return REACTION_TYPE.Like;
  if (props.currentUserReaction === REACTION_TYPE.Dislike) return REACTION_TYPE.Dislike;
  return null;
});

function handleClick(reactionType) {
  if (props.pending) return;

  if (!props.isAuthenticated) {
    emit("request-auth");
    return;
  }

  if (normalizedReaction.value === reactionType) {
    emit("remove");
    return;
  }

  emit("set", reactionType);
}
</script>

<style scoped>
.reaction-bar {
  display: inline-flex;
  align-items: center;
  gap: 0.35rem;
}

.reaction-btn {
  min-width: 48px;
  height: 32px;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.35rem;
  border: 1px solid var(--border-color);
  border-radius: 999px;
  background: var(--bg-secondary);
  color: var(--text-secondary);
  cursor: pointer;
  font: inherit;
  font-weight: 700;
  transition: background 0.2s ease, border-color 0.2s ease, color 0.2s ease, transform 0.2s ease;
}

.reaction-btn:hover:not(:disabled) {
  transform: translateY(-1px);
  border-color: var(--btn-primary-bg);
  color: var(--text-primary);
}

.reaction-btn.active {
  border-color: var(--btn-primary-bg);
  background: color-mix(in srgb, var(--btn-primary-bg) 16%, transparent);
  color: var(--btn-primary-bg);
}

.reaction-btn:disabled {
  cursor: wait;
  opacity: 0.7;
}

.reaction-icon {
  font-size: 0.9rem;
  line-height: 1;
}

.reaction-count {
  min-width: 1ch;
  font-size: 0.9rem;
}
</style>
