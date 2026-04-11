import { defineStore } from "pinia";
import {
  POST_SORT_BY,
  SORT_DIRECTION,
} from "@/features/post/api/postService";

function createDefaultFilters() {
  return {
    search: "",
    categoryId: null,
    createdFrom: "",
    sort: {
      sortBy: POST_SORT_BY.CreatedAt,
      direction: SORT_DIRECTION.Desc,
    },
  };
}

export const usePostFeedUiStore = defineStore("postFeedUi", {
  state: () => ({
    filtersOpen: false,
    filters: createDefaultFilters(),
    applyVersion: 0,
  }),

  actions: {
    setSearch(value) {
      this.filters.search = value;
    },

    setField(field, value) {
      this.filters[field] = value;
    },

    setSortField(field, value) {
      this.filters.sort[field] = value;
    },

    toggleFilters() {
      this.filtersOpen = !this.filtersOpen;
    },

    closeFilters() {
      this.filtersOpen = false;
    },

    applyFilters() {
      this.applyVersion++;
    },

    resetFilters() {
      this.filters = createDefaultFilters();
      this.applyVersion++;
    },
  },
});