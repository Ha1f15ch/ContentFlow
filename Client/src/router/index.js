// src/router/index.js
import { useAuthStore } from '@/features/auth/stores/authStore';
import { createRouter, createWebHistory } from 'vue-router';

import HomeView from '@/views/HomeView.vue';
import LoginView from '@/views/Login.vue';
import RegisterView from '@/views/Register.vue';
import ConfirmEmailView from '@/views/ConfirmEmail.vue';
import CreatePostView from '@/features/post/views/CreatePostView.vue';
import MyProfileView from '@/features/userProfile/views/MyProfileView.vue';
import UserProfileView from '@/features/userProfile/views/UserProfileView.vue';
import ModerationView from '@/features/moderation/views/ModerationView.vue';
import SettingsView from '@/features/settings/views/SettingsView.vue';

const routes = [
  {
    path: '/',
    name: 'home',
    component: HomeView,
  },
  {
    path: '/login',
    name: 'login',
    component: LoginView,
  },
  {
    path: '/register',
    name: 'register',
    component: RegisterView,
  },
  {
    path: '/confirm-email',
    name: 'confirmEmail',
    component: ConfirmEmailView,
  },

  {
    path: '/create-post',
    name: 'createPost',
    component: CreatePostView,
    meta: { requiresAuth: true } // Защита: только для авторизованных
  },
  {
    path: '/me',
    name: 'myProfile',
    component: MyProfileView,
    meta: { requiresAuth: true }
  },
  {
    path: '/profiles/:profileId',
    name: 'userProfile',
    component: UserProfileView,
    meta: { requiresAuth: true }
  },
  {
    path: '/settings',
    name: 'settings',
    component: SettingsView,
    meta: { requiresAuth: true }
  },
  {
    path: '/moderation',
    name: 'moderation',
    component: ModerationView,
    meta: { requiresAuth: true, requiresModerator: true }
  },
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

router.beforeEach(async (to, from, next) => {
  const authStore = useAuthStore();

  if (!authStore.sessionReady) {
    await authStore.initSession();
  }

  if (to.meta.requiresAuth && !authStore.isAuthenticated) {
    next('/login');
    return;
  }

  if (to.meta.requiresModerator && !authStore.canModerate) {
    next('/');
    return;
  }

  next();
});

export default router;