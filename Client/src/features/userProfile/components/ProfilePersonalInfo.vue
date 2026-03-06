<template>
  <div v-if="profile" class="personal-info">
    <div class="info-grid">
      <div class="info-item">
        <div class="label">Имя пользователя</div>
        <div class="value">{{ profile.userName || "—" }}</div>
      </div>

      <div class="info-item">
        <div class="label">Имя</div>
        <div class="value">{{ profile.firstName || "—" }}</div>
      </div>

      <div class="info-item">
        <div class="label">Фамилия</div>
        <div class="value">{{ profile.lastName || "—" }}</div>
      </div>

      <div class="info-item">
        <div class="label">Отчество</div>
        <div class="value">{{ profile.middleName || "—" }}</div>
      </div>

      <div class="info-item">
        <div class="label">Дата рождения</div>
        <div class="value">{{ formatBirthDate(profile.birthDate) }}</div>
      </div>

      <div class="info-item">
        <div class="label">Возраст</div>
        <div class="value">{{ profile.age ?? "—" }}</div>
      </div>

      <div class="info-item">
        <div class="label">Город</div>
        <div class="value">{{ profile.city || "—" }}</div>
      </div>

      <div class="info-item">
        <div class="label">Пол</div>
        <div class="value">{{ profile.gender || "—" }}</div>
      </div>

      <div class="info-item wide">
        <div class="label">О себе</div>
        <div class="value">{{ profile.bio || "—" }}</div>
      </div>

      <div class="info-item">
        <div class="label">Профиль создан</div>
        <div class="value">{{ formatDateTime(profile.createdAt) }}</div>
      </div>

      <div class="info-item">
        <div class="label">Последнее обновление</div>
        <div class="value">{{ formatDateTime(profile.updatedAt) }}</div>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
    profile: {
        type: Object,
        required: null,
    },
});

function formatBirthDate(value) {
  if (!value) return "—";
  return value;
}

function formatDateTime(value) {
  if (!value) return "—";

  const date = new Date(value);
  if (Number.isNaN(date.getTime())) return value;

  return date.toLocaleString("ru-RU");
}
</script>

<style scoped>
.info-grid {
  display: grid;
  grid-template-columns: repeat(2, minmax(220px, 1fr));
  gap: 16px;
}

.info-item {
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid var(--border-color, #333);
  border-radius: 12px;
  padding: 14px;
}

.info-item.wide {
  grid-column: 1 / -1;
}

.label {
  font-size: 13px;
  opacity: 0.7;
  margin-bottom: 6px;
}

.value {
  font-size: 15px;
  line-height: 1.4;
}
</style>