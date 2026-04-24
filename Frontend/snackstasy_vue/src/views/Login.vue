<!-- src/views/LoginView.vue -->
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { QrcodeStream } from 'vue-qrcode-reader'
import { type DetectedBarcode } from 'barcode-detector/pure'
import InputText from 'primevue/inputtext'
import { Login } from '@/services/Login'
import { initAuth } from '@/services/Authentification'

const username = ref('')
const ticket_id = ref('')
const router = useRouter()
const error = ref('')
const showScanner = ref(false)
const isLoggingIn = ref(false)
const loginSuccess = ref(false)
const eyeOpenAmount = ref(1)
const eyeLeftPos = ref({ x: 0, y: 0 })
const eyeRightPos = ref({ x: 0, y: 0 })

async function handleLogin() {
  try {
    const result = await Login({ username: username.value, ticket_id: ticket_id.value })

    await initAuth()

    if (result.logged_in) {
      router.push(`/`)
    } else {
      alert('Falscher Benutzername oder Passwort')
    }
  } catch (err) {
    alert('Fehler beim Login')
    console.error(err)
  }
}

function onError(err: Error) {
  const messages: Record<string, string> = {
    NotAllowedError: 'Kamera-Zugriff wurde verweigert.',
    NotFoundError: 'Keine Kamera gefunden.',
    NotSupportedError: 'HTTPS oder localhost erforderlich.',
    NotReadableError: 'Kamera wird bereits verwendet.',
    OverconstrainedError: 'Kamera nicht geeignet.',
    StreamApiNotSupportedError: 'Stream API nicht unterstützt.',
    InsecureContextError: 'Kamera nur über HTTPS verfügbar.',
  }
  error.value = messages[err.name] ?? err.message
  showScanner.value = false
}

function paintBoundingBox(detectedCodes: DetectedBarcode[], ctx: CanvasRenderingContext2D) {
  for (const detectedCode of detectedCodes) {
    const {
      boundingBox: { x, y, width, height },
    } = detectedCode
    ctx.lineWidth = 2
    ctx.strokeStyle = '#a78bfa'
    ctx.strokeRect(x, y, width, height)
  }
}

function onDetect(detectedCodes: DetectedBarcode[]) {
  if (detectedCodes.length > 0 && detectedCodes[0]) {
    ticket_id.value = detectedCodes[0].rawValue
    showScanner.value = false
    error.value = ''
  }
}

function updateEyePosition(event: MouseEvent) {
  const btn = document.querySelector('.login-btn') as HTMLElement
  if (!btn) return

  const rect = btn.getBoundingClientRect()
  const centerX = rect.left + rect.width / 2
  const centerY = rect.top + rect.height / 2

  const angle = Math.atan2(event.clientY - centerY, event.clientX - centerX)
  const distance = 15

  eyeLeftPos.value = {
    x: Math.cos(angle) * distance,
    y: Math.sin(angle) * distance,
  }
  eyeRightPos.value = {
    x: Math.cos(angle) * distance,
    y: Math.sin(angle) * distance,
  }
}

async function handleLoginWithBlink() {
  await animateEyesClosed()
  await handleLogin()
  await animateEyesOpen()
}

async function animateEyesClosed() {
  const steps = 10
  const duration = 300
  const stepDuration = duration / steps

  for (let i = 0; i <= steps; i++) {
    eyeOpenAmount.value = 1 - i / steps
    await new Promise((resolve) => setTimeout(resolve, stepDuration))
  }
}

async function animateEyesOpen() {
  const steps = 10
  const duration = 300
  const stepDuration = duration / steps

  for (let i = 0; i <= steps; i++) {
    eyeOpenAmount.value = i / steps
    await new Promise((resolve) => setTimeout(resolve, stepDuration))
  }
}

function closeEyes() {
  animateEyesClosed()
}

function openEyes() {
  animateEyesOpen()
}

onMounted(() => {
  document.addEventListener('mousemove', updateEyePosition)
})
</script>

