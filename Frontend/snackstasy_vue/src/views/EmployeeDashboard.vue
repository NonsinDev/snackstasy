<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { useEmployeeAuth } from '@/services/Authentification'
import { OrderPerStandId, UpdateOrderItems } from '@/services/Orders'
import type { GetOderById } from '@/model/UserData'
import Button from 'primevue/button'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Tooltip from 'primevue/tooltip'
import Dialog from 'primevue/dialog'

const router = useRouter()
const { employeeData } = useEmployeeAuth()
const orders = ref<any[]>([])
const isLoading = ref(true)
const error = ref('')
const selectedOrder = ref<any>(null)
const showOrderDetails = ref(false)

onMounted(async () => {
  if (!employeeData.value?.stand_id) {
    router.push('/employee-login')
    return
  }

  await fetchOrders()
})

async function fetchOrders() {
  try {
    isLoading.value = true
    error.value = ''
    const response = await OrderPerStandId(employeeData.value?.stand_id || 0)

    // Transformiere die Daten - die API gibt eine Liste von Objekten mit 'order' und 'items' zurück
    if (Array.isArray(response)) {
      orders.value = response.map((item: any) => ({
        ...item.order,
        items: item.items,
      }))
    } else if (response?.orders) {
      orders.value = response.orders
    } else {
      orders.value = []
    }
  } catch (err) {
    console.error('Error fetching orders:', err)
    error.value = 'Fehler beim Laden der Bestellungen'
    orders.value = []
  } finally {
    isLoading.value = false
  }
}

function showDetails(order: any) {
  selectedOrder.value = order
  showOrderDetails.value = true
}

async function markItemAsCollected(orderItemId: number) {
  try {
    await UpdateOrderItems(orderItemId)
    // Aktualisiere die Bestellungen nach Änderung
    await fetchOrders()
  } catch (err) {
    console.error('Error updating order item:', err)
    error.value = 'Fehler beim Aktualisieren des Items'
  }
}

function getStatusBadgeClass(status: string) {
  const statusMap: { [key: string]: string } = {
    preparing: 'badge-preparing',
    ready: 'badge-ready',
    completed: 'badge-completed',
    pending: 'badge-pending',
  }
  return statusMap[status] || 'badge-pending'
}

function getStatusLabel(status: string) {
  const labels: { [key: string]: string } = {
    preparing: 'In Zubereitung',
    ready: 'Bereit',
    completed: 'Abgeholt',
    pending: 'Ausstehend',
  }
  return labels[status] || status
}

async function logout() {
  localStorage.removeItem('employeeAuth')
  router.push('/login')
}
</script>

