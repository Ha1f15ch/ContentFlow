<template>
  <div v-if="profile" class="personal-info">
    <div class="section-header">
      <h2>Персональная информация</h2>
      
      <div class="actions">
        <button v-if="!isEditing"
          class="action-btn"
          type="button"
          @click = startEdit
          >
            Редактировать
        </button>
      
        <template v-else>
          <button 
            class="action-btn secondary"
            type="button"
            @click="cancelEdit"
            :disabled="saving"
          >
            Отменить
          </button>

          <button
            class="action-btn primary"
            type="button"
            @click="saveChanges"
            :disabled="!hasChanges || saving"
          >
            {{ saving ? "Сохранение..." : "Сохранить" }}
          </button>
        </template>
      </div>
    </div>

    <div v-if="saveError" class="save-error">
      {{ saveError }}
    </div>

    <div class="info-grid">
      <div class="info-item">
        <div class="label">Имя пользователя</div>
        <div class="value">{{ profile.userName || "—" }}</div>
      </div>
      <div class="info-item">
        <div class="label">Имя</div>
        <div v-if="!isEditing" class="value">
          {{ profile.firstName || "—" }}
        </div>
        <input
          v-else
          v-model="form.firstName"
          class="form-input"
          type="text"
          placeholder="Введите имя"
        />
      </div>
      <div class="info-item">
        <div class="label">Фамилия</div>
        <div v-if="!isEditing" class="value">{{ profile.lastName || "—" }}</div>
        <input
          v-else
          v-model="form.lastName"
          class="form-input"
          type="text"
          placeholder="Введите фамилию"
        />
      </div>
      <div class="info-item">
        <div class="label">Отчество</div>
        <div v-if="!isEditing" class="value">{{ profile.middleName || "—" }}</div>
        <input
          v-else
          v-model="form.middleName"
          class="form-input"
          type="text"
          placeholder="Введите отчество"
        />
      </div>
      <div class="info-item">
        <div class="label">Дата рождения</div>
        <div v-if="!isEditing" class="value">{{ formatBirthDate(profile.birthDate) }}</div>
        <input
          v-else
          v-model="form.birthDate"
          class="form-input"
          type="date"
        />
      </div>
      <div class="info-item">
        <div class="label">Возраст</div>
        <div class="value">{{ profile.age ?? "—" }}</div>
      </div>
      <div class="info-item">
        <div class="label">Город</div>
        <div v-if="!isEditing" class="value">{{ profile.city || "—" }}</div>
        <input
          v-else
          v-model="form.city"
          class="form-input"
          type="text"
          placeholder="Введите город"
        />
      </div>
      <div class="info-item">
        <div class="label">Пол</div>
        <div v-if="!isEditing" class="value">{{ profile.gender || "—" }}</div>
        <select v-else v-model="form.gender" class="form-input">
          <option value="Undefined">Не указан</option>
          <option value="Male">Мужской</option>
          <option value="Female">Женский</option>
        </select>
      </div>
      <div class="info-item wide">
        <div class="label">О себе</div>
        <div v-if="!isEditing" class="value">{{ profile.bio || "—" }}</div>
        <textarea
          v-else
          v-model="form.bio"
          class="form-input form-textarea"
          rows="5"
          placeholder="Расскажите о себе"
        />
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

  import { computed, reactive, ref, watch } from "vue";
  import { userProfileService } from "@/features/userProfile/api/userProfileService";

  const props = defineProps({
      profile: {
          type: Object,
          required: null,
          default: null,
      },
  });

  const emit = defineEmits(["updated"]);

  const isEditing = ref(false);
  const saving = ref(false);
  const saveError = ref("");

  const form = reactive({
    firstName: "",
    lastName: "",
    middleName: "",
    birthDate: "",
    city: "",
    bio: "",
    gender: "",
  });

  function normalizeString(value) {
    return value ?? "";
  }

  function fillFormFromProfile(profile) {
    form.firstName = normalizeString(profile?.firstName);
    form.lastName = normalizeString(profile?.lastName);
    form.middleName = normalizeString(profile?.middleName);
    form.birthDate = profile?.birthDate ?? "";
    form.city = normalizeString(profile?.city);
    form.bio = normalizeString(profile?.bio);
    form.gender = normalizeString(profile?.gender);
  }

  watch(
    () => props.profile,
    (newProfile) => {
      if(!newProfile) return;
      fillFormFromProfile(newProfile);
    },
    { immediate: true }
  );

  const hasChanges = computed(() => {
    if (!props.profile) return false;

    return (
      normalizeString(props.profile.firstName) !== form.firstName ||
      normalizeString(props.profile.lastName) !== form.lastName ||
      normalizeString(props.profile.middleName) !== form.middleName ||
      (props.profile.birthDate ?? "") !== form.birthDate ||
      normalizeString(props.profile.city) !== form.city ||
      normalizeString(props.profile.bio) !== form.bio ||
      normalizeString(props.profile.gender) !== form.gender
    );
  });

  function startEdit() {
    if(!props.profile) return;

    fillFormFromProfile(props.profile);
    saveError.value = "";
    isEditing.value = true;
  }

  function cancelEdit() {
    if (!props.profile) return;

    fillFormFromProfile(props.profile);
    saveError.value = "";
    isEditing.value = false;
  }

  async function saveChanges() {
    if (!props.profile || !hasChanges.value) return;

    saving.value = true;
    saveError.value = "";

    try {
      const payload = {
        firstName: emptyToNull(form.firstName),
        lastName: emptyToNull(form.lastName),
        middleName: emptyToNull(form.middleName),
        birthDate: emptyToNull(form.birthDate),
        city: emptyToNull(form.city),
        bio: emptyToNull(form.bio),
        gender: emptyToNull(form.gender),
      };

      const response = await userProfileService.updateMe(payload);

      isEditing.value = false;
      emit("updated", response.data);
    } catch (err) {
      console.error("Ошибка сохранения профиля:", err);
      saveError.value =
        err?.response?.data?.message || "Не удалось сохранить изменения.";
    } finally {
      saving.value = false;
    }
  }

  function emptyToNull(value) {
    if(value == null) return null;

    const trimmed = typeof value === "string" ? value.trim() : value;
    return trimmed === "" ? null : trimmed;
  }

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
.personal-info {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 16px;
}

.actions {
  display: flex;
  gap: 10px;
}

.action-btn {
  border: 1px solid var(--border-color);
  background: transparent;
  color: var(--text-primary);
  border-radius: 10px;
  padding: 10px 14px;
  cursor: pointer;
}

.action-btn.primary {
  background: var(--btn-primary-bg);
  border-color: var(--btn-primary-bg);
  color: white;
}

.action-btn.secondary {
  background: transparent;
}

.action-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.save-error {
  color: #d93025;
  font-size: 14px;
}

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

.info-item.wide {
  grid-column: 1 / -1;
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

.form-input {
  width: 100%;
  padding: 10px 12px;
  border: 1px solid var(--border-color);
  border-radius: 8px;
  background: var(--bg-primary);
  color: var(--text-primary);
  font: inherit;
}

.form-textarea {
  resize: vertical;
  min-height: 120px;
}
</style>