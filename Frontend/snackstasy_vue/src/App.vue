<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import Header from './components/header/Header.vue'
import { useRoute } from 'vue-router'

const route = useRoute()
</script>

<template>
  <div class="app">
    <Header v-if="route.meta.showHeader" />
    <RouterView />
  </div>
</template>

<style scoped></style>
