<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useCartStore } from '@/stores/PiniaStore'
import { useFoodStore } from '@/stores/foodStore'
import { RemoveBalance, UserData } from '@/services/Header'
import { checkSession } from '@/services/Login'
import type { Login_response } from '@/model/AuthentificationInterface'
import type { User_Data } from '@/model/UserData'
import { ref } from 'vue'

const router = useRouter()
const cartStore = useCartStore()
const foodStore = useFoodStore()
const isProcessing = ref(false)
const sessionData = ref<Login_response | null>(null)
const userData = ref<User_Data | null>(null)

// Session und User-Daten laden
const loadUserData = async () => {
  try {
    const session = await checkSession()
    sessionData.value = session

    if (session?.ticket_id) {
      userData.value = await UserData(session.ticket_id)
      console.log('User-Daten geladen:', userData.value)
    }
  } catch (err) {
    console.error('Fehler beim Laden der Daten:', err)
  }
}

loadUserData()

// Navigation
const goBack = () => {
  router.back()
}

// Item entfernen
const removeItem = (index: number) => {
  cartStore.removeItem(index)
}

// 🧾 Checkout vorbereiten
const prepareCheckout = () => {
  // TODO: Daten validieren / vorbereiten
}

// 💳 Kaufen
const buyItems = async () => {
  console.log('buyItems aufgerufen')
  console.log('userData:', userData.value)
  console.log('foodStore.currentStand:', foodStore.currentStand)

  if (!userData.value) {
    alert('Fehler: Benutzerdaten nicht gefunden. Bitte melde dich erneut an.')
    return
  }

  if (!foodStore.currentStand) {
    alert('Fehler: Stand nicht gefunden. Bitte wähle einen Stand aus.')
    return
  }

  try {
    isProcessing.value = true

    prepareCheckout()

    // 1️⃣ Balance abziehen
    const userId = userData.value.user_id
    const totalPrice = cartStore.totalPrice

    console.log('Guthaben wird abgezogen für User:', userId, 'Betrag:', totalPrice)

    await RemoveBalance(userId, totalPrice)

    console.log('Balance erfolgreich abgezogen')

    // 2️⃣ Bestellung erstellen
    cartStore.createOrder(foodStore.currentStand.pickup_id, foodStore.currentStand.name.toString())

    console.log('Bestellung erstellt:', cartStore.orderDetails)

    // 3️⃣ Warenkorb leeren
    cartStore.clearCart()

    // 4️⃣ Zur Order-Status Seite navigieren
    router.push('/order-status')
  } catch (err: any) {
    console.error('Fehler beim Kauf:', err)
    console.error('Fehlerdetails:', err.response?.data || err.message)

    let errorMessage = 'Fehler beim Verarbeiten der Bestellung. Bitte versuche es später erneut.'

    if (err.response?.status === 400) {
      errorMessage = 'Ungültige Anfrage. Bitte überprüfe deine Daten.'
    } else if (err.response?.status === 401) {
      errorMessage = 'Authentifizierung erforderlich. Bitte melde dich erneut an.'
    } else if (err.response?.status === 402) {
      errorMessage = 'Unzureichendes Guthaben!'
    } else if (err.message?.includes('Network')) {
      errorMessage = 'Netzwerkfehler. Bitte überprüfe deine Verbindung.'
    }

    alert(errorMessage)
  } finally {
    isProcessing.value = false
  }
}
</script>

<template>
  <div class="checkout-container">
    <!-- Header -->
    <div class="header">
      <button class="back-btn" @click="goBack">← Zurück</button>
      <h1>Checkout</h1>
    </div>

    <!-- Empty -->
    <div v-if="cartStore.items.length === 0" class="empty">
      <p>Dein Warenkorb ist leer</p>
    </div>

    <!-- Items -->
    <div v-else class="content">
      <div class="items">
        <div v-for="(item, index) in cartStore.items" :key="index" class="item">
          <div>
            <h3>{{ item.name }}</h3>
            <p>{{ item.price.toFixed(2) }}€</p>
          </div>
          <button class="remove-btn" @click="removeItem(index)">✕</button>
        </div>
      </div>

      <!-- Summary -->
      <div class="summary">
        <h2>Gesamt</h2>
        <p>{{ cartStore.totalPrice.toFixed(2) }}€</p>
      </div>

      <!-- Buy Button -->
      <button class="buy-btn" @click="buyItems" :disabled="isProcessing">
        {{ isProcessing ? 'Wird verarbeitet...' : 'Jetzt kaufen' }}
      </button>
    </div>
  </div>
</template>

<style scoped>
.checkout-container {
  min-height: 100vh;
  background: #0f0f0f;
  color: white;
  padding: 20px;
}

.header {
  display: flex;
  align-items: center;
  gap: 20px;
}

.back-btn {
  background: none;
  border: 1px solid #b6ff3b;
  color: #b6ff3b;
  padding: 6px 10px;
  border-radius: 8px;
  cursor: pointer;
}

.items {
  margin-top: 20px;
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.item {
  background: #1a1a1a;
  padding: 15px;
  border-radius: 12px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.remove-btn {
  background: #ef4444;
  border: none;
  color: white;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  cursor: pointer;
  font-size: 18px;
  font-weight: bold;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.remove-btn:hover {
  background: #dc2626;
  transform: scale(1.1);
}

.remove-btn:active {
  transform: scale(0.95);
}

.summary {
  margin-top: 30px;
  font-size: 20px;
  font-weight: bold;
}

.buy-btn {
  margin-top: 20px;
  width: 100%;

  padding: 15px;
  border-radius: 12px;
  border: none;

  background: linear-gradient(135deg, #b6ff3b, #4caf50);
  color: black;
  font-weight: bold;
  font-size: 16px;

  cursor: pointer;
  transition: all 0.2s ease;
}

.buy-btn:hover:not(:disabled) {
  transform: scale(1.02);
  box-shadow: 0 4px 12px rgba(76, 175, 80, 0.3);
}

.buy-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}
</style>
