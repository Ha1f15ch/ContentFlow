// Подключаем Vue 3 (через CDN)
const { createApp } = Vue;

// --- СОЗДАНИЕ ПРИЛОЖЕНИЯ ---
createApp({
  // --- ДАННЫЕ (data) ---
  // Здесь хранятся все реактивные переменные приложения
  data() {
    return {
      // Список всех задач, загружается из localStorage
      tasks: JSON.parse(localStorage.getItem('tasks')) || [],
      
      // Текущий фильтр: 'all', 'active', 'completed'
      filter: 'all',
      
      // Тема: true = тёмная, false = светлая
      isDark: false,

      // --- Состояние модального окна добавления задачи ---
      isNewTaskModalOpen: false, // Открыто ли окно
      newTaskData: {
        title: '',      // Заголовок новой задачи
        text: '',       // Описание новой задачи
        priority: 'medium', // Приоритет по умолчанию
        status: 'planned'   // Статус по умолчанию
      },

      // --- Состояние модального окна редактирования задачи ---
      isEditModalOpen: false,   // Открыто ли окно
      taskToEdit: null,         // Ссылка на редактируемую задачу
      editedTaskTitle: '',      // Временное значение заголовка
      editedTaskText: '',       // Временное значение описания
      editedTaskPriority: 'medium', // Временное значение приоритета
      editedTaskStatus: 'planned'   // Временное значение статуса
    };
  },

  // --- ВЫЧИСЛЯЕМЫЕ СВОЙСТВА (computed) ---
  // Автоматически пересчитываются при изменении зависимостей
  computed: {
    // Фильтрует задачи в зависимости от this.filter
    filteredTasks() {
      if (this.filter === 'active') {
        // Возвращает только НЕ выполненные задачи
        return this.tasks.filter(t => !t.completed);
      } else if (this.filter === 'completed') {
        // Возвращает только выполненные задачи
        return this.tasks.filter(t => t.completed);
      }
      // Если фильтр 'all' — возвращает все задачи
      return this.tasks;
    }
  },

  // --- МЕТОДЫ (methods) ---
  // Функции, которые можно вызывать в шаблоне и из кода
  methods: {
    // Удаление задачи по ID
    removeTask(id) {
      this.tasks = this.tasks.filter(t => t.id !== id);
      this.saveState(); // Сохраняем изменения
    },

    // Сохранение всех данных в localStorage
    saveState() {
      localStorage.setItem('tasks', JSON.stringify(this.tasks));
      localStorage.setItem('filter', this.filter);
      localStorage.setItem('isDark', this.isDark.toString());
    },

    // --- Методы для модального окна добавления задачи ---
    openNewTaskModal() {
      // Сбрасываем данные формы перед открытием
      this.newTaskData = {
        title: '',
        text: '',
        priority: 'medium',
        status: 'planned'
      };
      this.isNewTaskModalOpen = true; // Открываем модалку
    },
    closeNewTaskModal() {
      this.isNewTaskModalOpen = false; // Закрываем модалку
    },
    saveNewTask() {
      // Проверка: заголовок обязателен
      if (!this.newTaskData.title.trim()) {
        alert('Введите заголовок задачи');
        return;
      }
      // Добавляем новую задачу в список
      this.tasks.push({
        id: Date.now(),
        title: this.newTaskData.title.trim(),
        text: this.newTaskData.text.trim(),
        completed: this.newTaskData.status === 'done',
        priority: this.newTaskData.priority,
        status: this.newTaskData.status
      });
      this.saveState(); // Сохраняем изменения
      this.closeNewTaskModal(); // Закрываем модалку
    },

    // --- Методы для модального окна редактирования задачи ---
    openEditModal(task) {
      // Сохраняем ссылку на редактируемую задачу
      this.taskToEdit = task;
      // Копируем данные задачи во временные поля
      this.editedTaskTitle = task.title;
      this.editedTaskText = task.text;
      this.editedTaskPriority = task.priority;
      this.editedTaskStatus = task.status;
      this.isEditModalOpen = true; // Открываем модалку
    },
    closeEditModal() {
      this.isEditModalOpen = false; // Закрываем модалку
      this.taskToEdit = null;       // Очищаем ссылку
      // Сбрасываем временные поля
      this.editedTaskTitle = '';
      this.editedTaskText = '';
      this.editedTaskPriority = 'medium';
      this.editedTaskStatus = 'planned';
    },
    saveEditedTask() {
      if (this.taskToEdit) {
        // Обновляем оригинальную задачу
        this.taskToEdit.title = this.editedTaskTitle.trim();
        this.taskToEdit.text = this.editedTaskText.trim();
        this.taskToEdit.priority = this.editedTaskPriority;
        this.taskToEdit.status = this.editedTaskStatus;
        // Синхронизируем поле completed для фильтров
        this.taskToEdit.completed = this.editedTaskStatus === 'done';
        this.saveState(); // Сохраняем изменения
      }
      this.closeEditModal(); // Закрываем модалку
    },

    // Возвращает метку приоритета
    getPriorityLabel(priority) {
      const labels = { low: 'Низкий', medium: 'Средний', high: 'Высокий' };
      return labels[priority] || priority;
    },
    // Возвращает метку статуса
    getStatusLabel(status) {
      const labels = { planned: 'Запланирована', 'in-progress': 'В процессе', done: 'Выполнена' };
      return labels[status] || status;
    }
  },

  // Выполняются в определённый момент жизни приложения (УКИ ЖИЗНЕННОГО ЦИКЛА - mounted, created и т.д.) ---
  mounted() {
    // Загружаем сохранённые настройки пользователя
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

  // --- НАБЛЮДАТЕЛИ (watch) --- мб потом их удалить, хз
  // Следят за изменением конкретных переменных
  watch: {
    // Следит за изменением темы
    isDark(newVal) {
      if (newVal) {
        document.body.classList.add('dark-theme'); // Включаем тёмную тему
      } else {
        document.body.classList.remove('dark-theme'); // Выключаем тёмную тему
      }
      this.saveState(); // Сохраняем настройку темы
    }
  }
}).mount('#app'); // Монтируем приложение к элементу с id="app" 