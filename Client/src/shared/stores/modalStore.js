import { defineStore } from "pinia";

export const useModalStore = defineStore("modal", {
  state: () => ({
    isOpen: false,
  }),

  actions: {
    openLoginModal() {
      this.isOpen = true;
    },

    closeModal() {
      this.isOpen = false;
    },
  },
});
