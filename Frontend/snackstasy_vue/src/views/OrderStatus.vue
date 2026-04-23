<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useCartStore } from '@/stores/PiniaStore'
import { OrderById } from '@/services/Orders'
import { ItemsByStandId, StandById } from '@/services/Stands'
// @ts-ignore
import QRCode from 'qrcode'

const router = useRouter()
const route = useRoute()
const cartStore = useCartStore()

const qrCodeDataUrl = ref<string>('')
const autoStatusUpdate = ref(false)
const orderFromDb = ref<any>(null)
const isLoading = ref(true)
const itemsWithNames = ref<any[]>([])
const standData = ref<any>(null)

const getOrderId = (): number | null => {
  const routeOrderId = route.params.order_id
  if (routeOrderId) {
    return parseInt(routeOrderId as string)
  }
  return cartStore.orderDetails?.order_id || null
}

const getOrderData = () => {
  if (orderFromDb.value) {
    return orderFromDb.value
  }
  return cartStore.orderDetails
}

const loadItemNames = async (stand_id: number) => {
  try {
    const items = await ItemsByStandId(stand_id)
    const itemMap = new Map(items.map((item) => [item.item_id, item.name]))

    itemsWithNames.value = orderFromDb.value.items.map((orderItem: any) => ({
      ...orderItem,
      name: itemMap.get(orderItem.item_id) || `Item ${orderItem.item_id}`,
    }))
    console.log('Items mit Namen:', itemsWithNames.value)
  } catch (err) {
    console.error('Fehler beim Laden der Item-Namen:', err)
  }
}

const loadStandData = async (stand_id: number) => {
  try {
    standData.value = await StandById(stand_id)
    console.log('Stand-Daten geladen:', standData.value)
  } catch (err) {
    console.error('Fehler beim Laden der Stand-Daten:', err)
  }
}

onMounted(async () => {
  const orderId = getOrderId()

  if (orderId) {
    try {
      console.log('Lade Order von DB mit ID:', orderId)
      const dbResponse = await OrderById(orderId)
      orderFromDb.value = dbResponse

      if (dbResponse.order?.stand_id) {
        await loadItemNames(dbResponse.order.stand_id)
        await loadStandData(dbResponse.order.stand_id)
      }
      console.log('Order von DB geladen:', orderFromDb.value)
    } catch (err) {
      console.error('Fehler beim Laden der Order:', err)
    }
  } else if (!cartStore.orderDetails) {
    router.push('/')
    return
  }

  isLoading.value = false

  const orderData = getOrderData()
  if (orderData) {
    try {
      const itemIds = orderData.items?.map((item: any) => item.item_id).join(',') || ''
      qrCodeDataUrl.value = await QRCode.toDataURL(itemIds, {
        errorCorrectionLevel: 'H',
        type: 'image/png',
        width: 200,
        margin: 1,
        color: {
          dark: '#000000',
          light: '#ffffff',
        },
      })
    } catch (err) {
      console.error('Fehler beim Generieren des QR-Codes:', err)
    }
  }

  if (autoStatusUpdate.value && cartStore.orderDetails) {
    setTimeout(() => {
      cartStore.updateOrderStatus('ready')
    }, 3000)
  }
})

const statusClass = computed(() => {
  const orderData = getOrderData()
  if (!orderData) return ''
  const status = orderData.orderStatus || orderData.order?.status || orderData.status
  return `status-${status}`
})

const statusText = computed(() => {
  const orderData = getOrderData()
  if (!orderData) return ''
  const status = orderData.orderStatus || orderData.order?.status || orderData.status
  const statuses: Record<string, string> = {
    pending: 'Ausstehend',
    preparing: 'In Zubereitung',
    ready: 'Bereit zur Abholung',
    completed: 'Abgeholt',
    cancelled: 'Storniert',
  }
  return statuses[status] || 'Unbekannt'
})

const getItems = () => {
  if (itemsWithNames.value.length > 0) {
    return itemsWithNames.value
  }
  if (orderFromDb.value?.items) {
    return orderFromDb.value.items
  }
  return cartStore.orderDetails?.items || []
}

