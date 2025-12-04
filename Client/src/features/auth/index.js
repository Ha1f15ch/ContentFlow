// src/router/index.js
import { createRouter, createWebHistory } from 'vue-router';
import HomeView from '@/views/HomeView.vue'; // главная страница
import LoginView from '@/views/Login.vue';
import RegisterView from '@/views/Register.vue';
import ConfirmEmailView from '@/views/ConfirmEmail.vue';

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
];

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes,
});

export default router;