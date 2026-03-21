<script setup lang="ts">
import profileManImage from '@/assets/Profil_man.png'
import snackstasyIcon from '/Snackstasy_Icon.png'
import Dialog from 'primevue/dialog'
import { ref, computed, watchEffect, onMounted, watch } from 'vue'
import Button from 'primevue/button'
import Dialog_profile from './Dialog_profile.vue'
import Dialog_balance from './Dialog_balance.vue'
import { UserData } from '@/services/Header'
import type { User_Data } from '@/model/UserData'
import type { Login_response } from '@/model/AuthentificationInterface'
import { checkSession } from '@/services/Login'

const visible = ref(false)
const visibleBalance = ref(false)
const balance = ref(25.0)
const profileImage = ref(profileManImage)
const currentUser = ref<User_Data | undefined>();
const sessionData = ref<Login_response>();


  onMounted(async () => {
  await loadSessionData(); // nur einmal beim Laden
});

// Wenn sessionData sich ändert, lade UserData
watch(sessionData, async (newSession) => {
  if (newSession) {
    await loadUserData();
  }
});

// Handlers
const handleProfileImageChange = (newImage: string) => {
  profileImage.value = newImage
}
const handleBalanceUpdate = (newBalance: number) => {
  balance.value = newBalance
}

// Mobile Detection (optional, für Template)
const isMobile = computed(() => window.innerWidth <= 600)

async function loadUserData() {
  if (!sessionData.value) return;
  currentUser.value = await UserData(sessionData.value.ticket_id);
}

async function loadSessionData() {
  sessionData.value = await checkSession();
}
</script>

<template>
  <header class="header">
    <!-- Logo nur für Desktop -->
    <div class="header-logo" v-if="!isMobile">
      <img :src="snackstasyIcon" alt="Snackstasy Icon" class="logo-icon" />
    </div>

    <h1 class="snackstasy-title">Snackstasy</h1>

    <div class="header-actions" v-if="currentUser">
      <!-- Guthaben -->
      <button class="header-budget" @click="visibleBalance = true" title="Guthaben aufladen">
        <h4>{{ currentUser.balance.toFixed(2) }} €</h4>
      </button>

      <!-- Profilbild -->
      <button class="header-profile" @click="visible = true" title="Profil bearbeiten">
        <img :src="profileImage" alt="Profilbild" class="header-Avatar" />
      </button>
    </div>

    <!-- Dialog: Guthaben -->
    <Dialog
      v-model:visible="visibleBalance"
      :modal="true"
      :closable="false"
      class="modern-dialog"
      :style="{ width: '90vw', maxWidth: '700px', marginTop: '5rem' }"
    >
      <Dialog_balance
      v-if="currentUser"
        :currentUser="currentUser"
        @update-balance="handleBalanceUpdate"
        @close-dialog="visibleBalance = false"
        @refresh-user="loadUserData"
      />
      <template #footer>
        <div class="dialog-footer">
          <Button type="button" label="Schließen" severity="secondary" @click="visibleBalance = false" class="footer-button" />
        </div>
      </template>
    </Dialog>

    <!-- Dialog: Profil -->
    <Dialog
      v-model:visible="visible"
      :modal="true"
      :closable="false"
      class="modern-dialog"
      :style="{ width: '90vw', maxWidth: '700px', marginTop: '5rem' }"
    >
      <Dialog_profile
        :visible="visible"
        :current-image="profileImage"
        :currentUser="currentUser"
        @update-profile-image="handleProfileImageChange"
        @switch-to-balance="
          () => {
            visible = false
            visibleBalance = true
          }
        "
      />
      <template #footer>
        <div class="dialog-footer">
          <Button type="button" label="Schließen" severity="secondary" @click="visible = false" class="footer-button" />
        </div>
      </template>
    </Dialog>
  </header>
</template>

<style scoped>
.header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  width: 100%;
  padding: 0.5rem 1rem;
  background: linear-gradient(135deg, #1a1a1a 0%, #2d2d2d 100%);
  color: white;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
}

.header-logo {
  display: flex;
  align-items: center;
}
.logo-icon {
  width: 40px;
  height: 40px;
  object-fit: contain;
}

.snackstasy-title {
  font-family: 'Poppins', sans-serif;
  font-size: 24px;
  font-weight: 800;
  background: linear-gradient(135deg, #ffd700 100%, #ffed4e 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  text-shadow: 0 2px 6px rgba(255, 215, 0, 0.2);
  text-align: center;
  flex: 1;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.header-budget {
  border: 2px solid #ffd700;
  border-radius: 25px;
  padding: 0.25rem 1rem;
  font-weight: 600;
  cursor: pointer;
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.2) 0%, rgba(255, 215, 0, 0.1) 100%);
  color: white;
  transition: transform 0.2s ease;
}
.header-budget:hover {
  transform: scale(1.05);
}

.header-profile {
  border: none;
  border-radius: 50%;
  padding: 0.25rem;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}
.header-profile:hover {
  transform: scale(1.1);
  box-shadow: 0 4px 12px rgba(255, 215, 0, 0.2);
}

.header-Avatar {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  object-fit: cover;
}

h4 {
  margin: 0;
  font-size: 14px;
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
  padding: 1rem 1.5rem;
}
:deep(.modern-dialog .p-dialog-content) {
  background: linear-gradient(135deg, #1a1a1a 0%, #2d2d2d 100%);
  color: white;
  padding: 1.5rem 2rem;
}


/* Mobile Anpassungen */
@media (max-width: 600px) {
  /* Header-Anpassungen wie vorher */
  .header-logo {
    display: none;
  }
  .snackstasy-title {
    font-size: 20px;
    flex: 1;
    text-align: center;
  }

  /* Dialog: Abstand zum unteren Rand vergrößern */
  :deep(.modern-dialog) {
    margin-bottom: 4rem !important; /* Abstand unten für leichtere Button-Bedienung */
  }

  /* Optional: Footer Buttons größer auf Mobile */
  :deep(.modern-dialog .dialog-footer button) {
    padding: 1rem 1.5rem;
    font-size: 16px;
  }
}
</style>
