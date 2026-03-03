<script setup lang="ts">
import InputText from 'primevue/inputtext'
</script>

<template>
  <div class="Dialog-frame">
    <span class="text-surface-500 dark:text-surface-400 block mb-8">Update your information.</span>
    <div class="flex items-center gap-4 mb-4">
      <label for="username" class="font-semibold w-24">Username</label>
      <InputText id="username" class="flex-auto" autocomplete="off" />
    </div>
    <div class="flex items-center gap-4 mb-8">
      <label for="email" class="font-semibold w-24">Email</label>
      <InputText id="email" class="flex-auto" autocomplete="off" />
    </div>
  </div>
</template>

<style scoped>
.Dialog-frame {
  width: 100%;
  height: 100%;
  background-color: #1e1e1e;
  color: white;
}
</style>
