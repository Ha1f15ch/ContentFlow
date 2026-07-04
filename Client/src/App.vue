<template>
  <div id="app" :class="{ 'dark-theme': isDarkTheme, 'light-theme': !isDarkTheme }">
    <HeaderSection />
    <router-view />
    <LoginModal v-if="modalStore.isOpen" />
    <footer class="app-footer">
      <p>&copy; 2025 Мой сайт. Все права защищены.</p>
    </footer>
  </div>
</template>

<script setup>
import { computed, onMounted } from "vue";
import { useThemeStore } from "@/shared/stores/theme";
import { useModalStore } from "@/shared/stores/modalStore";

import HeaderSection from "@/shared/components/HeaderSection.vue";
import LoginModal from "@/features/auth/components/LoginModal.vue";

const themeStore = useThemeStore();
const modalStore = useModalStore();

const isDarkTheme = computed(() => themeStore.isDark);

onMounted(() => {
  themeStore.loadTheme();
});
</script>

<style src="@/shared/assets/css/styles.css"></style>