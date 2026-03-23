<script setup lang="ts">
import { ref } from 'vue'
import profileManImage from '@/assets/Profil_man.png'
import profileWomanImage from '@/assets/Profil_woman.png'
import type { User_Data } from '@/model/UserData'

const props = defineProps<{
  currentImage: string
  currentUser: User_Data | undefined
}>()

const emit = defineEmits<{
  updateProfileImage: [newImage: string]
  switchToBalance: []
}>()

// Balance / Rechnungen - wird vom Header übergeben
const topUps = ref([
  { id: 1, date: '10.03.2026', amount: 10.0, method: 'Kreditkarte' },
  { id: 2, date: '05.03.2026', amount: 20.0, method: 'PayPal' },
  { id: 3, date: '28.02.2026', amount: 15.5, method: 'Kreditkarte' },
])

// Bestellungen
const orders = ref([
  { id: 1, date: '13.03.2026', items: 'Pizza, Cola', price: 12.5, status: 'Abgeholt' },
  { id: 2, date: '12.03.2026', items: 'Burrito, Wasser', price: 8.99, status: 'Abgeholt' },
  { id: 3, date: '11.03.2026', items: 'Sandwich', price: 7.5, status: 'Abgeholt' },
])

const toggleProfileImage = () => {
  const newImage = props.currentImage === profileManImage ? profileWomanImage : profileManImage
  emit('updateProfileImage', newImage)
}

const handleBalanceClick = () => {
  emit('switchToBalance')
}
</script>

