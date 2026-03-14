<!-- src/views/LoginView.vue -->
<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { QrcodeStream } from 'vue-qrcode-reader'
import { type DetectedBarcode } from 'barcode-detector/pure'
import InputText from 'primevue/inputtext'
import { checkLogin } from '@/services/Login'

const username = ref('')
const password = ref('')
const router = useRouter()
const error = ref('')
const showScanner = ref(false)
const isLoggingIn = ref(false)
const loginSuccess = ref(false)

/* function login() {
  if (username.value === 'max' && password.value === '1') {
    sessionStorage.setItem('auth', 'true') // ← sessionStorage statt localStorage
    router.push('/')
  } else {
    alert('Falscher Benutzername oder Passwort')
  }
} */

async function login() {
  try {
    let result = await checkLogin({
      username: username.value,
      password: password.value,
    })
    if (result.success == true) {
      sessionStorage.setItem('auth', 'true')
      router.push(`/`)
    } else {
      alert('Falscher Benutzername oder Passwort')
      console.warn(
        'Falscher Benutzername oder Passwort ',

        result,
        username.value,
        password.value,
      )
    }
  } catch (error) {
    alert('catch ')
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
    password.value = detectedCodes[0].rawValue
    showScanner.value = false
    error.value = ''
  }
}

