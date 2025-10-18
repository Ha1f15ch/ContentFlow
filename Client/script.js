let isLoggedIn = false;
let pendingUser = null; // Временное хранение данных пользователя до подтверждения

function openModal(mode) {
  const modal = document.getElementById('auth-modal');
  const title = document.getElementById('modal-title');
  const form = document.getElementById('auth-form');
  const errorMessage = document.getElementById('error-message');

  // Сброс формы и сообщений об ошибках
  if (errorMessage) {
    errorMessage.classList.add('hidden');
    errorMessage.innerHTML = '';
  }

  // Очистка классов ошибок
  document.querySelectorAll('input').forEach(input => {
    if (input && input.classList) {
      input.classList.remove('error');
    }
  });

  if (mode === 'login') {
    title.textContent = 'Вход';
    form.innerHTML = `
      <input type="text" id="username-input" placeholder="Имя пользователя или email" required>
      <input type="password" id="password-input" placeholder="Пароль" required>
      <div id="error-message" class="error-message hidden">Ошибка</div>
      <button type="submit" class="btn">Отправить</button>
      <button type="button" class="btn" onclick="closeModal()">Закрыть</button>
    `;
  } else if (mode === 'register') {
    title.textContent = 'Регистрация';
    form.innerHTML = `
      <div class="input-group">
        <input type="text" id="reg-name" placeholder="Ваше имя (4-32 символа)" required>
        <span class="error-text" id="name-error"></span>
      </div>
      <div class="input-group">
        <input type="email" id="reg-email" placeholder="Email (@gmail.com, @mail.ru, @yandex.ru)" required>
        <span class="error-text" id="email-error"></span>
      </div>
      <div class="input-group">
        <input type="password" id="reg-password" placeholder="Пароль (мин. 6 символов)" required>
        <span class="error-text" id="password-error"></span>
      </div>
      <div id="error-message" class="error-message hidden">Ошибка</div>
      <button type="submit" class="btn">Зарегистрироваться</button>
      <button type="button" class="btn" onclick="closeModal()">Закрыть</button>
    `;
  }

  form.onsubmit = function(e) {
    e.preventDefault();

    if (mode === 'login') {
      const usernameOrEmail = document.getElementById('username-input')?.value || '';
      const password = document.getElementById('password-input')?.value || '';
      login(usernameOrEmail, password);
    } else if (mode === 'register') {
      const name = document.getElementById('reg-name')?.value?.trim() || '';
      const email = document.getElementById('reg-email')?.value?.trim() || '';
      const password = document.getElementById('reg-password')?.value || '';

      validateAndRegister(name, email, password);
    }
  };

  if (modal) {
    modal.style.display = 'flex';
  }
}

function validateAndRegister(name, email, password) {
  const errors = [];

  // Валидация имени
  if (name.length < 4) {
    errors.push('Имя должно содержать не менее 4 символов');
  } else if (name.length > 32) {
    errors.push('Имя не может превышать 32 символа');
  }

  // Валидация почты
  const validDomains = ['@gmail.com', '@mail.ru', '@yandex.ru'];
  const isValidEmail = validDomains.some(domain => email.endsWith(domain));

  if (!email.includes('@')) {
    errors.push('Адрес электронной почты должен содержать символ "@"');
  } else if (!isValidEmail) {
    errors.push('Почта должна заканчиваться на @gmail.com, @mail.ru или @yandex.ru');
  }

  // Валидация пароля
  if (password.length < 6) {
    errors.push('Пароль должен быть не менее 6 символов');
  }

  // Проверка уникальности
  let users = [];
  try {
    users = JSON.parse(localStorage.getItem('users')) || [];
  } catch (e) {
    users = [];
  }

  if (users.some(u => u.name === name)) {
    errors.push('Пользователь с таким именем уже существует');
  }

  if (users.some(u => u.email === email)) {
    errors.push('Пользователь с такой почтой уже зарегистрирован');
  }

  // Если есть ошибки — показываем их
  if (errors.length > 0) {
    showError(errors.join('<br>'));
    return;
  }

  // Генерация кода и сохранение данных пользователя
  const code = generateCode();
  pendingUser = { name, email, password, code };

  // "Отправка" письма
  alert(`Код подтверждения отправлен на ${email}. Код: ${code}`);

  closeModal();
  openConfirmModal();
}

function generateCode() {
  return Math.floor(100000 + Math.random() * 900000).toString(); // 6-значный код
}

function openConfirmModal() {
  const modal = document.getElementById('confirm-modal');
  if (modal) {
    modal.style.display = 'flex';
  }
}

function closeConfirmModal() {
  const modal = document.getElementById('confirm-modal');
  if (modal) {
    modal.style.display = 'none';
  }
}

function confirmRegistration() {
  const inputCode = document.getElementById('confirm-code-input')?.value || '';
  if (pendingUser && inputCode === pendingUser.code) {
    // Сохраняем пользователя
    let users = [];
    try {
      users = JSON.parse(localStorage.getItem('users')) || [];
    } catch (e) {
      users = [];
    }
    users.push({ name: pendingUser.name, email: pendingUser.email, password: pendingUser.password });
    localStorage.setItem('users', JSON.stringify(users));

    alert('Регистрация завершена!');
    closeConfirmModal();
    pendingUser = null;
  } else {
    const errorDiv = document.getElementById('confirm-error-message');
    if (errorDiv) {
      errorDiv.textContent = 'Неверный код подтверждения';
      errorDiv.classList.remove('hidden');
    }
  }
}