<template>
  <div class="dialog-container">
    <div class="profile-card">
      <!-- Profile Picture Section -->
      <div class="profile-picture-section">
        <div class="profile-picture-wrapper">
          <img :src="currentImage" alt="Profile" class="profile-picture" />
          <button class="toggle-btn" @click="toggleProfileImage" title="Profilbild wechseln">
            <span class="toggle-icon">⟲</span>
          </button>
        </div>
      </div>

      <!-- User Information Section -->
      <div class="user-information">
        <div class="info-card">
          <h2 class="info-title">Benutzerinformation</h2>

          <div class="info-item">
            <label class="label">Name</label>
            <div class="value-display">
              {{ props.currentUser?.first_name }} {{ props.currentUser?.last_name }}
            </div>
          </div>

          <div class="info-item">
            <label class="label">Ticket ID</label>
            <div class="value-display ticket-id">{{ props.currentUser?.ticket_id }}</div>
          </div>
        </div>
      </div>
      <!-- User Bills section -->
      <div class="user-information">
        <div class="info-card">
          <h2 class="info-title">Guthaben</h2>

          <div class="balance-display" @click="handleBalanceClick">
            <div class="balance-amount" v-if="props.currentUser">
              {{ props.currentUser.balance.toFixed(2) }} €
            </div>
            <p class="balance-label">Aktueller Kontostand</p>
          </div>

          <div class="topups-section">
            <h3 class="section-title">Aufladungshistorie</h3>
            <div v-if="topUps.length > 0" class="topups-list">
              <div v-for="topup in topUps" :key="topup.id" class="topup-item">
                <div class="topup-header">
                  <span class="topup-date">{{ topup.date }}</span>
                  <span class="topup-amount">+{{ topup.amount.toFixed(2) }} €</span>
                </div>
                <div class="topup-method">
                  <span class="method-label">{{ topup.method }}</span>
                </div>
              </div>
            </div>
            <div v-else class="no-topups">
              <p>Keine Aufladungen vorhanden</p>
            </div>
          </div>
        </div>
      </div>
      <!-- User Orders Section -->
      <div class="user-information">
        <div class="info-card">
          <h2 class="info-title">Bestellungen</h2>

          <div v-if="orders.length > 0" class="orders-list">
            <div v-for="order in orders" :key="order.id" class="order-item">
              <div class="order-header">
                <span class="order-date">{{ order.date }}</span>
                <span class="order-status">{{ order.status }}</span>
              </div>
              <div class="order-details">
                <p class="order-items">{{ order.items }}</p>
              </div>
              <div class="order-footer">
                <span class="order-price">{{ order.price.toFixed(2) }} €</span>
              </div>
            </div>
          </div>
          <div v-else class="no-orders">
            <p>Keine Bestellungen vorhanden</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.dialog-container {
  width: 100%;
  background: linear-gradient(135deg, #1a1a1a 0%, #2d2d2d 100%);
  padding: 2rem;
  border-radius: 16px;
  max-height: 70vh;
  overflow-y: auto;
}

.dialog-container::-webkit-scrollbar {
  width: 8px;
}

.dialog-container::-webkit-scrollbar-track {
  background: rgba(255, 215, 0, 0.05);
  border-radius: 10px;
}

.dialog-container::-webkit-scrollbar-thumb {
  background: rgba(255, 215, 0, 0.3);
  border-radius: 10px;
}

.dialog-container::-webkit-scrollbar-thumb:hover {
  background: rgba(255, 215, 0, 0.5);
}

.profile-card {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.5rem;
  align-items: start;
  justify-content: center;
}

/* Profile Picture Section */
.profile-picture-section {
  display: flex;
  justify-content: center;
  align-items: center;
}

.profile-picture-wrapper {
  position: relative;
  width: 200px;
  height: 200px;
}

.profile-picture {
  width: 100%;
  height: 100%;
  border-radius: 16px;
  object-fit: cover;
  border: 4px solid #ffd700;
  box-shadow: 0 8px 24px rgba(255, 215, 0, 0.2);
  transition: all 0.3s ease;
}

.profile-picture:hover {
  transform: scale(1.05);
  box-shadow: 0 12px 32px rgba(255, 215, 0, 0.3);
}

.toggle-btn {
  position: absolute;
  bottom: -15px;
  right: -15px;
  width: 50px;
  height: 50px;
  border-radius: 50%;
  border: none;
  background: linear-gradient(135deg, #ffd700 0%, #ffed4e 100%);
  color: #1a1a1a;
  font-size: 24px;
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: 0 4px 12px rgba(255, 215, 0, 0.3);
  transition: all 0.3s ease;
  font-weight: bold;
}

.toggle-btn:hover {
  transform: scale(1.15) rotate(180deg);
  box-shadow: 0 6px 16px rgba(255, 215, 0, 0.4);
}

.toggle-btn:active {
  transform: scale(0.9);
}

.toggle-icon {
  display: block;
  line-height: 1;
}

/* User Information Section */
.user-information {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

/* Info Card */
.info-card {
  background: rgba(79, 16, 85, 0.08);
  border: 2px solid rgba(255, 215, 0, 0.2);
  border-radius: 12px;
  padding: 1rem 1.25rem;
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
}

.info-card:hover {
  border-color: rgba(255, 215, 0, 0.4);
  background: rgba(255, 215, 0, 0.12);
}

.info-title {
  font-family: 'Poppins', sans-serif;
  font-size: 18px;
  font-weight: 700;
  color: #ffd700;
  margin: 0 0 1.5rem 0;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.info-item {
  margin-bottom: 1.5rem;
}

.info-item:last-child {
  margin-bottom: 0;
}

.label {
  display: block;
  font-family: 'Montserrat', sans-serif;
  font-size: 12px;
  font-weight: 600;
  color: #9ca3af;
  text-transform: uppercase;
  margin-bottom: 0.5rem;
  letter-spacing: 0.5px;
}

.value-display {
  font-family: 'Poppins', sans-serif;
  font-size: 18px;
  font-weight: 600;
  color: #ffffff;
  padding: 0.75rem 1rem;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  border-left: 3px solid #ffd700;
  transition: all 0.3s ease;
}

.value-display:hover {
  background: rgba(255, 255, 255, 0.08);
  transform: translateX(4px);
}

.value-display.ticket-id {
  font-family: 'Montserrat', monospace;
  font-size: 14px;
  letter-spacing: 1px;
}

/* Balance Section */
.balance-display {
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.15) 0%, rgba(255, 215, 0, 0.05) 100%);
  border-radius: 12px;
  padding: 1.5rem;
  text-align: center;
  margin-bottom: 1.5rem;
  border: 2px solid rgba(255, 215, 0, 0.3);
  transition: all 0.3s ease;
  cursor: pointer;
}

.balance-display:hover {
  border-color: rgba(255, 215, 0, 0.5);
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.2) 0%, rgba(255, 215, 0, 0.08) 100%);
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(255, 215, 0, 0.2);
}

.balance-amount {
  font-family: 'Poppins', sans-serif;
  font-size: 36px;
  font-weight: 700;
  color: #ffd700;
  margin-bottom: 0.5rem;
}

.balance-label {
  font-size: 12px;
  color: #9ca3af;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin: 0;
}

.top-up-section {
  margin-top: 1.5rem;
  padding-top: 1.5rem;
  border-top: 1px solid rgba(255, 215, 0, 0.2);
}

