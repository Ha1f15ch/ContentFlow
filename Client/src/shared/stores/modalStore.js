import { ref } from 'vue'
import { defineStore } from 'pinia'

export const useModalStore = defineStore('modal', () => {
  // Тип текущего модального окна
  const currentModal = ref(null) // например: 'login', 'confirmEmail', null

  // Данные, связанные с модальным окном
  const modalData = ref({})

  // Открыть модалку с данными
  const openModal = (type, data = {}) => {
    currentModal.value = type
    modalData.value = data
  }

  // Закрыть все модалки
  const closeModal = () => {
    currentModal.value = null
    modalData.value = {}
  }

  // Здесь экспортируем описанные нижу функции, как цельый файл - modalStore.js
  return {
    currentModal,
    modalData,
    openModal,
    closeModal,
  }
})