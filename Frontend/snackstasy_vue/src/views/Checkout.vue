<script setup lang="ts">
import { useRouter } from 'vue-router'
import { useCartStore } from '@/stores/PiniaStore'

const router = useRouter()
const cartStore = useCartStore()

// Navigation
const goBack = () => {
  router.back()
}

// 🧾 Checkout vorbereiten
const prepareCheckout = () => {
  // TODO: Daten validieren / vorbereiten
}

// 💳 Kaufen (API kommt später)
const buyItems = async () => {
  try {
    prepareCheckout()

    // TODO: API CALL EINBAUEN
    // await checkoutApi(cartStore.items)

    console.log('Kauf ausgelöst:', cartStore.items)

    // nach Kauf leeren
    cartStore.clearCart()

    router.push('/success') // optional
  } catch (err) {
    console.error('Fehler beim Kauf:', err)
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
        </div>
      </div>

      <!-- Summary -->
      <div class="summary">
        <h2>Gesamt</h2>
        <p>{{ cartStore.totalPrice.toFixed(2) }}€</p>
      </div>

      <!-- Buy Button -->
      <button class="buy-btn" @click="buyItems">Jetzt kaufen</button>
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
}
</style>
