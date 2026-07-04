export const MODERATION_PRIORITY = {
  0: { label: "Низкий", className: "priority-low" },
  1: { label: "Средний", className: "priority-medium" },
  2: { label: "Высокий", className: "priority-high" },
  3: { label: "Критический", className: "priority-critical" },
};

export const MODERATION_STATUS = {
  0: "Открыт",
  1: "На проверке",
  2: "Решён",
  3: "Отклонён",
};

export const MODERATION_DECISIONS = [
  { value: 0, label: "Без действий" },
  { value: 1, label: "Скрыть контент" },
  { value: 2, label: "Удалить контент" },
  { value: 3, label: "Предупреждение автору" },
  { value: 4, label: "Временная блокировка автора" },
  { value: 5, label: "Постоянная блокировка автора" },
];

export function getPriorityMeta(priority) {
  return MODERATION_PRIORITY[priority] ?? MODERATION_PRIORITY[0];
}