function login(usernameOrEmail, password) {
  let users = [];
  try {
    users = JSON.parse(localStorage.getItem('users')) || [];
  } catch (e) {
    users = [];
  }

  const user = users.find(u => (u.name === usernameOrEmail || u.email === usernameOrEmail) && u.password === password);

  if (user) {
    isLoggedIn = true;
    const usernameSpan = document.getElementById('username');
    const greetingSpan = document.getElementById('greeting-user');
    const guestActions = document.getElementById('guest-actions');
    const userActions = document.getElementById('user-actions');
    const welcomeMessage = document.getElementById('welcome-message');
    const protectedContent = document.getElementById('protected-content');

    if (usernameSpan) usernameSpan.textContent = user.name;
    if (greetingSpan) greetingSpan.textContent = user.name;
    if (guestActions) guestActions.classList.add('hidden');
    if (userActions) userActions.classList.remove('hidden');
    if (welcomeMessage) welcomeMessage.textContent = `Привет, ${user.name}!`;
    if (protectedContent) protectedContent.classList.remove('hidden');

    closeModal();
  } else {
    showError('Неправильное имя/почта или пароль');
  }
}

function showError(message) {
  const errorDiv = document.getElementById('error-message');
  if (errorDiv) {
    errorDiv.innerHTML = message;
    errorDiv.classList.remove('hidden');
  }

  // Очистка предыдущих ошибок
  document.querySelectorAll('.error-text').forEach(el => {
    if (el) el.textContent = '';
  });

  document.querySelectorAll('input').forEach(input => {
    if (input && input.classList) {
      input.classList.remove('error');
    }
  });

  // Установка ошибок под полями
  if (message.includes('Имя')) {
    const nameError = document.getElementById('name-error');
    const regName = document.getElementById('reg-name');
    if (nameError) {
      if (message.includes('не менее 4')) {
        nameError.textContent = 'Имя слишком короткое';
      } else if (message.includes('не может превышать')) {
        nameError.textContent = 'Имя слишком длинное';
      }
    }
    if (regName) regName.classList.add('error');
  }
  if (message.includes('Адрес электронной почты')) {
    const emailError = document.getElementById('email-error');
    const regEmail = document.getElementById('reg-email');
    if (emailError) {
      emailError.textContent = 'Отсутствует символ "@"';
    }
    if (regEmail) regEmail.classList.add('error');
  } else if (message.includes('Почта должна заканчиваться')) {
    const emailError = document.getElementById('email-error');
    const regEmail = document.getElementById('reg-email');
    if (emailError) {
      emailError.textContent = 'Недопустимый домен';
    }
    if (regEmail) regEmail.classList.add('error');
  }
  if (message.includes('Пароль')) {
    const passwordError = document.getElementById('password-error');
    const regPassword = document.getElementById('reg-password');
    if (passwordError) {
      passwordError.textContent = 'Пароль слишком короткий';
    }
    if (regPassword) regPassword.classList.add('error');
  }
}

// Переключение темы и прочие инициализации
document.addEventListener('DOMContentLoaded', () => {
  const themeToggle = document.getElementById('theme-toggle-checkbox');
  const body = document.body;

  // Проверяем сохранённую тему
  if (localStorage.getItem('theme') === 'dark') {
    body.classList.add('dark-theme');
    if (themeToggle) themeToggle.checked = true;
  }

  if (themeToggle) {
    themeToggle.addEventListener('change', () => {
      body.classList.toggle('dark-theme', themeToggle.checked);
      localStorage.setItem('theme', themeToggle.checked ? 'dark' : 'light');
    });
  }

  // Закрытие модального окна по Esc
  document.addEventListener('keydown', (e) => {
    if (e.key === 'Escape') {
      closeModal();
      closeConfirmModal();
    }
  });
});

function closeModal() {
  const modal = document.getElementById('auth-modal');
  if (modal) {
    modal.style.display = 'none';
    // Очистка всех ошибок
    const errorMessage = document.getElementById('error-message');
    if (errorMessage) {
      errorMessage.classList.add('hidden');
      errorMessage.innerHTML = '';
    }
    // Очистка классов ошибок
    document.querySelectorAll('input').forEach(input => {
      if (input && input.classList) {
        input.classList.remove('error');
      }
    });
    // Очистка полей (по желанию)
    const usernameInput = document.getElementById('username-input');
    const passwordInput = document.getElementById('password-input');
    if (usernameInput) usernameInput.value = '';
    if (passwordInput) passwordInput.value = '';
  }
}

function logout() {
  isLoggedIn = false;
  const guestActions = document.getElementById('guest-actions');
  const userActions = document.getElementById('user-actions');
  const welcomeMessage = document.getElementById('welcome-message');
  const protectedContent = document.getElementById('protected-content');

  if (guestActions) guestActions.classList.remove('hidden');
  if (userActions) userActions.classList.add('hidden');
  if (welcomeMessage) welcomeMessage.textContent = 'Пожалуйста, войдите, чтобы получить доступ к контенту.';
  if (protectedContent) protectedContent.classList.add('hidden');
}