<template>
  <div id="app" :class="{ 'dark-theme': isDarkTheme, 'light-theme': !isDarkTheme }">
    <HeaderSection />
    <HeroSection />
    <ProtectedContent />
    
    <LoginModal 
      v-if="modalStore.currentModal === 'login'" 
      @close="modalStore.closeModal" 
    />

    <ConfirmModal 
      v-if="modalStore.currentModal === 'confirmEmail'"
      :email="modalStore.modalData.email"
      @close="modalStore.closeModal"
    />
    
    <footer>
      <p>&copy; 2025 Мой сайт. Все права защищены.</p>
    </footer>
  </div>
</template>

<script setup>
import {computed, onMounted } from 'vue';
import { useThemeStore } from '@/shared/stores/theme';
import { useModalStore } from '@/shared/stores/modalStore';

// Stores
const themeStore = useThemeStore()
const modalStore = useModalStore()

// Computed
const isDarkTheme = computed(() => themeStore.isDark)

// Lifecycle
onMounted(() => {
  themeStore.loadTheme()
})

// компоненты для всего приложения (shared/common)
import HeaderSection from '@/shared/components/HeaderSection.vue';
import HeroSection from '@/shared/components/HeroSection.vue';
import ProtectedContent from '@/shared/components/ProtectedContent.vue';
import LoginModal from '@/features/auth/components/LoginModal.vue';
import ConfirmModal from '@/shared/components/ConfirmModal.vue';
</script>

<style src="@/shared/assets/css/styles.css"></style>