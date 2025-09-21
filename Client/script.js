console.log("script.js has been loaded and executed");

const { createApp } = Vue;

createApp({
  data() {
    // Создаем объект данных и логируем начальное состояние
    const initialData = {
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
      isNewTaskModalOpen: false, // Начальное значение false
      newTaskData: {
        text: '',
        priority: 'medium',
        status: 'planned'
      }
    };
    // Отладка 1: Проверяем начальное значение
    console.log('Initial isNewTaskModalOpen:', initialData.isNewTaskModalOpen);
    return initialData;
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
        completed: false,
        priority: 'medium',
        status: 'planned'
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
      localStorage.setItem('isDark', this.isDark.toString());
    },
    // --- Методы для модального окна редактирования ---
    openEditModal(task) {
      console.log('openEditModal called'); // Отладка
      this.taskToEdit = task;
      this.editedTaskText = task.text;
      this.editedTaskPriority = task.priority;
      this.editedTaskStatus = task.status;
      this.isEditModalOpen = true;
      console.log('isEditModalOpen after open:', this.isEditModalOpen); // Отладка
    },
    closeEditModal() {
      console.log('closeEditModal called'); // Отладка
      this.isEditModalOpen = false;
      this.taskToEdit = null;
      this.editedTaskText = '';
      this.editedTaskPriority = 'medium';
      this.editedTaskStatus = 'planned';
      console.log('isEditModalOpen after close:', this.isEditModalOpen); // Отладка
    },
    saveEditedTask() {
      if (this.taskToEdit) {
        this.taskToEdit.text = this.editedTaskText;
        this.taskToEdit.priority = this.editedTaskPriority;
        this.taskToEdit.status = this.editedTaskStatus;
        this.taskToEdit.completed = this.editedTaskStatus === 'done';
        this.saveState();
      }
      this.closeEditModal();
    },
    // --- Методы для модального окна добавления ---
    openNewTaskModal() {
      console.log('openNewTaskModal called'); // Отладка
      this.newTaskData = {
        text: '',
        priority: 'medium',
        status: 'planned'
      };
      this.isNewTaskModalOpen = true;
      console.log('isNewTaskModalOpen after open:', this.isNewTaskModalOpen); // Отладка
    },
    closeNewTaskModal() {
      console.log('closeNewTaskModal called'); // Отладка
      this.isNewTaskModalOpen = false;
      console.log('isNewTaskModalOpen after close:', this.isNewTaskModalOpen); // Отладка
    },
    saveNewTask() {
      console.log('saveNewTask called'); // Отладка
      if (this.newTaskData.text.trim().length < 2) {
        alert('Текст задачи должен быть не менее 2 символов');
        return;
      }
      this.tasks.push({
        id: Date.now(),
        text: this.newTaskData.text.trim(),
        completed: this.newTaskData.status === 'done',
        priority: this.newTaskData.priority,
        status: this.newTaskData.status
      });
      this.saveState();
      console.log('Task added, about to close modal'); // Отладка
      this.closeNewTaskModal();
      console.log('Modal closed after save'); // Отладка
    },
    getPriorityLabel(priority) {
      const labels = { low: 'Низкий', medium: 'Средний', high: 'Высокий' };
      return labels[priority] || priority;
    },
    getStatusLabel(status) {
      const labels = { planned: 'Запланирована', 'in-progress': 'В процессе', done: 'Выполнена' };
      return labels[status] || status;
    }
  },
  mounted() {
    // Отладка 2: Проверяем значение после монтирования
    console.log('Mounted hook start, isNewTaskModalOpen:', this.isNewTaskModalOpen);

    const savedFilter = localStorage.getItem('filter');
    if (savedFilter) this.filter = savedFilter;

    const savedTheme = localStorage.getItem('isDark');
    if (savedTheme) this.isDark = savedTheme === 'true';

    if (this.isDark) {
      document.body.classList.add('dark-theme');
    } else {
      document.body.classList.remove('dark-theme');
    }
    // Отладка 3: Проверяем значение в конце mounted
    console.log('Mounted hook end, isNewTaskModalOpen:', this.isNewTaskModalOpen);
  },
  // --- ОБНОВЛЕННАЯ СЕКЦИЯ watch ---
  watch: {
    // Наблюдатель за нашей "подозрительной" переменной
    isNewTaskModalOpen: {
      handler(newVal, oldVal) {
        console.log(`[Watcher] isNewTaskModalOpen changed from ${oldVal} to ${newVal}`);
        // Выведем стек вызовов, чтобы понять, кто это изменил
        if (newVal === true) {
          console.trace('[Watcher] isNewTaskModalOpen became TRUE');
        }
      },
      immediate: true // Вызовет handler сразу при создании, даже если значение не менялось
    },
    isDark(newVal) {
      if (newVal) {
        document.body.classList.add('dark-theme');
      } else {
        document.body.classList.remove('dark-theme');
      }
      this.saveState();
    }
  }
  // --- КОНЕЦ ОБНОВЛЕННОЙ СЕКЦИИ watch ---
}).mount('#app');
console.log("Vue app mounted to #app");