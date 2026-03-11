<script setup lang="ts">
import profileManImage from '@/assets/Profil_man.png'
import profileWomanImage from '@/assets/Profil_woman.png'
import snackstasyIcon from '/Snackstasy_Icon.png'
import Dialog from 'primevue/dialog'
import { ref } from 'vue'
import Button from 'primevue/button'
import Dialog_profile from './Dialog_profile.vue'
import Dialog_balance from './Dialog_balance.vue'

const visible = ref(false)
const visibleBalance = ref(false)
const balance = ref(25.0)
const profileImage = ref(profileManImage)

const handleProfileImageChange = (newImage: string) => {
  profileImage.value = newImage
}

const handleBalanceUpdate = (newBalance: number) => {
  balance.value = newBalance
}
</script>

<template>
  <header class="header">
    <div class="header-logo">
      <img :src="snackstasyIcon" alt="Snackstasy Icon" class="logo-icon" />
    </div>
    <h1 class="snackstasy-title">Snackstasy</h1>
    <div style="display: flex; flex-direction: row; align-items: center; gap: 1rem">
      <button class="header-budget" @click="visibleBalance = true" title="Guthaben aufladen">
        <h4>{{ balance.toFixed(2) }} €</h4>
      </button>
      <button class="header-profile" @click="visible = true" title="Profil bearbeiten">
        <img :src="profileImage" alt="Profilbild" class="header-Avatar" />
      </button>
    </div>
    <Dialog
      v-model:visible="visibleBalance"
      :modal="true"
      :closable="false"
      class="modern-dialog"
      :style="{ width: '90vw', maxWidth: '700px' }"
    >
      <Dialog_balance
        :current-balance="balance"
        @update-balance="handleBalanceUpdate"
        @close-dialog="visibleBalance = false"
      />
      <template #footer>
        <div class="dialog-footer">
          <Button
            type="button"
            label="Schließen"
            severity="secondary"
            @click="visibleBalance = false"
            class="footer-button"
          ></Button>
        </div>
      </template>
    </Dialog>
    <Dialog
      v-model:visible="visible"
      :modal="true"
      :closable="false"
      class="modern-dialog"
      :style="{ width: '90vw', maxWidth: '700px' }"
    >
      <Dialog_profile
        :visible="visible"
        :current-image="profileImage"
        @update-profile-image="handleProfileImageChange"
      />
      <template #footer>
        <div class="dialog-footer">
          <Button
            type="button"
            label="Abbrechen"
            severity="secondary"
            @click="visible = false"
            class="footer-button"
          ></Button>
          <Button
            type="button"
            label="Speichern"
            @click="visible = false"
            class="footer-button save-button"
          ></Button>
        </div>
      </template>
    </Dialog>
  </header>
</template>

<style scoped>
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
  cursor: pointer;
  color: white;
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.2) 0%, rgba(255, 215, 0, 0.1) 100%);
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

/* Modern Dialog Styling */
:deep(.modern-dialog) {
  border-radius: 16px !important;
  overflow: hidden !important;
}

:deep(.modern-dialog .p-dialog-header) {
  background: linear-gradient(135deg, #ffd700 0%, #ffed4e 100%);
  color: #1a1a1a;
  border: none;
  padding: 1.5rem;
  border-radius: 16px 16px 0 0;
}

:deep(.modern-dialog .p-dialog-header .p-dialog-title) {
  font-family: 'Poppins', sans-serif;
  font-weight: 700;
  font-size: 20px;
  letter-spacing: 0.5px;
}

:deep(.modern-dialog .p-dialog-header .p-dialog-header-close) {
  color: #1a1a1a;
  width: 40px;
  height: 40px;
  transition: all 0.3s ease;
}

:deep(.modern-dialog .p-dialog-header .p-dialog-header-close:hover) {
  background-color: rgba(26, 26, 26, 0.2);
  border-radius: 8px;
  transform: rotate(90deg);
}

:deep(.modern-dialog .p-dialog-content) {
  background: linear-gradient(135deg, #1a1a1a 0%, #2d2d2d 100%);
  padding: 2rem;
  border: none;
  color: white;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  padding-top: 1.5rem;
  border-top: 1px solid rgba(255, 215, 0, 0.1);
  margin-top: 1.5rem;
}

.footer-button {
  font-family: 'Poppins', sans-serif;
  font-weight: 600;
  padding: 0.75rem 1.5rem;
  border-radius: 8px;
  transition: all 0.3s ease;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  font-size: 13px;
}

:deep(.footer-button.p-button-secondary) {
  background-color: rgba(255, 255, 255, 0.1);
  border: 2px solid rgba(255, 255, 255, 0.2);
  color: white;
}

:deep(.footer-button.p-button-secondary:hover) {
  background-color: rgba(255, 255, 255, 0.15);
  border-color: rgba(255, 255, 255, 0.4);
  transform: translateY(-2px);
}

:deep(.save-button.p-button) {
  background: linear-gradient(135deg, #ffd700 0%, #ffed4e 100%);
  border: none;
  color: #1a1a1a;
}

:deep(.save-button.p-button:hover) {
  box-shadow: 0 6px 16px rgba(255, 215, 0, 0.3);
  transform: translateY(-2px);
}

:deep(.p-dialog-mask.p-component-overlay) {
  backdrop-filter: blur(4px);
}
</style>
