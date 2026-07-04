export const REPORT_REASONS = [
  { value: 0, label: "Спам" },
  { value: 1, label: "Оскорбления / травля" },
  { value: 2, label: "Язык вражды" },
  { value: 3, label: "NSFW / неприемлемый контент" },
  { value: 4, label: "Ложная информация" },
  { value: 5, label: "Другое" },
];

export function getReportReasonLabel(value) {
  return REPORT_REASONS.find((reason) => reason.value === value)?.label ?? "Неизвестно";
}
