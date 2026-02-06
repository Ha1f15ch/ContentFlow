import { onBeforeUnmount, watch } from "vue";

export function useInfiniteScroll({ sentinelRef, canLoadMore, loadMore }) {
  let observer = null;

  function ensureObserver() {
    if (observer) return;
    observer = new IntersectionObserver(
      (entries) => {
        const entry = entries[0];
        if (entry?.isIntersecting && canLoadMore()) loadMore();
      },
      { root: null, threshold: 0.1 }
    );
  }

  watch(
    () => sentinelRef.value,
    (el, prev) => {
      ensureObserver();
      if (prev) observer.unobserve(prev);
      if (el) observer.observe(el);
    },
    { immediate: true }
  );

  onBeforeUnmount(() => observer?.disconnect());
}
