import { defineStore } from 'pinia';

export const useThemeStore = defineStore('theme', {
  state: () => ({
    isDark: false,
  }),

  actions: {
    toggleTheme() {
      this.isDark = !this.isDark;
      this.applyTheme();
      localStorage.setItem('theme', this.isDark ? 'dark' : 'light');
    },

    loadTheme() {
      const savedTheme = localStorage.getItem('theme');
      if (savedTheme === 'dark') {
        this.isDark = true;
      }
      this.applyTheme();
    },

    applyTheme() {
      if (this.isDark) {
        document.body.classList.add('dark-theme');
        document.body.classList.remove('light-theme');
      } else {
        document.body.classList.remove('dark-theme');
        document.body.classList.add('light-theme');
      }
    },
  },
});