const getTotalPrice = () => {
  if (orderFromDb.value?.order?.total_price) {
    return orderFromDb.value.order.total_price
  }
  return cartStore.orderDetails?.totalPrice || 0
}

const goBack = () => {
  router.push('/')
  cartStore.clearOrder()
}

const copyPickupId = () => {
  const orderData = getOrderData()
  if (orderData) {
    const pickupId = orderData.pickupId || orderData.pickup_id || 'N/A'
    navigator.clipboard.writeText(pickupId.toString())
    alert('Abhol-ID kopiert!')
  }
}
</script>

<template>
  <div v-if="!isLoading" class="order-status-container">
    <!-- Header -->
    <div class="header">
      <h1>🎉 Bestellung aufgegeben</h1>
      <p>Deine Bestellung wird zubereitet</p>
    </div>

    <!-- Status Badge -->
    <div :class="['status-badge', statusClass]">
      <div class="status-indicator"></div>
      <span>{{ statusText }}</span>
    </div>

    <!-- Order Details -->
    <div class="details-section">
      <h2>Bestelldetails</h2>

      <!-- Order ID -->
      <div class="detail-item">
        <label>Bestellnummer:</label>
        <span>#{{ orderFromDb?.order?.order_id || getOrderData()?.order_id }}</span>
      </div>

      <!-- Stand Info -->
      <div class="detail-item">
        <label>Stand:</label>
        <span>{{ standData?.name || getOrderData()?.standName || 'N/A' }}</span>
      </div>

      <!-- Pickup ID -->
      <div class="detail-item pickup-id">
        <label>Abhol-ID:</label>
        <div class="pickup-display">
          <span class="pickup-number">{{ standData?.pickup_id || 'N/A' }}</span>
          <button class="copy-btn" @click="copyPickupId" title="Kopieren">📋</button>
        </div>
      </div>

      <!-- Gesamtpreis -->
      <div class="detail-item">
        <label>Gesamtpreis:</label>
        <span class="price">{{ getTotalPrice().toFixed(2) }}€</span>
      </div>

      <!-- Zeit -->
      <div class="detail-item">
        <label>Bestellung:</label>
        <span>{{
          getOrderData()?.timestamp?.toLocaleTimeString('de-DE') ||
          new Date(orderFromDb?.order?.created_at).toLocaleString('de-DE')
        }}</span>
      </div>
    </div>

    <!-- Items List -->
    <div class="items-section">
      <h2>Bestellte Artikel</h2>
      <div class="items-list">
        <div v-for="item in getItems()" :key="item.item_id" class="item">
          <div class="item-details">
            <span class="item-name">{{ item.name || 'Item' }} x{{ item.quantity || 1 }}</span>
            <span class="item-price">{{ (item.price || 0).toFixed(2) }}€</span>
          </div>
        </div>
      </div>
    </div>

    <!-- QR Code -->
    <div class="qr-section">
      <h2>QR-Code zum Abholen</h2>
      <img v-if="qrCodeDataUrl" :src="qrCodeDataUrl" alt="QR-Code" class="qr-code" />
      <p class="qr-hint">Zeige diesen QR-Code beim Abholen</p>
    </div>

    <!-- Action Buttons -->
    <div class="actions">
      <button class="back-btn" @click="goBack">← Zur Startseite</button>
    </div>
  </div>
  <div v-else class="error-container">
    <p>Lade Bestellung...</p>
    <button class="back-btn" @click="router.push('/')">Zur Startseite</button>
  </div>
</template>

