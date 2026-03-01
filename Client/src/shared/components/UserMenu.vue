<!-- src/components/UserMenu.vue -->
<template>
  <div class="user-menu" ref="root">
    <button class="user-btn" @click="toggleMenu" type="button" aria-haspopup="menu" :aria-expanded="showMenu">
      <span class="user-name">{{ userName }}</span>

      <span class="avatar" :style="avatarStyle" aria-hidden="true">
        <img
          v-if="!avatarError && resolvedAvatar"
          :src="resolvedAvatar"
          alt=""
          @error="avatarError = true"
        />
        <span v-else class="avatar-fallback">{{ initials }}</span>
      </span>
    </button>

    <div v-if="showMenu" class="menu" role="menu">
      <button class="menu-item" role="menuitem" type="button" @click="goToProfile">Профиль</button>
      <button class="menu-item" role="menuitem" type="button" @click="goToSettings">Настройки</button>
      <div class="menu-sep"></div>
      <button class="menu-item danger" role="menuitem" type="button" @click="logout">Выйти</button>
    </div>
  </div>
</template>

<script setup>
  import { computed, onBeforeUnmount, onMounted, ref, watch } from 'vue';
  import { useRouter } from 'vue-router';
  import { useAuthStore } from '@/features/auth/stores/authStore';
  
  const props = defineProps({
    userName: { type: String, required: true },
    avatarUrl: { type: String, default: '' },
  })
  
  const router = useRouter();
  const authStore = useAuthStore();
  
  const showMenu = ref(false);
  const root = ref(null);
  
  const avatarError = ref(false)

  const resolvedAvatar = computed(() => props.avatarUrl?.trim() || '')

  watch(resolvedAvatar, () => (avatarError.value = false))
  
  const initials = computed(() => {
    const s = (props.userName || '').trim()
    if (!s) return 'U'
    const parts = s.split(/\s+/)
    const first = parts[0]?.[0] ?? 'U'
    const second = parts.length > 1 ? parts[1]?.[0] : ''
    return (first + second).toUpperCase()
  })

  // простой “стабильный” цвет из имени
  const avatarStyle = computed(() => {
    let hash = 0
    for (let i = 0; i < props.userName.length; i++) hash = (hash * 31 + props.userName.charCodeAt(i)) | 0
    const hue = Math.abs(hash) % 360
    return { background: `hsl(${hue} 70% 45%)` }
  })

  const toggleMenu = () => {
    showMenu.value = !showMenu.value;
  };
  
  const closeMenu = () => {
    showMenu.value = false
  }
  
  // Закрываем меню при клике вне его
  const onDocClick = (e) => {
    if (!root.value) return
    if (!root.value.contains(e.target)) closeMenu()
  }
  
  onMounted(() => document.addEventListener('click', onDocClick))
  onBeforeUnmount(() => document.removeEventListener('click', onDocClick))
  
  const goToProfile = () => {
    closeMenu();
    router.push('/me');
  };
  
  const goToSettings = () => {
    closeMenu();
    router.push('/settings');
  };
  
  const logout = () => {
    closeMenu();
    authStore.logout();
    router.push('/logout');
  };
</script>

<style scoped>

  .user-menu {
    position: relative;
    display: inline-flex;
  }

  .menu {
    position: absolute;
    top: calc(100% + 8px);
    right: 0;

    z-index: 10000;
    background: var(--menu-bg);
    border: 1px solid var(--border-color);
    border-radius: 12px;
    box-shadow: 0 10px 30px rgba(0,0,0,0.12);
    padding: 6px;
    min-width: 180px;
  }

  .user-btn {
    display: inline-flex;
    align-items: center;
    gap: 10px;

    background: transparent;
    border: 0;
    padding: 6px 10px;
    border-radius: 10px;

    cursor: pointer;
    user-select: none;

    color: var(--text-primary);
  }

  .user-btn:hover {
    background: rgba(0, 0, 0, 0.06);
  }

  .user-name {
    font-size: 14px;
    line-height: 1;
    white-space: nowrap;
    color: inherit;
  }

  .avatar {
    width: 34px;
    height: 34px;
    border-radius: 999px;
    overflow: hidden;

    display: inline-flex;
    align-items: center;
    justify-content: center;

    color: white;
    font-weight: 700;
    font-size: 12px;
  }

  .avatar img {
    width: 100%;
    height: 100%;
    object-fit: cover;
    display: block;
  }

  .avatar-fallback {
    line-height: 1;
  }

  

  .menu-item {
    width: 100%;
    text-align: left;

    background: transparent;
    border: 0;
    padding: 10px 12px;

    border-radius: 10px;
    cursor: pointer;

    color: var(--menu-text);
    font-size: 14px;
  }

  .menu-item:hover {
    background: var(--menu-hover);
  }

  .menu-sep {
    height: 1px;
    margin: 6px 4px;
    background: rgba(0,0,0,0.08);
  }

  .menu-item.danger {
    color: #b00020;
  }

</style>