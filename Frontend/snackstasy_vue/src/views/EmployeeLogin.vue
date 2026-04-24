<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import InputText from 'primevue/inputtext'
import InputGroup from 'primevue/inputgroup'
import InputGroupAddon from 'primevue/inputgroupaddon'
import Password from 'primevue/password'
import Button from 'primevue/button'
import { EmployeeLogin as EmployeeLoginService } from '@/services/Employee'
import { initEmployeeAuth } from '@/services/Authentification'

const username = ref('')
const password = ref('')
const router = useRouter()
const error = ref('')
const isLoggingIn = ref(false)
const loginSuccess = ref(false)

async function handleLogin() {
  if (!username.value || !password.value) {
    error.value = 'Benutzername und Passwort erforderlich'
    return
  }

  isLoggingIn.value = true
  error.value = ''

  try {
    console.log('Versuche Login mit:', { username: username.value })
    const result = await EmployeeLoginService({
      username: username.value,
      password: password.value,
    })

    console.log('Login Response:', result)

    if (result.logged_in) {
      console.log('Login erfolgreich!')
      loginSuccess.value = true

      console.log('Speichere Auth-Daten mit:', {
        employee_id: result.employee_id,
        username: username.value,
        stand_id: result.stand_id,
      })

      await initEmployeeAuth(result.employee_id, username.value, result.stand_id)

      // Überprüfe, dass Daten wirklich gespeichert sind
      const stored = localStorage.getItem('employeeAuth')
      console.log('Gespeicherte Daten in localStorage:', stored)

      if (!stored) {
        error.value = 'Auth-Daten konnten nicht gespeichert werden'
        loginSuccess.value = false
        isLoggingIn.value = false
        return
      }

      console.log('Auth-Daten erfolgreich gespeichert, navigiere zu Dashboard...')

      // Navigiere mit replace um History zu überschreiben
      try {
        await router.push({ path: '/employee-dashboard', replace: false })
        console.log('Navigation erfolgreich!')
      } catch (navError) {
        console.error('Navigation Error:', navError)
        error.value = 'Navigation zur Dashboard fehlgeschlagen'
        loginSuccess.value = false
      }
    } else {
      error.value = 'Falscher Benutzername oder Passwort'
    }
  } catch (err: any) {
    console.error('Login Error:', err)
    console.error('Error Response:', err.response?.data)
    console.error('Error Status:', err.response?.status)
    console.error('Error Message:', err.message)

    if (err.response?.status === 401) {
      error.value = 'Falscher Benutzername oder Passwort'
    } else if (
      err.message?.includes('ERR_NETWORK') ||
      err.message?.includes('Connection refused')
    ) {
      error.value = 'Verbindung zum Server fehlgeschlagen. Bitte überprüfen Sie den Server.'
    } else {
      error.value = `Login fehlgeschlagen: ${err.message || 'Unbekannter Fehler'}`
    }
  } finally {
    isLoggingIn.value = false
  }
}

function goBack() {
  router.push('/login')
}
</script>

<template>
  <div class="employee-login-scene">
    <!-- Animated background orbs -->
    <div class="orb orb-1"></div>
    <div class="orb orb-2"></div>
    <div class="orb orb-3"></div>
    <div class="orb orb-4"></div>

    <!-- Card -->
    <div class="card" :class="{ 'card--success': loginSuccess }">
      <!-- Header -->
      <div class="card__header">
        <h1 class="card__title">Mitarbeiter-Zugang</h1>
        <p class="card__subtitle">Bitte melden Sie sich an</p>
      </div>

      <!-- Fields -->
      <div class="fields">
        <div class="field">
          <div class="field__icon"><i class="pi pi-user"></i></div>
          <InputText
            v-model="username"
            type="text"
            class="field__input"
            placeholder="Benutzername"
            @keyup.enter="handleLogin"
            :disabled="isLoggingIn"
          />
        </div>

        <div class="field">
          <div class="field__icon"><i class="pi pi-lock"></i></div>
          <Password
            v-model="password"
            class="field__input"
            placeholder="Passwort"
            :toggleMask="true"
            @keyup.enter="handleLogin"
            :disabled="isLoggingIn"
            input-class="password-input"
            :feedback="false"
          />
        </div>

        <!-- Error -->
        <transition name="fade">
          <div v-if="error" class="error-msg">
            <i class="pi pi-exclamation-circle"></i>
            {{ error }}
          </div>
        </transition>
      </div>

      <!-- Action Buttons -->
      <div class="button-container">
        <button
          class="login-btn"
          @click="handleLogin"
          :disabled="isLoggingIn || loginSuccess"
          :class="{ 'login-btn--loading': isLoggingIn, 'login-btn--success': loginSuccess }"
        >
          <i v-if="isLoggingIn" class="pi pi-spinner pi-spin"></i>
          <i v-else-if="loginSuccess" class="pi pi-check"></i>
          <span v-else>Login</span>
        </button>

        <button class="back-btn" @click="goBack" :disabled="isLoggingIn || loginSuccess">
          <i class="pi pi-arrow-left"></i>
          Zurück
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
* {
  box-sizing: border-box;
}