<template>
  <div class="scene" :class="{ 'scene--success': loginSuccess }">
    <!-- Animated background orbs -->
    <div class="orb orb-1"></div>
    <div class="orb orb-2"></div>
    <div class="orb orb-3"></div>
    <div class="orb orb-4"></div>

    <!-- Grid overlay -->
    <div class="grid-overlay"></div>

    <!-- Card -->
    <div class="card" :class="{ 'card--success': loginSuccess }">
      <!-- Header -->
      <div class="card__header">
        <h1 class="card__title">Willkommen</h1>
        <p class="card__subtitle">Ticket-System · Zugang</p>
      </div>

      <!-- Fields -->
      <div class="fields">
        <div class="field">
          <div class="field__icon"><i class="pi pi-user"></i></div>
          <InputText
            v-model="username"
            id="username"
            type="text"
            class="field__input"
            placeholder="Benutzername"
            @keyup.enter="handleLogin"
          />
        </div>

        <div class="field">
          <div class="field__icon"><i class="pi pi-ticket"></i></div>
          <InputText
            v-model="ticket_id"
            id="password"
            class="field__input"
            placeholder="Ticket ID"
            @keyup.enter="handleLogin"
            @focus="closeEyes"
            @blur="openEyes"
          />
          <button
            class="qr-btn"
            @click="showScanner = !showScanner"
            :class="{ 'qr-btn--active': showScanner }"
            :title="showScanner ? 'Scanner schließen' : 'QR-Code scannen'"
          >
            <i :class="showScanner ? 'pi pi-times' : 'pi pi-qrcode'"></i>
          </button>
        </div>

        <!-- Scanner -->
        <transition name="scanner-slide">
          <div v-if="showScanner" class="scanner-box">
            <div class="scanner-hint">
              <span class="scanner-dot"></span>
              QR-Code vor die Kamera halten
            </div>
            <div class="scanner-frame">
              <qrcode-stream
                :track="paintBoundingBox"
                @detect="onDetect"
                @error="onError"
                class="scanner-stream"
              />
              <div class="corner corner--tl"></div>
              <div class="corner corner--tr"></div>
              <div class="corner corner--bl"></div>
              <div class="corner corner--br"></div>
            </div>
          </div>
        </transition>

        <!-- Error -->
        <transition name="fade">
          <div v-if="error" class="error-msg">
            <i class="pi pi-exclamation-circle"></i>
            {{ error }}
          </div>
        </transition>
      </div>

      <!-- Login Button -->
      <button
        class="login-btn"
        @click="handleLoginWithBlink"
        :disabled="isLoggingIn || loginSuccess"
        :class="{ 'login-btn--loading': isLoggingIn, 'login-btn--success': loginSuccess }"
      >
        <div class="eyes-container">
          <div class="eye eye-left">
            <div class="eye-lid" :style="{ height: `${(1 - eyeOpenAmount) * 100}%` }"></div>
            <div
              class="pupil"
              :style="{ transform: `translate(${eyeLeftPos.x}px, ${eyeLeftPos.y}px)` }"
            ></div>
          </div>
          <div class="eye eye-right">
            <div class="eye-lid" :style="{ height: `${(1 - eyeOpenAmount) * 100}%` }"></div>
            <div
              class="pupil"
              :style="{ transform: `translate(${eyeRightPos.x}px, ${eyeRightPos.y}px)` }"
            ></div>
          </div>
        </div>
      </button>

      <!-- Employee Login Link -->
      <div class="employee-login-link">
        <p>Mitarbeiter?</p>
        <button class="link-btn" @click="router.push('/employee-login')">
          Zum Mitarbeiter-Zugang
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
* {
  box-sizing: border-box;
}

.scene {
  width: 100%;
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #f5f5f5;
  padding: 1rem;
}

.scene--success {
  background: #f5f5f5;
}

.orb,
.grid-overlay {
  display: none;
}

.card {
  width: 100%;
  max-width: 380px;
  background: white;
  border-radius: 16px;
  padding: 2rem;
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.08);
}

.card--success {
  background: white;
}

.card__header {
  text-align: center;
  margin-bottom: 1.5rem;
}

.logo-ring,
.logo-icon {
  display: none;
}

