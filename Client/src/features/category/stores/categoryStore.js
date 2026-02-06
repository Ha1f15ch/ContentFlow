import { defineStore } from "pinia";
import { categoryService } from "../api/categoryService";

export const useCategoryStore = defineStore("categories", {
  state: () => ({
    // текущий "list state"
    items: [],
    page: 0,
    pageSize: 50,
    totalCount: 0,
    totalPages: 0,

    isLoading: false,
    error: null,

    // простой кэш для fetchPage (опционально)
    cache: new Map(),
  }),

  getters: {
    hasMore: (s) => (s.totalPages === 0 ? true : s.page < s.totalPages),
    nextPage: (s) => s.page + 1,
  },

  actions: {
    resetList({ pageSize = 50 } = {}) {
      this.items = [];
      this.page = 0;
      this.pageSize = pageSize;
      this.totalCount = 0;
      this.totalPages = 0;
      this.isLoading = false;
      this.error = null;
    },

    _applyPaged(result, { append } = { append: false }) {
      const list = result.items ?? [];

      this.items = append ? [...this.items, ...list] : list;
      this.page = result.page ?? this.page;
      this.pageSize = result.pageSize ?? this.pageSize;
      this.totalCount = result.totalCount ?? 0;
      this.totalPages = result.totalPages ?? 0;
    },

    // обычная пагинация (админка/таблица)
    async fetchPage({ page = 1, pageSize = 10, force = false } = {}) {
      const key = `${page}|${pageSize}`;

      if (!force && this.cache.has(key)) {
        this._applyPaged(this.cache.get(key), { append: false });
        return;
      }

      this.isLoading = true;
      this.error = null;
      try {
        const { data } = await categoryService.list({ page, pageSize });
        this.cache.set(key, data);
        this._applyPaged(data, { append: false });
      } catch (e) {
        this.error = e;
      } finally {
        this.isLoading = false;
      }
    },

    // infinite scroll
    async fetchNext() {
      if (this.isLoading || !this.hasMore) return;

      this.isLoading = true;
      this.error = null;
      try {
        const { data } = await categoryService.list({
          page: this.nextPage,
          pageSize: this.pageSize,
        });

        this._applyPaged(data, { append: true });
      } catch (e) {
        this.error = e;
      } finally {
        this.isLoading = false;
      }
    },

    invalidateCache() {
      this.cache.clear();
    },

    async add(payload) {
      const { data } = await categoryService.create(payload);
      this.invalidateCache();
      return data;
    },

    async edit(id, payload) {
      const { data } = await categoryService.update(id, payload);
      this.invalidateCache();
      return data;
    },

    async remove(id) {
      await categoryService.remove(id);
      this.invalidateCache();
    },
  },
});