.input-wrapper {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

:deep(.input-field) {
  width: 100%;
  padding: 0.75rem 1rem;
  background-color: rgba(255, 255, 255, 0.05);
  border: 2px solid rgba(255, 215, 0, 0.2);
  border-radius: 8px;
  color: #ffffff;
  font-family: 'Poppins', sans-serif;
  font-size: 14px;
  transition: all 0.3s ease;
}

:deep(.input-field:focus) {
  background-color: rgba(255, 255, 255, 0.08);
  border-color: #ffd700;
  box-shadow: 0 0 12px rgba(255, 215, 0, 0.2);
  outline: none;
}

:deep(.input-field::placeholder) {
  color: rgba(255, 255, 255, 0.4);
}

.topup-btn {
  padding: 0.75rem 1.5rem;
  background: linear-gradient(135deg, #ffd700 0%, #ffed4e 100%);
  border: none;
  border-radius: 8px;
  color: #1a1a1a;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s ease;
  white-space: nowrap;
}

.topup-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(255, 215, 0, 0.3);
}

.topup-btn:active {
  transform: translateY(0);
}

/* Top-Ups Section */
.topups-section {
  margin-top: 1.5rem;
  padding-top: 1.5rem;
  border-top: 1px solid rgba(255, 215, 0, 0.2);
}

.section-title {
  font-family: 'Poppins', sans-serif;
  font-size: 14px;
  font-weight: 600;
  color: #ffd700;
  margin: 0 0 1rem 0;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.topups-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  max-height: 200px;
  overflow-y: auto;
}

.topups-list::-webkit-scrollbar {
  width: 6px;
}

.topups-list::-webkit-scrollbar-track {
  background: rgba(255, 215, 0, 0.05);
  border-radius: 10px;
}

.topups-list::-webkit-scrollbar-thumb {
  background: rgba(255, 215, 0, 0.3);
  border-radius: 10px;
}

.topups-list::-webkit-scrollbar-thumb:hover {
  background: rgba(255, 215, 0, 0.5);
}

.topup-item {
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 215, 0, 0.15);
  border-radius: 8px;
  padding: 0.75rem;
  transition: all 0.3s ease;
}

.topup-item:hover {
  background: rgba(255, 215, 0, 0.08);
  border-color: rgba(255, 215, 0, 0.3);
  transform: translateX(4px);
}

.topup-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.topup-date {
  font-size: 12px;
  color: #9ca3af;
  font-weight: 600;
}

.topup-amount {
  font-size: 14px;
  font-weight: 700;
  color: #86efac;
}

.topup-method {
  display: flex;
  align-items: center;
}

.method-label {
  font-size: 12px;
  background: rgba(59, 130, 246, 0.2);
  color: #93c5fd;
  padding: 0.25rem 0.5rem;
  border-radius: 4px;
  font-weight: 600;
}

.no-topups {
  text-align: center;
  padding: 1rem;
  color: #9ca3af;
}

.no-topups p {
  margin: 0;
  font-size: 14px;
}

/* Orders List */
.orders-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  max-height: 280px;
  overflow-y: auto;
}

.orders-list::-webkit-scrollbar {
  width: 6px;
}

.orders-list::-webkit-scrollbar-track {
  background: rgba(255, 215, 0, 0.05);
  border-radius: 10px;
}

.orders-list::-webkit-scrollbar-thumb {
  background: rgba(255, 215, 0, 0.3);
  border-radius: 10px;
}

.orders-list::-webkit-scrollbar-thumb:hover {
  background: rgba(255, 215, 0, 0.5);
}

.order-item {
  background: rgba(255, 255, 255, 0.03);
  border: 1px solid rgba(255, 215, 0, 0.15);
  border-radius: 10px;
  padding: 1rem;
  transition: all 0.3s ease;
}

.order-item:hover {
  background: rgba(255, 215, 0, 0.08);
  border-color: rgba(255, 215, 0, 0.3);
  transform: translateX(4px);
}

.order-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.75rem;
}

.order-date {
  font-size: 12px;
  color: #9ca3af;
  font-weight: 600;
}

.order-status {
  font-size: 12px;
  background: rgba(34, 197, 94, 0.2);
  color: #86efac;
  padding: 0.25rem 0.75rem;
  border-radius: 20px;
  font-weight: 600;
}

.order-details {
  margin-bottom: 0.75rem;
}

.order-items {
  font-size: 14px;
  color: #ffffff;
  margin: 0;
  font-weight: 500;
}

.order-footer {
  display: flex;
  justify-content: flex-end;
}

.order-price {
  font-size: 16px;
  font-weight: 700;
  color: #ffd700;
}

.no-orders {
  text-align: center;
  padding: 2rem 1rem;
  color: #9ca3af;
}

.no-orders p {
  margin: 0;
  font-size: 14px;
}
.edit-section {
  background: rgba(255, 215, 0, 0.05);
  border: 2px solid rgba(255, 215, 0, 0.15);
  border-radius: 12px;
  padding: 1.5rem;
  backdrop-filter: blur(10px);
  transition: all 0.3s ease;
}

.edit-section:hover {
  border-color: rgba(255, 215, 0, 0.3);
  background: rgba(255, 215, 0, 0.08);
}

.edit-title {
  font-family: 'Poppins', sans-serif;
  font-size: 14px;
  font-weight: 700;
  color: #ffd700;
  margin: 0 0 1rem 0;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.input-group {
  margin-bottom: 1.5rem;
}

.input-group:last-child {
  margin-bottom: 0;
}

/* Responsive Design */
@media (max-width: 768px) {
  .profile-card {
    grid-template-columns: 1fr;
    gap: 1rem;
  }

  .profile-picture-wrapper {
    width: 150px;
    height: 150px;
  }

  .toggle-btn {
    width: 40px;
    height: 40px;
    font-size: 18px;
    bottom: -10px;
    right: -10px;
  }

  .dialog-container {
    padding: 1rem;
    max-height: 80vh;
  }
}
</style>
