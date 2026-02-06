<template>
  <CategoryList
    :items="store.items"
    :isLoading="store.isLoading"
    :hasMore="store.hasMore"
    :error="store.error"
    ref="view"
  />
</template>

<script setup>
import { ref, onMounted } from "vue";
import CategoryList from "@/shared/components/CategoryList.vue";
import { useInfiniteScroll } from "@/shared/composables/useInfiniteScroll";
import { useCategoryStore } from "@/features/category/stores/categoryStore";

const store = useCategoryStore();
const view = ref(null);

onMounted(async () => {
  // если ещё не грузили — грузим первую страницу
  if (store.page === 0) {
    store.resetList({ pageSize: 50 });
    await store.fetchNext();
  }
});

useInfiniteScroll({
  sentinelRef: { get value() { return view.value?.sentinel; } },
  canLoadMore: () => store.hasMore && !store.isLoading,
  loadMore: () => store.fetchNext(),
});
</script>
