
    const { createApp } = Vue;

    createApp({
      data() {
        return {
          newTask: '',
          tasks: JSON.parse(localStorage.getItem('tasks')) || []
        };
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
          this.saveTasks();
        },
        removeTask(id) {
          this.tasks = this.tasks.filter(t => t.id !== id);
          this.saveTasks();
        },
        saveTasks() {
          localStorage.setItem('tasks', JSON.stringify(this.tasks));
        }
      }
    }).mount('#app');
