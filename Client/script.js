const { createApp } = Vue;

createApp({
  data() {
    return {
      newTask: '',
      tasks: JSON.parse(localStorage.getItem('tasks')) || [],
      filter: 'all', // 'all', 'active', 'completed'
      isDark: false
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
      localStorage.setItem('isDark', this.isDark);
    }
  },
  mounted() {
    const savedFilter = localStorage.getItem('filter');
    if (savedFilter) this.filter = savedFilter;

    const savedTheme = localStorage.getItem('isDark');
    if (savedTheme) this.isDark = savedTheme === 'true';
  }
}).mount('#app');