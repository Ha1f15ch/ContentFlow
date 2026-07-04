<template>
  <div class="moderation-page">
    <header class="page-header">
      <h1>Модерация</h1>
      <p class="page-subtitle">
        Открытые тикеты отсортированы по приоритету: критичные — сверху.
      </p>
    </header>

    <div v-if="initialLoading" class="page-state">Загрузка тикетов...</div>
    <div v-else-if="loadError" class="page-state page-error">{{ loadError }}</div>

    <div v-else class="cases-area">
      <ModerationCaseCard
        v-for="moderationCase in cases"
        :key="moderationCase.id"
        :moderation-case="moderationCase"
        @open="openCase"
      />

      <p v-if="cases.length === 0" class="page-state">Открытых тикетов нет.</p>

      <div ref="casesSentinel" class="cases-sentinel" aria-hidden="true"></div>
      <p v-if="isLoadingMore && cases.length > 0" class="page-status">Загрузка ещё...</p>
      <p v-else-if="!hasMore && cases.length > 0" class="page-status">Все тикеты загружены.</p>
    </div>

    <ModerationCaseDetailsModal
      v-model="isDetailsOpen"
      :case-id="selectedCaseId"
      @updated="handleCaseUpdated"
    />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import { useInfiniteScroll } from "@/shared/composables/useInfiniteScroll.js";
import { moderationService } from "@/features/moderation/api/moderationService.js";
import ModerationCaseCard from "@/features/moderation/components/ModerationCaseCard.vue";
import ModerationCaseDetailsModal from "@/features/moderation/components/ModerationCaseDetailsModal.vue";

const CASES_PAGE_SIZE = 10;

const cases = ref([]);
const initialLoading = ref(true);
const isLoadingMore = ref(false);
const loadError = ref("");
const casesPage = ref(0);
const casesTotalPages = ref(0);
const casesSentinel = ref(null);

const selectedCaseId = ref(null);
const isDetailsOpen = ref(false);

const hasMore = computed(() => casesPage.value < casesTotalPages.value);

async function loadCases({ reset = false } = {}) {
  if (isLoadingMore.value) return;
  if (!reset && !hasMore.value) return;

  const nextPage = reset ? 1 : casesPage.value + 1;

  isLoadingMore.value = true;
  loadError.value = "";

  try {
    const response = await moderationService.getOpenCases(nextPage, CASES_PAGE_SIZE);
    const result = response.data ?? {};
    const items = result.items ?? [];

    cases.value = reset ? items : [...cases.value, ...items];
    casesPage.value = result.page ?? nextPage;
    casesTotalPages.value = result.totalPages ?? casesTotalPages.value;
  } catch (e) {
    loadError.value = e?.response?.data?.message ?? "Не удалось загрузить тикеты.";
  } finally {
    isLoadingMore.value = false;
    initialLoading.value = false;
  }
}

async function resetAndLoadCases() {
  cases.value = [];
  casesPage.value = 0;
  casesTotalPages.value = 0;
  initialLoading.value = true;
  await loadCases({ reset: true });
}

function openCase(caseId) {
  selectedCaseId.value = caseId;
  isDetailsOpen.value = true;
}

async function handleCaseUpdated() {
  await resetAndLoadCases();
}

useInfiniteScroll({
  sentinelRef: casesSentinel,
  canLoadMore: () => !initialLoading.value && !isLoadingMore.value && hasMore.value,
  loadMore: () => loadCases(),
});

onMounted(() => {
  resetAndLoadCases();
});
</script>

<style scoped>
.moderation-page {
  max-width: 860px;
  margin: 0 auto;
  padding: 2rem 1.5rem 3rem;
  box-sizing: border-box;
}

.page-header {
  margin-bottom: 1.25rem;
}

.page-header h1 {
  margin: 0 0 0.35rem;
  font-size: 1.8rem;
}

.page-subtitle {
  margin: 0;
  color: var(--text-secondary);
  line-height: 1.5;
}

.cases-area {
  display: flex;
  flex-direction: column;
  gap: 0.85rem;
}

.cases-sentinel {
  width: 100%;
  height: 1px;
}

.page-state,
.page-status {
  margin: 0;
  padding: 1.25rem 0;
  text-align: center;
  color: var(--text-secondary);
}

.page-error {
  color: #ef4444;
}

@media (max-width: 820px) {
  .moderation-page {
    padding: 1rem 1rem 2rem;
  }
}
</style>
