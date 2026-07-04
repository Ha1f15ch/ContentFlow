<template>
  <div class="report-button-wrap">
    <button
      type="button"
      class="report-btn"
      title="Пожаловаться"
      @click.stop="onClick"
    >
      Пожаловаться
    </button>

    <ReportModal
      v-model="showModal"
      :target-type="targetType"
      :target-id="targetId"
    />
  </div>
</template>

<script setup>
import { ref } from "vue";
import { useModalStore } from "@/shared/stores/modalStore";
import ReportModal from "@/features/reports/components/ReportModal.vue";

const props = defineProps({
  targetType: { type: String, required: true },
  targetId: { type: Number, required: true },
  isAuthenticated: { type: Boolean, default: false },
});

const modalStore = useModalStore();
const showModal = ref(false);

function onClick() {
  if (!props.isAuthenticated) {
    modalStore.openLoginModal();
    return;
  }

  showModal.value = true;
}
</script>

<style scoped>
.report-button-wrap {
  display: inline-flex;
}

.report-btn {
  border: 1px solid var(--border-color);
  background: var(--bg-secondary);
  color: var(--text-secondary);
  border-radius: 999px;
  padding: 0.35rem 0.75rem;
  font-size: 0.78rem;
  cursor: pointer;
  transition: color 0.2s, border-color 0.2s, background 0.2s;
}

.report-btn:hover {
  color: #ef4444;
  border-color: rgba(239, 68, 68, 0.45);
  background: rgba(239, 68, 68, 0.08);
}
</style>
