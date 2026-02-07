<template>
  <div>
    <ul>
      <li v-for="c in items" :key="c.id">
        <strong>{{ c.name }}</strong>
        <div>{{ c.description }}</div>
      </li>
    </ul>

    <div v-if="isLoading">Загрузка...</div>
    <div v-else-if="!hasMore">Конец списка</div>
    <div v-if="error">Ошибка загрузки</div>

    <div v-if="showSentinel" ref="sentinel" style="height:1px;"></div>
  </div>
</template>

<script setup>
import { ref } from "vue";

defineProps({
  items: { type: Array, default: () => [] },
  isLoading: Boolean,
  hasMore: Boolean,
  error: null,
  showSentinel: { type: Boolean, default: true },
});

const sentinel = ref(null);
defineExpose({ sentinel });
</script>
