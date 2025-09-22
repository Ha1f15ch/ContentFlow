const { createApp } = Vue;

createApp({
  data() {
    return {
      tasks: [],
      loading: true,
      error: null
    }
  },

  async mounted() {
    try {
      const response = await fetch('./indesx_object.json');
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      this.tasks = await response.json();
    } catch (err) {
      console.error("Не удалось загрузить задачи:", err);
      this.error = "Ошибка загрузки данных. Проверьте консоль.";
    } finally {
      this.loading = false;
    }
  },

  template: `
    <div>
      <h2>Список задач</h2>

      <div v-if="loading">Загрузка задач, пожалуйста, подождите...</div>

      <div v-else-if="error" style="color: red;">
        {{ error }}
      </div>

      <div v-else>
        <div v-for="task in tasks" :key="task.id" style="border: 1px solid #ccc; margin: 10px; padding: 10px; border-radius: 5px;">
          <h4>{{ task.title }}</h4>
          <p><strong>Описание:</strong> {{ task.description }}</p>
          <p><strong>Статус:</strong> {{ task.status }} | <strong>Приоритет:</strong> {{ task.priority }}</p>
        </div>
      </div>
    </div>
  `
}).mount('#app');