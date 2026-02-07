<template>
  <div id="app" :class="{ 'dark-theme': isDarkTheme, 'light-theme': !isDarkTheme }">
    <HeaderSection />
    <router-view />
    <footer>
      <p>&copy; 2025 Мой сайт. Все права защищены.</p>
    </footer>
  </div>
</template>

<script setup>
import { computed, onMounted } from "vue";
import { useThemeStore } from "@/shared/stores/theme";
import { useAuthStore } from "@/features/auth/stores/authStore";

import HeaderSection from "@/shared/components/HeaderSection.vue";

const themeStore = useThemeStore();
const authStore = useAuthStore();

const isDarkTheme = computed(() => themeStore.isDark);

onMounted(async () => {
  themeStore.loadTheme();

  // если есть токен — подтягиваем пользователя
  try {
    await authStore.bootstrap();
  } catch (e) {
    // чтобы не спамить ошибками в консоли, просто логируем и продолжаем
    console.warn("bootstrap failed", e);
  }
});
</script>

<style src="@/shared/assets/css/styles.css"></style>