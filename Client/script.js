const { createApp } = Vue;

createApp({
  data() {
    return {
      newTask: '',
      tasks: JSON.parse(localStorage.getItem('tasks')) || [],
      filter: 'all',
      isDark: false,
      // Новые данные для модального окна редактирования
      isEditModalOpen: false,
      taskToEdit: null,
      editedTaskText: ''
    };
  },
  computed: {
    filteredTasks() {
      if (this.filter === 'active') {
        return this.tasks.filter(t => !t.completed);
      } else if (this.filter === 'completed') {
        return this.tasks.filter(t => t.completed);
      }
      return this.tasks;
    }
  },
  methods: {
    addTask() {
      if (this.newTask.trim().length < 2) return;
      this.tasks.push({
        id: Date.now(),
        text: this.newTask.trim(),
        completed: false
      });
      this.newTask = '';
      this.saveState();
    },
    removeTask(id) {
      this.tasks = this.tasks.filter(t => t.id !== id);
      this.saveState();
    },
    saveState() {
      localStorage.setItem('tasks', JSON.stringify(this.tasks));
      localStorage.setItem('filter', this.filter);
      localStorage.setItem('isDark', this.isDark.toString()); // Сохраняем как строку
    },
    // Открытие модального окна редактирования
    openEditModal(task) {
      this.taskToEdit = task;
      this.editedTaskText = task.text;
      this.isEditModalOpen = true;
    },

    // Закрытие модального окна
    closeEditModal() {
      this.isEditModalOpen = false;
      this.taskToEdit = null;
      this.editedTaskText = '';
    },

    // Сохранение изменений
    saveEditedTask() {
      if (this.taskToEdit) {
        this.taskToEdit.text = this.editedTaskText;
        this.saveState();
      }
      this.closeEditModal();
    }
  },
  mounted() {
    const savedFilter = localStorage.getItem('filter');
    if (savedFilter) this.filter = savedFilter;

    const savedTheme = localStorage.getItem('isDark');
    if (savedTheme) this.isDark = savedTheme === 'true';

    // Применяем тему при загрузке
    if (this.isDark) {
      document.body.classList.add('dark-theme');
    } else {
      document.body.classList.remove('dark-theme');
    }
  },
  watch: {
    isDark(newVal) {
      if (newVal) {
        document.body.classList.add('dark-theme');
      } else {
        document.body.classList.remove('dark-theme');
      }
      this.saveState();
    }
  }
}).mount('#app');