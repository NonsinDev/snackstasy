<script setup lang="ts">
import profileManImage from '@/assets/Profil_man.png'
import Dialog from 'primevue/dialog'
import { ref } from 'vue'
import Button from 'primevue/button'
import Dialog_profile from './Dialog_profile.vue'

const visible = ref(false)
</script>

<template>
  <header class="header">
    <div class="header-Title">
      <h4>Snackstasy</h4>
    </div>
    <div class="header-Title">
      <h4>Snackstasy</h4>
    </div>

    <div class="header-budget">
      <h4>25.00€</h4>
    </div>
    <button class="header-profile" @click="visible = true">
      <img :src="profileManImage" alt="Profilbild" class="header-Avatar" />
    </button>
    <Dialog v-model:visible="visible" header="Profil bearbeiten" modal>
      <div class="Dialog-frame">
        <Dialog_profile :visible="visible" />
        <div class="flex justify-end gap-2">
          <Button
            type="button"
            label="Cancel"
            severity="secondary"
            @click="visible = false"
          ></Button>
          <Button type="button" label="Save" @click="visible = false"></Button>
        </div>
      </div>
    </Dialog>
  </header>
</template>

<style scoped>
.header {
  width: 100%;
  background-color: #1e1e1e;
  color: white;
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 0.5rem;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
}

.header-Title {
  width: 100%;
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 20px;
}
.header-budget {
  border: 2px solid white;
  border-radius: 20px;
  padding: 0 20px;
}
.header-profile {
  border: 1px solid rgb(0, 0, 0);
  border-radius: 20px;
  padding: 0;

  cursor: pointer;
}

.header-Avatar {
  width: 40px;
  height: 40px;
  object-fit: cover;
  display: block;
}

h4 {
  margin: 0;
}

.Dialog-frame {
  background-color: bisque;
  padding: 1rem;
}
</style>