.employee-login-scene {
  width: 100%;
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 1rem;
  position: relative;
  overflow: hidden;
}

.orb {
  display: none;
}

.card {
  width: 100%;
  max-width: 400px;
  background: white;
  border-radius: 16px;
  padding: 2rem;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease;
}

.card--success {
  background: white;
}

.card__header {
  text-align: center;
  margin-bottom: 2rem;
}

.card__title {
  font-size: 1.5rem;
  font-weight: 700;
  color: #333;
  margin: 0;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.card__subtitle {
  font-size: 0.9rem;
  color: #999;
  margin: 0.5rem 0 0;
}

.fields {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin: 1.5rem 0;
}

.field {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  background: #f9f9f9;
  border-radius: 12px;
  padding: 0.75rem 1rem;
  border: 2px solid #e5e5e5;
  transition: all 0.2s;
}

.field:focus-within {
  border-color: #667eea;
  background: #f5f7ff;
}

.field__icon {
  color: #667eea;
  font-size: 1rem;
  flex-shrink: 0;
}

.field__input {
  flex: 1;
  background: transparent !important;
  border: none !important;
  outline: none !important;
  color: #333 !important;
  font-size: 0.95rem !important;
  padding: 0 !important;
  box-shadow: none !important;
}

.field__input::placeholder {
  color: #ccc !important;
}

:deep(.p-password-input) {
  background: transparent !important;
  border: none !important;
  outline: none !important;
  color: #333 !important;
  padding: 0 !important;
}

:deep(.p-password-input::placeholder) {
  color: #ccc !important;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.15s;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.error-msg {
  background: #ffe8e8;
  border: 1px solid #ffb3b3;
  border-radius: 10px;
  padding: 0.75rem 1rem;
  color: #d32f2f;
  font-size: 0.85rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.button-container {
  display: flex;
  gap: 1rem;
  margin-top: 2rem;
}

.login-btn {
  flex: 1;
  border: none;
  border-radius: 10px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  font-size: 0.95rem;
  font-weight: 600;
  cursor: pointer;
  padding: 0.9rem;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.login-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 16px rgba(102, 126, 234, 0.4);
}

.login-btn:active:not(:disabled) {
  transform: translateY(0);
}

.login-btn:disabled {
  cursor: not-allowed;
  opacity: 0.6;
}

.login-btn--success {
  background: linear-gradient(135deg, #22a55a 0%, #1b7d42 100%);
}

.back-btn {
  flex: 1;
  border: 2px solid #e5e5e5;
  border-radius: 10px;
  background: white;
  color: #667eea;
  font-size: 0.95rem;
  font-weight: 600;
  cursor: pointer;
  padding: 0.9rem;
  transition: all 0.2s;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.back-btn:hover:not(:disabled) {
  border-color: #667eea;
  background: #f5f7ff;
}

.back-btn:disabled {
  cursor: not-allowed;
  opacity: 0.6;
}

@media (max-width: 480px) {
  .card {
    padding: 1.5rem;
    border-radius: 14px;
  }

  .card__title {
    font-size: 1.3rem;
  }

  .button-container {
    gap: 0.5rem;
  }
}
</style>