// Orbs animieren
onMounted(() => {
  const orbs = document.querySelectorAll('.orb')
  orbs.forEach((orb, i) => {
    const el = orb as HTMLElement
    const speed = 6 + i * 2.5
    const xAmp = 30 + i * 15
    const yAmp = 20 + i * 10
    let t = i * 1.3

    const animate = () => {
      t += 0.008
      el.style.transform = `translate(${Math.sin(t * 0.7) * xAmp}px, ${Math.cos(t * 0.5) * yAmp}px)`
      requestAnimationFrame(animate)
    }
    animate()
  })
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
            @keyup.enter="login"
          />
        </div>

        <div class="field">
          <div class="field__icon"><i class="pi pi-ticket"></i></div>
          <InputText
            v-model="password"
            id="password"
            class="field__input"
            placeholder="Ticket ID"
            @keyup.enter="login"
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
        @click="login"
        :disabled="isLoggingIn || loginSuccess"
        :class="{ 'login-btn--loading': isLoggingIn, 'login-btn--success': loginSuccess }"
      >
        <div class="logo-ring">
          <svg viewBox="0 0 48 48" fill="none" xmlns="http://www.w3.org/2000/svg" class="logo-icon">
            <circle cx="24" cy="24" r="20" stroke="url(#g1)" stroke-width="2" />
            <path
              d="M16 24h16M24 16l8 8-8 8"
              stroke="url(#g2)"
              stroke-width="2.5"
              stroke-linecap="round"
              stroke-linejoin="round"
            />
            <defs>
              <linearGradient id="g1" x1="4" y1="4" x2="44" y2="44">
                <stop stop-color="#818cf8" />
                <stop offset="1" stop-color="#a78bfa" />
              </linearGradient>
              <linearGradient id="g2" x1="16" y1="16" x2="32" y2="32">
                <stop stop-color="#c4b5fd" />
                <stop offset="1" stop-color="#818cf8" />
              </linearGradient>
            </defs>
          </svg>
        </div>
      </button>

      <!-- Footer -->
      <p class="card__footer">Sicherer Zugang · Verschlüsselt</p>
    </div>
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Syne:wght@400;600;700;800&family=DM+Sans:wght@300;400;500&display=swap');

* {
  box-sizing: border-box;
}

/* ── Scene ── */
.scene {
  max-height: 100vh;
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #0a0a14;
  overflow: hidden;
  position: relative;
  font-family: 'DM Sans', sans-serif;
  padding: 1rem;
  transition: background 0.8s ease;
}

.scene--success {
  background: #07120f;
}

/* ── Orbs ── */
.orb {
  position: absolute;
  border-radius: 50%;
  filter: blur(80px);
  opacity: 0.35;
  pointer-events: none;
}
.orb-1 {
  width: 500px;
  height: 500px;
  background: #4f46e5;
  top: -150px;
  left: -100px;
}
.orb-2 {
  width: 400px;
  height: 400px;
  background: #7c3aed;
  bottom: -100px;
  right: -80px;
  opacity: 0.3;
}
.orb-3 {
  width: 300px;
  height: 300px;
  background: #0ea5e9;
  top: 40%;
  left: 60%;
  opacity: 0.2;
}
.orb-4 {
  width: 250px;
  height: 250px;
  background: #a855f7;
  bottom: 20%;
  left: 10%;
  opacity: 0.2;
}

/* ── Grid ── */
.grid-overlay {
  position: absolute;
  inset: 0;
  background-image:
    linear-gradient(rgba(255, 255, 255, 0.025) 1px, transparent 1px),
    linear-gradient(90deg, rgba(255, 255, 255, 0.025) 1px, transparent 1px);
  background-size: 48px 48px;
  pointer-events: none;
}

/* ── Card ── */
.card {
  position: relative;
  z-index: 10;
  width: 100%;
  max-width: 500px;
  background: rgba(255, 255, 255, 0.04);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 24px;
  margin: 900rem;
  padding: 2.5rem 2rem;
  backdrop-filter: blur(24px);
  -webkit-backdrop-filter: blur(24px);
  box-shadow:
    0 0 0 1px rgba(139, 92, 246, 0.15),
    0 32px 80px rgba(0, 0, 0, 0.6),
    inset 0 1px 0 rgba(255, 255, 255, 0.08);
  animation: cardIn 0.7s cubic-bezier(0.16, 1, 0.3, 1) forwards;
  transition: box-shadow 0.5s ease;
}

.card--success {
  box-shadow:
    0 0 0 1px rgba(52, 211, 153, 0.3),
    0 32px 80px rgba(0, 0, 0, 0.6),
    inset 0 1px 0 rgba(255, 255, 255, 0.08);
}

@keyframes cardIn {
  from {
    opacity: 0;
    transform: translateY(32px) scale(0.97);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

/* ── Header ── */
.card__header {
  text-align: center;
  margin-bottom: 2rem;
}

.logo-ring {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 70px;
  height: 70px;
  border-radius: 50%;
  background: rgba(129, 140, 248, 0.1);
  border: 1px solid rgba(129, 140, 248, 0.25);
  animation: pulse-ring 3s ease-in-out infinite;
}

@keyframes pulse-ring {
  0%,
  100% {
    box-shadow: 0 0 0 0 rgba(129, 140, 248, 0.2);
  }
  50% {
    box-shadow: 0 0 0 12px rgba(129, 140, 248, 0);
  }
}

.logo-icon {
  width: 32px;
  height: 32px;
}

.card__title {
  font-family: 'Syne', sans-serif;
  font-size: 1.75rem;
  font-weight: 800;
  color: #f1f5f9;
  margin: 0 0 0.25rem;
  letter-spacing: -0.03em;
}

.card__subtitle {
  font-size: 0.8rem;
  color: rgba(148, 163, 184, 0.7);
  letter-spacing: 0.12em;
  text-transform: uppercase;
  margin: 0;
}

/* ── Fields ── */
.fields {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.field {
  display: flex;
  align-items: center;
  gap: 0;
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 12px;
  padding: 0 0.75rem;
  transition:
    border-color 0.2s,
    box-shadow 0.2s;
}

.field:focus-within {
  border-color: rgba(139, 92, 246, 0.5);
  box-shadow: 0 0 0 3px rgba(139, 92, 246, 0.1);
}

.field__icon {
  color: rgba(148, 163, 184, 0.5);
  font-size: 0.85rem;
  flex-shrink: 0;
  width: 24px;
}

.field__input {
  flex: 1;
  background: transparent !important;
  border: none !important;
  box-shadow: none !important;
  outline: none !important;
  color: #f1f5f9 !important;
  font-family: 'DM Sans', sans-serif !important;
  font-size: 0.95rem !important;
  padding: 0.85rem 0.5rem !important;
  width: 100%;
}

.field__input::placeholder {
  color: rgba(148, 163, 184, 0.4) !important;
}

/* ── QR Button ── */
.qr-btn {
  flex-shrink: 0;
  width: 32px;
  height: 32px;
  border-radius: 8px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  background: rgba(255, 255, 255, 0.06);
  color: rgba(148, 163, 184, 0.7);
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.8rem;
  transition: all 0.2s ease;
}

.qr-btn:hover {
  background: rgba(139, 92, 246, 0.2);
  border-color: rgba(139, 92, 246, 0.4);
  color: #a78bfa;
}

.qr-btn--active {
  background: rgba(239, 68, 68, 0.15);
  border-color: rgba(239, 68, 68, 0.3);
  color: #f87171;
}

/* ── Scanner ── */
.scanner-slide-enter-active {
  transition: all 0.35s cubic-bezier(0.16, 1, 0.3, 1);
}
.scanner-slide-leave-active {
  transition: all 0.25s ease;
}
.scanner-slide-enter-from {
  opacity: 0;
  transform: translateY(-10px) scale(0.98);
}
.scanner-slide-leave-to {
  opacity: 0;
  transform: translateY(-6px) scale(0.98);
}

.scanner-box {
  border-radius: 12px;
  overflow: hidden;
  background: rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(139, 92, 246, 0.2);
}

.scanner-hint {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  padding: 0.6rem;
  font-size: 0.78rem;
  color: #a78bfa;
  letter-spacing: 0.05em;
}

.scanner-dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: #a78bfa;
  animation: blink 1.2s ease-in-out infinite;
}

@keyframes blink {
  0%,
  100% {
    opacity: 1;
  }
  50% {
    opacity: 0.2;
  }
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
  width: 18px;
  height: 18px;
  border-color: #a78bfa;
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

/* ── Error ── */
.fade-enter-active,
.fade-leave-active {
  transition: all 0.25s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
  transform: translateY(-4px);
}

.error-msg {
  background: rgba(239, 68, 68, 0.1);
  border: 1px solid rgba(239, 68, 68, 0.25);
  border-radius: 10px;
  padding: 0.65rem 0.9rem;
  color: #fca5a5;
  font-size: 0.83rem;
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

/* ── Login Button ── */
.login-btn {
  margin-top: 1.25rem;
  width: 100%;

  border: none;
  border-radius: 12px;
  background: linear-gradient(135deg, #6d28d9, #4f46e5);
  color: white;
  font-family: 'Syne', sans-serif;
  font-size: 0.95rem;
  font-weight: 700;
  letter-spacing: 0.04em;
  cursor: pointer;
  position: relative;
  overflow: hidden;
  transition: all 0.3s ease;
  box-shadow: 0 4px 24px rgba(109, 40, 217, 0.4);
}

.login-btn::before {
  content: '';
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, rgba(255, 255, 255, 0.15), transparent);
  opacity: 0;
  transition: opacity 0.3s;
}

.login-btn:hover:not(:disabled)::before {
  opacity: 1;
}
.login-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 32px rgba(109, 40, 217, 0.55);
}

.login-btn:active:not(:disabled) {
  transform: translateY(0);
}

.login-btn:disabled {
  cursor: not-allowed;
}

.login-btn__content {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
}

.login-btn--loading {
  background: linear-gradient(135deg, #5b21b6, #4338ca);
}

.login-btn--success {
  background: linear-gradient(135deg, #059669, #0d9488) !important;
  box-shadow: 0 4px 24px rgba(5, 150, 105, 0.4) !important;
}

/* ── Spinner ── */
.spinner {
  display: inline-block;
  width: 20px;
  height: 20px;
  border: 2px solid rgba(255, 255, 255, 0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 0.7s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

/* ── Footer ── */
.card__footer {
  text-align: center;
  font-size: 0.72rem;
  color: rgba(148, 163, 184, 0.3);
  letter-spacing: 0.1em;
  text-transform: uppercase;
  margin: 1.25rem 0 0;
}

/* ── Mobile ── */
@media (max-width: 480px) {
  .card {
    padding: 2rem 1.5rem;
    border-radius: 20px;
    margin: 0;
  }

  .card__title {
    font-size: 1.5rem;
  }

  .logo-ring {
    width: 56px;
    height: 56px;
  }
  .logo-icon {
    width: 26px;
    height: 26px;
  }
  .scene {
    min-height: 100%;
    height: 100vh;
  }
}

@media (max-width: 360px) {
  .card {
    padding: 1.75rem 1.25rem;
  }
  .card__title {
    font-size: 1.35rem;
  }
}
</style>
