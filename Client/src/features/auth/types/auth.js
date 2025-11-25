// Регистрация
export const RegisterCommand = {
  email: '',
  password: '',
  firstName: '',
  lastName: '',
};

// Логин
export const LoginCommand = {
  email: '',
  password: '',
};

// Подтверждение email
export const ConfirmEmailCommand = {
  email: '',
  token: '',
};

// Повторная отправка подтверждения
export const ResendConfirmationCommand = {
  email: '',
};