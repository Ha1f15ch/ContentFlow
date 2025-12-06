<template>
  <div id="app" :class="{ 'dark-theme': isDarkTheme, 'light-theme': !isDarkTheme }">
    <HeaderSection />
    <HeroSection />
    <ProtectedContent />
    <router-view /> <!-- ← Новое место для отображения страниц -->
    <footer>
      <p>&copy; 2025 Мой сайт. Все права защищены.</p>
    </footer>
  </div>
</template>

<script setup>
import { computed, onMounted } from 'vue';
import { useThemeStore } from '@/shared/stores/theme';

// Stores
const themeStore = useThemeStore();

// Computed
const isDarkTheme = computed(() => themeStore.isDark);

// Lifecycle
onMounted(() => {
  themeStore.loadTheme();
});

// компоненты для всего приложения (shared/common)
import HeaderSection from '@/shared/components/HeaderSection.vue';
import HeroSection from '@/shared/components/HeroSection.vue';
import ProtectedContent from '@/shared/components/ProtectedContent.vue';
</script>

<style src="@/shared/assets/css/styles.css"></style>