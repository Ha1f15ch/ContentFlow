<template>
  <div id="app" :class="{ 'dark-theme': isDarkTheme, 'light-theme': !isDarkTheme }">
    <HeaderSection @open-login="showLoginModal = true" />
    <HeroSection />
    <ProtectedContent />
    <LoginModal v-if="showLoginModal" @close="closeModal" />
    <ConfirmModal v-if="showConfirmModal" @close="closeModal" />
    <footer>
      <p>&copy; 2025 Мой сайт. Все права защищены.</p>
    </footer>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useThemeStore } from '@/stores/theme';
import HeaderSection from '@/components/HeaderSection.vue';
import HeroSection from '@/components/HeroSection.vue';
import ProtectedContent from '@/components/ProtectedContent.vue';
import LoginModal from '@/components/LoginModal.vue';
import ConfirmModal from '@/components/ConfirmModal.vue';

const themeStore = useThemeStore();
const isDarkTheme = computed(() => themeStore.isDark);

const showLoginModal = ref(false);
const showConfirmModal = ref(false);

const closeModal = () => {
  showLoginModal.value = false;
  showConfirmModal.value = false;
};

onMounted(() => {
  themeStore.loadTheme();
});
</script>

<style src="@/assets/css/styles.css"></style>