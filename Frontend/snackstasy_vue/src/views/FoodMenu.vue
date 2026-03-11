<script setup lang="ts">
import { ref, onMounted } from 'vue'
import InputText from 'primevue/inputtext'

const Hallo = ref('Moin Leude')

function DanielSagtHallo() {
  let Hallo = 'Moin Leude'
}
</script>

<template>
  <InputText v-model="Hallo"></InputText>
  {{ Hallo }}
</template>

<style scoped></style>
