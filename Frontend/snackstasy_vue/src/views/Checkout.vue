<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useCartStore } from '@/stores/PiniaStore'
import { useFoodStore } from '@/stores/foodStore'
import { RemoveBalance, UserData } from '@/services/Header'
import { checkSession } from '@/services/Login'
import { createOrder } from '@/services/Orders'
import type { Login_response } from '@/model/AuthentificationInterface'
import type { User_Data } from '@/model/UserData'
import type { ItemsByStand } from '@/model/Items'
import type { Order } from '@/model/UserData'
import { ref, computed } from 'vue'

const router = useRouter()
const cartStore = useCartStore()
const foodStore = useFoodStore()
const isProcessing = ref(false)
const sessionData = ref<Login_response | null>(null)
const userData = ref<User_Data | null>(null)
const itemQuantities = ref<Map<number, number>>(new Map())

// Gruppierte Items mit Anzahl
const groupedItems = computed(() => {
  const groups = new Map<number, { item: ItemsByStand; quantity: number }>()

  cartStore.items.forEach((item) => {
    const itemId = item.item_id
    if (groups.has(itemId)) {
      const group = groups.get(itemId)!
      group.quantity += 1
    } else {
      groups.set(itemId, { item, quantity: 1 })
    }
  })

  return Array.from(groups.values())
})

// Berechne Gesamtpreis basierend auf neuen Mengen
const totalPrice = computed(() => {
  return groupedItems.value.reduce((sum, group) => sum + group.item.price * group.quantity, 0)
})

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
const removeItem = (itemId: number) => {
  // Entferne alle Instanzen dieses Items
  cartStore.items = cartStore.items.filter((item) => item.item_id !== itemId)
}

// Menge aktualisieren
const updateQuantity = (itemId: number, newQuantity: number) => {
  if (newQuantity <= 0) {
    removeItem(itemId)
    return
  }

  // Finde aktuelle Menge
  const currentQuantity = cartStore.items.filter((item) => item.item_id === itemId).length

  if (newQuantity > currentQuantity) {
    // Füge Items hinzu
    const item = cartStore.items.find((item) => item.item_id === itemId)
    if (item) {
      for (let i = 0; i < newQuantity - currentQuantity; i++) {
        cartStore.addItem({ ...item })
      }
    }
  } else if (newQuantity < currentQuantity) {
    // Entferne Items
    let removed = 0
    for (
      let i = cartStore.items.length - 1;
      i >= 0 && removed < currentQuantity - newQuantity;
      i--
    ) {
      const currentItem = cartStore.items[i]
      if (currentItem?.item_id === itemId) {
        cartStore.items.splice(i, 1)
        removed++
      }
    }
  }
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
    const totalPriceValue = totalPrice.value

    console.log('Guthaben wird abgezogen für User:', userId, 'Betrag:', totalPriceValue)

    await RemoveBalance(userId, totalPriceValue)

    console.log('Balance erfolgreich abgezogen')

    // 2️⃣ Bestellung in der DB erstellen
    // Items gruppieren: itemId -> Menge zählen
    const itemCounts = new Map<number, number>()
    cartStore.items.forEach((item) => {
      itemCounts.set(item.item_id, (itemCounts.get(item.item_id) || 0) + 1)
    })

    // Order-Objekt für API erstellen
    const orderData: Order = {
      user_id: userId,
      stand_id: foodStore.currentStand.stand_id,
      items: Array.from(itemCounts).map(([item_id, quantity]) => ({
        item_id,
        quantity,
      })),
    }

    console.log('Order wird gesendet:', orderData)
    const orderResponse = await createOrder(orderData)
    console.log('Order erfolgreich erstellt:', orderResponse)

    // 3️⃣ Lokal Order im Store erstellen (mit der DB order_id)
    cartStore.createOrder(
      foodStore.currentStand.pickup_id,
      foodStore.currentStand.name.toString(),
      orderResponse.order_id,
    )

    console.log('Bestellung erstellt:', cartStore.orderDetails)

    // 4️⃣ Warenkorb leeren
    cartStore.clearCart()

    // 5️⃣ Zur Order-Status Seite navigieren
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
        <div v-for="group in groupedItems" :key="group.item.item_id" class="item">
          <div class="item-info">
            <h3>{{ group.item.name }}</h3>
            <p>{{ group.item.price.toFixed(2) }}€ pro Stück</p>
          </div>
          <div class="item-controls">
            <div class="quantity-control">
              <button
                class="qty-btn"
                @click="updateQuantity(group.item.item_id, group.quantity - 1)"
              >
                −
              </button>
              <input
                type="number"
                class="qty-input"
                :value="group.quantity"
                @input="
                  (e) =>
                    updateQuantity(
                      group.item.item_id,
                      parseInt((e.target as HTMLInputElement).value) || 1,
                    )
                "
                min="1"
              />
              <button
                class="qty-btn"
                @click="updateQuantity(group.item.item_id, group.quantity + 1)"
              >
                +
              </button>
            </div>
            <div class="item-total">{{ (group.item.price * group.quantity).toFixed(2) }}€</div>
            <button class="remove-btn" @click="removeItem(group.item.item_id)">✕</button>
          </div>
        </div>
      </div>

      <!-- Summary -->
      <div class="summary">
        <h2>Gesamt</h2>
        <p>{{ totalPrice.toFixed(2) }}€</p>
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
  gap: 15px;
}

.item-info {
  flex: 1;
}

.item-info h3 {
  margin: 0;
  font-size: 16px;
}

.item-info p {
  margin: 5px 0 0 0;
  color: #999;
  font-size: 14px;
}

.item-controls {
  display: flex;
  align-items: center;
  gap: 12px;
}

.quantity-control {
  display: flex;
  align-items: center;
  gap: 5px;
  background: #0f0f0f;
  padding: 4px 8px;
  border-radius: 8px;
  border: 1px solid #333;
}

.qty-btn {
  background: none;
  border: none;
  color: #b6ff3b;
  cursor: pointer;
  font-size: 16px;
  font-weight: bold;
  width: 24px;
  height: 24px;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
}

.qty-btn:hover {
  background: #b6ff3b;
  color: #0f0f0f;
  border-radius: 4px;
}

.qty-input {
  width: 40px;
  background: #0f0f0f;
  border: none;
  color: white;
  text-align: center;
  font-weight: bold;
  font-size: 14px;
}

.qty-input::-webkit-outer-spin-button,
.qty-input::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

.qty-input[type='number'] {
  -moz-appearance: textfield;
}

.item-total {
  min-width: 60px;
  text-align: right;
  font-weight: bold;
  color: #b6ff3b;
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
