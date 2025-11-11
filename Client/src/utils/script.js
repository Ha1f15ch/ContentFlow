export const generateRandomCode = () => {
  return Math.floor(100000 + Math.random() * 900000).toString();
};

/**
 * Валидация email по разрешённым доменам
 */
export const isValidEmail = (email) => {
  const validDomains = ['@gmail.com', '@mail.ru', '@yandex.ru'];
  return validDomains.some(domain => email.endsWith(domain));
};

/**
 * Проверка длины строки
 */
export const isLengthValid = (str, min = 0, max = Infinity) => {
  return str.length >= min && str.length <= max;
};

/**
 * Проверка, является ли строка допустимым именем (4-32 символа)
 */
export const isValidName = (name) => {
  return isLengthValid(name.trim(), 4, 32);
};

/**
 * Проверка, является ли строка допустимым паролем (мин. 6 символов)
 */
export const isValidPassword = (password) => {
  return password.length >= 6;
};

/**
 * Очистка всех классов ошибок у элементов
 */
export const clearInputErrors = (selector = 'input') => {
  const inputs = document.querySelectorAll(selector);
  inputs.forEach(input => {
    if (input.classList) {
      input.classList.remove('error');
    }
  });
};

/**
 * Показ сообщения об ошибке в элементе
 */
export const showError = (elementId, message) => {
  const element = document.getElementById(elementId);
  if (element) {
    element.textContent = message;
    element.classList.remove('hidden');
  }
};

/**
 * Скрытие сообщения об ошибке
 */
export const hideError = (elementId) => {
  const element = document.getElementById(elementId);
  if (element) {
    element.classList.add('hidden');
    element.textContent = '';
  }
};

/**
 * Очистка формы по ID
 */
export const clearForm = (formId) => {
  const form = document.getElementById(formId);
  if (form) {
    form.reset();
  }
};