.card__title {
  font-size: 1.5rem;
  font-weight: 600;
  color: #333;
  margin: 0;
}

.card__subtitle {
  font-size: 0.8rem;
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
  border: 1px solid #e5e5e5;
}

.field:focus-within {
  border-color: #3366ff;
  background: #f0f4ff;
}

.field__icon {
  color: #999;
  font-size: 0.9rem;
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

.qr-btn {
  flex-shrink: 0;
  width: 36px;
  height: 36px;
  border-radius: 8px;
  border: none;
  background: white;
  color: #999;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.9rem;
}

.qr-btn:active {
  background: #f0f0f0;
}

.qr-btn--active {
  background: #e8f0ff;
  color: #3366ff;
}

.scanner-slide-enter-active,
.scanner-slide-leave-active {
  transition: opacity 0.15s;
}

.scanner-slide-enter-from,
.scanner-slide-leave-to {
  opacity: 0;
}

.scanner-box {
  border-radius: 12px;
  overflow: hidden;
  background: #f9f9f9;
  border: 1px solid #e5e5e5;
  margin-top: 0.75rem;
}

.scanner-hint {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding: 0.75rem;
  font-size: 0.8rem;
  color: #666;
}

.scanner-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: #999;
}

.scanner-frame {
  position: relative;
}

.scanner-stream {
  width: 100%;
  display: block;
}

.corner {
  position: absolute;
  width: 14px;
  height: 14px;
  border-color: #3366ff;
  border-style: solid;
}

.corner--tl {
  top: 6px;
  left: 6px;
  border-width: 2px 0 0 2px;
}

.corner--tr {
  top: 6px;
  right: 6px;
  border-width: 2px 2px 0 0;
}

.corner--bl {
  bottom: 6px;
  left: 6px;
  border-width: 0 0 2px 2px;
}

.corner--br {
  bottom: 6px;
  right: 6px;
  border-width: 0 2px 2px 0;
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

.login-btn {
  margin-top: 1.5rem;
  width: 100%;
  border: none;
  border-radius: 12px;
  background: #3366ff;
  color: white;
  font-size: 0.95rem;
  font-weight: 600;
  cursor: pointer;
  padding: 0.85rem;
  transition: background 0.15s;
  display: flex;
  align-items: center;
  justify-content: center;
}

.login-btn:hover:not(:disabled) {
  background: #254fbf;
}

.login-btn:active:not(:disabled) {
  background: #1a3a99;
}

.login-btn:disabled {
  cursor: not-allowed;
  background: #ccc;
  color: #999;
}

.login-btn--loading {
  background: #999;
}

.login-btn--success {
  background: #22a55a !important;
}

/* Eyes */
.eyes-container {
  display: flex;
  gap: 0.75rem;
  align-items: center;
  justify-content: center;
}

.eye {
  width: 32px;
  height: 32px;
  background: white;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  position: relative;
}

.pupil {
  width: 12px;
  height: 12px;
  background: #3366ff;
  border-radius: 50%;
  transition: transform 0.05s ease-out;
}

.eye-lid {
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  background: #172e70;
  z-index: 10;
  transition: height 0.05s ease-out;
}

.card__footer {
  text-align: center;
  font-size: 0.75rem;
  color: #999;
  margin-top: 1rem;
}

.employee-login-link {
  text-align: center;
  margin-top: 1.5rem;
  padding-top: 1rem;
  border-top: 1px solid #e5e5e5;
}

.employee-login-link p {
  margin: 0 0 0.5rem;
  font-size: 0.85rem;
  color: #666;
}

.link-btn {
  background: transparent;
  border: 1px solid #3366ff;
  color: #3366ff;
  padding: 0.6rem 1.2rem;
  border-radius: 8px;
  cursor: pointer;
  font-size: 0.85rem;
  font-weight: 600;
  transition: all 0.2s;
}

.link-btn:hover {
  background: #f0f4ff;
}

.link-btn:active {
  background: #e8ecff;
}

@media (max-width: 480px) {
  .card {
    padding: 1.5rem;
    border-radius: 14px;
  }

  .card__title {
    font-size: 1.3rem;
  }
}
</style>