<template>
  <div class="employee-dashboard">
    <!-- Header -->
    <div class="dashboard-header">
      <div class="header-content">
        <h1>
          Bestellungen für Stand: <span class="stand-name">{{ employeeData?.stand_id }}</span>
        </h1>
        <p class="user-info">
          Angemeldet als: <strong>{{ employeeData?.username }}</strong>
        </p>
      </div>
      <button class="logout-btn" @click="logout">
        <i class="pi pi-sign-out"></i>
        Logout
      </button>
    </div>

    <!-- Loading State -->
    <div v-if="isLoading" class="loading-state">
      <i class="pi pi-spinner pi-spin"></i>
      <p>Bestellungen werden geladen...</p>
    </div>

    <!-- Error State -->
    <div v-if="error" class="error-message">
      <i class="pi pi-exclamation-circle"></i>
      {{ error }}
      <button class="retry-btn" @click="fetchOrders">Erneut versuchen</button>
    </div>

    <!-- Empty State -->
    <div v-if="!isLoading && orders.length === 0" class="empty-state">
      <i class="pi pi-inbox"></i>
      <p>Keine Bestellungen vorhanden</p>
    </div>

    <!-- Orders Table -->
    <div v-if="!isLoading && orders.length > 0" class="orders-container">
      <DataTable :value="orders" class="orders-table" stripedRows responsiveLayout="scroll">
        <Column field="order_id" header="Bestellungs-ID" style="width: 15%">
          <template #body="slotProps">
            <strong>#{{ slotProps.data.order_id }}</strong>
          </template>
        </Column>

        <Column field="user_id" header="Kunden-ID" style="width: 15%">
          <template #body="slotProps">
            {{ slotProps.data.user_id }}
          </template>
        </Column>

        <Column field="total_price" header="Gesamtpreis" style="width: 15%">
          <template #body="slotProps">
            <strong>{{ slotProps.data.total_price.toFixed(2) }}€</strong>
          </template>
        </Column>

        <Column field="status" header="Status" style="width: 20%">
          <template #body="slotProps">
            <span :class="['badge', getStatusBadgeClass(slotProps.data.status)]">
              {{ getStatusLabel(slotProps.data.status) }}
            </span>
          </template>
        </Column>

        <Column field="created_at" header="Zeitstempel" style="width: 20%">
          <template #body="slotProps">
            {{ new Date(slotProps.data.created_at).toLocaleString('de-DE') }}
          </template>
        </Column>

        <Column header="Aktionen" style="width: 15%">
          <template #body="slotProps">
            <button
              class="detail-btn"
              @click="showDetails(slotProps.data)"
              v-tooltip.top="'Details anzeigen'"
            >
              <i class="pi pi-eye"></i>
            </button>
          </template>
        </Column>
      </DataTable>
    </div>

    <!-- Order Details Dialog -->
    <Dialog
      v-model:visible="showOrderDetails"
      :header="`Bestellung #${selectedOrder?.order_id}`"
      :modal="true"
      class="order-dialog"
      style="width: 90vw; max-width: 600px"
    >
      <div v-if="selectedOrder" class="order-details">
        <div class="order-info-section">
          <h3>Bestellinformationen</h3>
          <div class="info-grid">
            <div class="info-item">
              <label>Bestellungs-ID:</label>
              <span>#{{ selectedOrder.order_id }}</span>
            </div>
            <div class="info-item">
              <label>Kunden-ID:</label>
              <span>{{ selectedOrder.user_id }}</span>
            </div>
            <div class="info-item">
              <label>Status:</label>
              <span :class="['badge', getStatusBadgeClass(selectedOrder.status)]">
                {{ getStatusLabel(selectedOrder.status) }}
              </span>
            </div>
            <div class="info-item">
              <label>Zeitstempel:</label>
              <span>{{ new Date(selectedOrder.created_at).toLocaleString('de-DE') }}</span>
            </div>
          </div>
        </div>

        <div class="items-section">
          <h3>Bestellte Artikel</h3>
          <div v-if="selectedOrder.items && selectedOrder.items.length > 0" class="items-list">
            <div v-for="item in selectedOrder.items" :key="item.order_item_id" class="item-card">
              <div class="item-info">
                <div class="item-name">{{ item.item_id }} x {{ item.quantity }}</div>
                <div class="item-price">{{ item.price.toFixed(2) }}€</div>
              </div>
              <div class="item-status">
                <span :class="['item-badge', item.is_collected ? 'collected' : 'pending']">
                  {{ item.is_collected ? 'Abgeholt' : 'Ausstehend' }}
                </span>
                <button
                  v-if="!item.is_collected"
                  class="collect-btn"
                  @click="markItemAsCollected(item.order_item_id)"
                >
                  <i class="pi pi-check"></i>
                  Abholen
                </button>
              </div>
            </div>
          </div>
          <div v-else class="no-items">Keine Artikel gefunden</div>
        </div>

        <div class="total-section">
          <strong>Gesamtbetrag: {{ selectedOrder.total_price.toFixed(2) }}€</strong>
        </div>
      </div>
    </Dialog>
  </div>
</template>

<style scoped>
.employee-dashboard {
  min-height: 100vh;
  background: #f5f5f5;
  padding: 2rem 1rem;
}

.dashboard-header {
  max-width: 1200px;
  margin: 0 auto 2rem;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: white;
  padding: 1.5rem 2rem;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

.header-content h1 {
  margin: 0 0 0.5rem;
  font-size: 1.5rem;
  color: #333;
}

.stand-name {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.user-info {
  margin: 0;
  font-size: 0.9rem;
  color: #666;
}

.logout-btn {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 8px;
  cursor: pointer;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  transition: all 0.2s;
}

.logout-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3);
}