<style scoped>
.order-status-container {
  min-height: 100vh;
  background: linear-gradient(135deg, #1a1a1a 0%, #2d2d2d 100%);
  color: white;
  padding: 20px;
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.error-container {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100vh;
  flex-direction: column;
  gap: 20px;
}

/* Header */
.header {
  text-align: center;
  margin-bottom: 20px;
}

.header h1 {
  margin: 0;
  font-size: 28px;
  background: linear-gradient(135deg, #ffd700, #ffed4e);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.header p {
  margin: 10px 0 0;
  font-size: 16px;
  color: #b0b0b0;
}

/* Status Badge */
.status-badge {
  display: flex;
  align-items: center;
  gap: 12px;
  padding: 16px 20px;
  border-radius: 12px;
  font-size: 18px;
  font-weight: bold;
  text-align: center;
  justify-content: center;
  margin: 0 auto;
}

.status-indicator {
  width: 12px;
  height: 12px;
  border-radius: 50%;
  animation: pulse 2s infinite;
}

.status-badge.status-preparing {
  background: rgba(255, 193, 7, 0.1);
  border: 2px solid #ffc107;
  color: #ffc107;
}

.status-badge.status-preparing .status-indicator {
  background: #ffc107;
}

.status-badge.status-ready {
  background: rgba(76, 175, 80, 0.1);
  border: 2px solid #4caf50;
  color: #4caf50;
}

.status-badge.status-ready .status-indicator {
  background: #4caf50;
}

.status-badge.status-completed {
  background: rgba(33, 150, 243, 0.1);
  border: 2px solid #2196f3;
  color: #2196f3;
}

.status-badge.status-completed .status-indicator {
  background: #2196f3;
}

@keyframes pulse {
  0%,
  100% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
}

/* Details Section */
.details-section,
.items-section,
.qr-section {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 215, 0, 0.2);
  padding: 20px;
  border-radius: 12px;
}

.details-section h2,
.items-section h2,
.qr-section h2 {
  margin: 0 0 15px;
  font-size: 18px;
  color: #ffd700;
}

/* Detail Items */
.detail-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px 0;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.detail-item:last-child {
  border-bottom: none;
}

.detail-item label {
  font-weight: 600;
  color: #b0b0b0;
}

.detail-item span {
  color: white;
  font-weight: 500;
}

.detail-item.pickup-id {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.pickup-display {
  display: flex;
  align-items: center;
  gap: 10px;
}

.pickup-number {
  font-size: 20px;
  font-weight: bold;
  color: #ffd700;
  font-family: monospace;
  letter-spacing: 2px;
}

.copy-btn {
  background: rgba(255, 215, 0, 0.2);
  border: 1px solid #ffd700;
  color: #ffd700;
  width: 32px;
  height: 32px;
  border-radius: 6px;
  cursor: pointer;
  font-size: 16px;
  transition: all 0.2s ease;
}

.copy-btn:hover {
  background: rgba(255, 215, 0, 0.3);
  transform: scale(1.05);
}

.price {
  color: #4caf50;
  font-weight: bold;
  font-size: 16px;
}

/* Items List */
.items-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px;
  background: rgba(0, 0, 0, 0.3);
  border-radius: 8px;
}

.item-details {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  gap: 15px;
}

.item-name {
  color: white;
  flex: 1;
}

.item-price {
  color: #ffd700;
  font-weight: bold;
  white-space: nowrap;
}

/* QR Section */
.qr-section {
  text-align: center;
}

.qr-code {
  width: 200px;
  height: 200px;
  margin: 20px auto;
  padding: 15px;
  background: white;
  border-radius: 12px;
  display: block;
}

.qr-hint {
  font-size: 14px;
  color: #b0b0b0;
  margin: 0;
}

/* Actions */
.actions {
  display: flex;
  gap: 10px;
  margin-top: 20px;
}

.back-btn {
  flex: 1;
  padding: 15px;
  border-radius: 12px;
  border: 1px solid #ffd700;
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.1), rgba(255, 215, 0, 0.05));
  color: #ffd700;
  font-weight: bold;
  cursor: pointer;
  transition: all 0.2s ease;
}

.back-btn:hover {
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.2), rgba(255, 215, 0, 0.1));
  transform: translateY(-2px);
}

/* Mobile */
@media (max-width: 600px) {
  .order-status-container {
    padding: 15px;
  }

  .header h1 {
    font-size: 24px;
  }

  .qr-code {
    width: 150px;
    height: 150px;
  }
}
</style>
