import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import router from './router'
import { useAuthStore } from '@/features/auth/stores/authStore'
import { bindAuthStore } from '@/shared/api/HttpClient'

async function bootstrapApp() {
  const app = createApp(App)
  const pinia = createPinia()

  app.use(pinia)
  app.use(router)

  const authStore = useAuthStore()
  bindAuthStore(authStore)
  await authStore.initSession()

  app.mount('#app')
}

bootstrapApp()
