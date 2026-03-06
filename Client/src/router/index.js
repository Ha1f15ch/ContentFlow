// src/router/index.js
import { createRouter, createWebHistory } from 'vue-router';
import HomeView from '@/views/HomeView.vue';
import LoginView from '@/views/Login.vue';
import RegisterView from '@/views/Register.vue';
import ConfirmEmailView from '@/views/ConfirmEmail.vue';
import CreatePostView from '@/features/post/views/CreatePostView.vue';
import MyProfileView from '@/features/userProfile/views/MyProfileView.vue';

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
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

export default router;