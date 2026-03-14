<script setup lang="ts">
import profileManImage from '@/assets/Profil_man.png'
import snackstasyIcon from '/Snackstasy_Icon.png'
import Dialog from 'primevue/dialog'
import { ref } from 'vue'
import Button from 'primevue/button'
import Dialog_profile from './Dialog_profile.vue'

const visible = ref(false)
</script>

<template>
  <header class="header">
    <div class="header-logo">
      <img :src="snackstasyIcon" alt="Snackstasy Icon" class="logo-icon" />
    </div>
    <h1 class="snackstasy-title">Snackstasy</h1>
    <div style="display: flex; flex-direction: row; align-items: center; gap: 1rem">
      <div class="header-budget">
        <h4>25.00€</h4>
      </div>
      <button class="header-profile" @click="visible = true" title="Profil bearbeiten">
        <img :src="profileManImage" alt="Profilbild" class="header-Avatar" />
      </button>
    </div>
    <Dialog v-model:visible="visible" header="Profil bearbeiten">
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
@import url('https://fonts.googleapis.com/css2?family=Poppins:wght@700;800&family=Montserrat:wght@600&display=swap');

.header {
  width: 100%;
  background: linear-gradient(135deg, #1a1a1a 0%, #2d2d2d 100%);
  color: white;
  display: flex;
  align-items: center;
  gap: 2rem;
  padding: 0.75rem 1.5rem;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
}

.header-logo {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.logo-icon {
  width: 48px;
  height: 48px;
  object-fit: contain;
}

.snackstasy-title {
  font-family: 'Poppins', sans-serif;
  font-size: 28px;
  font-weight: 800;
  margin-left: auto;
  margin-right: auto;
  background: linear-gradient(135deg, #ffd700 0%, #ffed4e 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  letter-spacing: 0.5px;
  text-shadow: 0 2px 8px rgba(255, 215, 0, 0.2);
}

.header-budget {
  border: 2px solid #ffd700;
  border-radius: 25px;
  padding: 0.5rem 1.5rem;
  background-color: rgba(255, 215, 0, 0.1);
  font-family: 'Montserrat', sans-serif;
  font-weight: 600;
  transition: all 0.3s ease;
}

.header-budget:hover {
  background-color: rgba(255, 215, 0, 0.2);
  transform: scale(1.05);
}

.header-profile {
  border: none;
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.2) 0%, rgba(255, 215, 0, 0.1) 100%);
  border-radius: 50%;
  padding: 0.5rem;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 2px 8px rgba(255, 215, 0, 0.1);
  margin-left: auto;
}

.header-profile:hover {
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.4) 0%, rgba(255, 215, 0, 0.2) 100%);
  transform: scale(1.1);
  box-shadow: 0 4px 12px rgba(255, 215, 0, 0.2);
}

.header-profile:active {
  transform: scale(0.95);
}

.header-Avatar {
  width: 42px;
  height: 42px;
  object-fit: cover;
  display: block;
  border-radius: 50%;
}

h4 {
  margin: 0;
}

.Dialog-frame {
  background-color: bisque;
  padding: 1rem;
}
</style>
