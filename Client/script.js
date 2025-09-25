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
      editedTaskText: '',
      editedTaskPriority: 'medium',
      editedTaskStatus: 'planned',
      // Состояние модального окна добавления
      isNewTaskModalOpen: false,
      newTaskData: {
        text: '',
        priority: 'medium',
        status: 'planned',
      },
    };
  },
  computed: {
    filteredTasks() {
      if (this.filter === 'active') {
        return this.tasks.filter((t) => !t.completed);
      } else if (this.filter === 'completed') {
        return this.tasks.filter((t) => t.completed);
      }
      return this.tasks;
    },
  },
  methods: {
    addTask() {
      if (this.newTask.trim().length < 2) return;
      this.tasks.push({
        id: Date.now(),
        text: this.newTask.trim(),
        completed: false,
        priority: 'medium',
        status: 'planned',
      });
      this.newTask = '';
      this.saveState();
    },
    removeTask(id) {
      this.tasks = this.tasks.filter((t) => t.id !== id);
      this.saveState();
    },
    saveState() {
      localStorage.setItem('tasks', JSON.stringify(this.tasks));
      localStorage.setItem('filter', this.filter);
      localStorage.setItem('isDark', this.isDark.toString());
    },
    // --- Методы для модального окна редактирования ---
    openEditModal(task) {
      this.taskToEdit = task;
      this.editedTaskText = task.text;
      this.editedTaskPriority = task.priority;
      this.editedTaskStatus = task.status;
      this.isEditModalOpen = true;
    },
    closeEditModal() {
      this.isEditModalOpen = false;
      this.taskToEdit = null;
      this.editedTaskText = '';
      this.editedTaskPriority = 'medium';
      this.editedTaskStatus = 'planned';
    },
    saveEditedTask() {
      if (this.taskToEdit) {
        this.taskToEdit.text = this.editedTaskText;
        this.taskToEdit.priority = this.editedTaskPriority;
        this.taskToEdit.status = this.editedTaskStatus;
        // Синхронизируем старое поле completed для совместимости с фильтрами
        this.taskToEdit.completed = this.editedTaskStatus === 'done';
        this.saveState();
      }
      this.closeEditModal();
    },
    // --- Методы для модального окна добавления ---
    openNewTaskModal() {
      this.newTaskData = {
        text: '',
        priority: 'medium',
        status: 'planned',
      };
      this.isNewTaskModalOpen = true;
    },
    closeNewTaskModal() {
      this.isNewTaskModalOpen = false;
    },
    saveNewTask() {
      if (this.newTaskData.text.trim().length < 2) {
        alert('Текст задачи должен быть не менее 2 символов');
        return;
      }
      this.tasks.push({
        id: Date.now(),
        text: this.newTaskData.text.trim(),
        completed: this.newTaskData.status === 'done',
        priority: this.newTaskData.priority,
        status: this.newTaskData.status,
      });
      this.saveState();
      this.closeNewTaskModal();
    },
    getPriorityLabel(priority) {
      const labels = { low: 'Низкий', medium: 'Средний', high: 'Высокий' };
      return labels[priority] || priority;
    },
    getStatusLabel(status) {
      const labels = {
        planned: 'Запланирована',
        'in-progress': 'В процессе',
        done: 'Выполнена',
      };
      return labels[status] || status;
    },
  },
  mounted() {
    const savedFilter = localStorage.getItem('filter');
    if (savedFilter) this.filter = savedFilter;

    const savedTheme = localStorage.getItem('isDark');
    if (savedTheme) this.isDark = savedTheme === 'true';

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
    },
  },
}).mount('#app');