.loading-state,
.empty-state {
  max-width: 1200px;
  margin: 0 auto;
  background: white;
  padding: 3rem 2rem;
  border-radius: 12px;
  text-align: center;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
}

.loading-state i,
.empty-state i {
  font-size: 3rem;
  color: #667eea;
  display: block;
  margin-bottom: 1rem;
}

.loading-state .pi-spinner {
  animation: spin 1s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

.error-message {
  max-width: 1200px;
  margin: 0 auto 2rem;
  background: #ffe8e8;
  border: 1px solid #ffb3b3;
  border-radius: 10px;
  padding: 1rem;
  color: #d32f2f;
  display: flex;
  align-items: center;
  gap: 1rem;
}

.retry-btn {
  background: #d32f2f;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 600;
  margin-left: auto;
}

.orders-container {
  max-width: 1200px;
  margin: 0 auto;
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
  overflow: hidden;
}

.orders-table {
  width: 100%;
}

:deep(.p-datatable) {
  border: none;
}

:deep(.p-datatable .p-datatable-thead > tr > th) {
  background: #f9f9f9;
  border: none;
  padding: 1rem;
  font-weight: 600;
  color: #333;
}

:deep(.p-datatable .p-datatable-tbody > tr > td) {
  padding: 1rem;
  border-bottom: 1px solid #e5e5e5;
}

.badge {
  display: inline-block;
  padding: 0.4rem 0.8rem;
  border-radius: 20px;
  font-size: 0.75rem;
  font-weight: 600;
}

.badge-preparing {
  background: #fff3cd;
  color: #856404;
}

.badge-ready {
  background: #d1ecf1;
  color: #0c5460;
}

.badge-completed {
  background: #d4edda;
  color: #155724;
}

.badge-pending {
  background: #f8f9fa;
  color: #383d41;
}

.detail-btn {
  background: transparent;
  border: 1px solid #667eea;
  color: #667eea;
  padding: 0.5rem 0.75rem;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s;
}

.detail-btn:hover {
  background: #f5f7ff;
}

.order-details {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.order-info-section,
.items-section {
  padding: 1.5rem;
  background: #f9f9f9;
  border-radius: 8px;
}

.order-info-section h3,
.items-section h3 {
  margin: 0 0 1rem;
  color: #333;
  font-size: 1rem;
}

.info-grid {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1rem;
}

.info-item {
  display: flex;
  flex-direction: column;
}

.info-item label {
  font-size: 0.75rem;
  color: #999;
  text-transform: uppercase;
  margin-bottom: 0.25rem;
}

.info-item span {
  color: #333;
  font-weight: 500;
}

.items-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.item-card {
  background: white;
  padding: 1rem;
  border-radius: 8px;
  border: 1px solid #e5e5e5;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.item-info {
  flex: 1;
}

.item-name {
  font-weight: 600;
  color: #333;
  margin-bottom: 0.25rem;
}

.item-price {
  font-size: 0.85rem;
  color: #666;
}

.item-status {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.item-badge {
  display: inline-block;
  padding: 0.3rem 0.6rem;
  border-radius: 4px;
  font-size: 0.75rem;
  font-weight: 600;
}

.item-badge.pending {
  background: #ffe0b2;
  color: #e65100;
}

.item-badge.collected {
  background: #c8e6c9;
  color: #2e7d32;
}

.collect-btn {
  background: #667eea;
  color: white;
  border: none;
  padding: 0.4rem 0.8rem;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.75rem;
  font-weight: 600;
  transition: all 0.2s;
}

.collect-btn:hover {
  background: #5568d3;
}

.no-items {
  text-align: center;
  padding: 2rem;
  color: #999;
}

.total-section {
  padding: 1rem;
  background: linear-gradient(135deg, #f5f7ff 0%, #eef0ff 100%);
  border-radius: 8px;
  text-align: right;
  font-size: 1.1rem;
  color: #333;
}

@media (max-width: 768px) {
  .dashboard-header {
    flex-direction: column;
    gap: 1rem;
    align-items: flex-start;
  }

  .info-grid {
    grid-template-columns: 1fr;
  }

  .item-card {
    flex-direction: column;
    align-items: flex-start;
  }

  .item-status {
    width: 100%;
    margin-top: 0.75rem;
    justify-content: space-between;
  }
}
</style>
