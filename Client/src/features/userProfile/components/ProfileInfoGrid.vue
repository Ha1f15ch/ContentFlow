<template>
  <div class="info-grid">
    <div
      v-for="item in items"
      :key="item.label"
      class="info-item"
    >
      <div class="label">{{ item.label }}</div>
      <div class="value">{{ item.value }}</div>
    </div>
  </div>
</template>

<script setup>
import { computed } from "vue";

const props = defineProps({
  profile: {
    type: Object,
    required: true,
  },
});

const items = computed(() => [
  { label: "Имя", value: props.profile.firstName || "—" },
  { label: "Фамилия", value: props.profile.lastName || "—" },
  { label: "Отчество", value: props.profile.middleName || "—" },
  { label: "Город", value: props.profile.city || "—" },
  { label: "Возраст", value: props.profile.age ?? "—" },
  { label: "Пол", value: props.profile.gender || "—" },
]);
</script>

<style scoped>
.info-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(220px, 1fr));
  gap: 16px;
}

.info-item {
  background: var(--bg-secondary);
  border: 1px solid var(--border-color);
  border-radius: 12px;
  padding: 14px;
}

.label {
  font-size: 13px;
  color: var(--text-secondary);
  margin-bottom: 6px;
}

.value {
  font-size: 15px;
  line-height: 1.5;
  color: var(--text-primary);
}

@media (max-width: 768px) {
  .info-grid {
    grid-template-columns: 1fr;
  }
}
</style>
