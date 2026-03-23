<script setup lang="ts">
import { ref, computed } from 'vue'
import InputNumber from 'primevue/inputnumber'
import Button from 'primevue/button'
import type { User_Data } from '@/model/UserData';
import { AddBalance } from '@/services/Header';

const props = defineProps<{
  currentUser: User_Data 
}>()

const emit = defineEmits<{
  updateBalance: [newBalance: number]
  closeDialog: []
  refreshUser: []
}>()

const amounts = [5, 10, 20, 50]
const customAmount = ref<number | null>(null)
const isLoading = ref(false)

const balanceAfterTopUp = computed(() => {
  if (customAmount.value) {
    return props.currentUser?.balance + customAmount.value
  }
  return props.currentUser.balance
})

const topUpWithAmount = async (amount: number) => {
  if (!props.currentUser) return

  isLoading.value = true
  try {
    // echte API aufrufen
    await AddBalance(props.currentUser.user_id, amount)
    emit('refreshUser');
  } catch (error) {
    console.error("Fehler beim Aufladen:", error)
    // hier könntest du noch eine Fehlermeldung anzeigen
  } finally {
    isLoading.value = false
  }
}

const topUpWithCustomAmount = async () => {
  if (!props.currentUser || !customAmount.value || customAmount.value <= 0) return

  isLoading.value = true
  try {
    await AddBalance(props.currentUser.user_id, customAmount.value)
    emit('refreshUser');
    customAmount.value = null
    isLoading.value = false
  } catch (error) {
    console.error("Fehler beim Aufladen:", error)
  } finally {
    isLoading.value = false
  }
}

</script>

<template>
  <div class="dialog-container">
    <!-- Current Balance Section -->
    <div class="balance-section">
      <h2 class="section-title">Aktueller Kontostand</h2>
      <div class="balance-card">
        <span class="currency-symbol">€</span>
        <span class="balance-amount">{{ props.currentUser.balance.toFixed(2) }}</span>
      </div>
    </div>

    <!-- Quick Top-Up Options -->
    <div class="topup-section">
      <h3 class="section-title">Schnell aufladen</h3>
      <div class="amount-buttons">
        <button
          v-for="amount in amounts"
          :key="amount"
          class="amount-btn"
          @click="topUpWithAmount(amount)"
          :disabled="isLoading"
        >
          <span class="plus-icon">+</span>
          {{ amount }}€
        </button>
      </div>
    </div>

    <!-- Custom Amount Section -->
    <div class="custom-section">
      <h3 class="section-title">Benutzerdefinierten Betrag eingeben</h3>
      <div class="input-container">
        <InputNumber
          v-model="customAmount"
          placeholder="Betrag in €"
          :min="0.01"
          :step="0.1"
          currency="EUR"
          :locale="'de-DE'"
          class="custom-input"
          :disabled="isLoading"
        />
      </div>
      <button
        v-if="customAmount && customAmount > 0"
        class="custom-btn"
        @click="topUpWithCustomAmount"
        :disabled="isLoading"
      >
        <span v-if="!isLoading">{{ customAmount.toFixed(2) }}€ aufladen</span>
        <span v-else>Wird verarbeitet...</span>
      </button>
    </div>

    <!-- Preview Section -->
    <div v-if="customAmount && customAmount > 0" class="preview-section">
      <div class="preview-card">
        <div class="preview-item">
          <span class="preview-label">Aktueller Saldo:</span>
          <span class="preview-value current">{{ props.currentUser.balance.toFixed(2) }}€</span>
        </div>
        <div class="preview-plus">+</div>
        <div class="preview-item">
          <span class="preview-label">Aufladen:</span>
          <span class="preview-value add">{{ customAmount.toFixed(2) }}€</span>
        </div>
        <div class="preview-equal">=</div>
        <div class="preview-item total">
          <span class="preview-label">Neuer Saldo:</span>
          <span class="preview-value total">{{ balanceAfterTopUp.toFixed(2) }}€</span>
        </div>
      </div>
    </div>

    <!-- Info Section -->
    <div class="info-section">
      <p class="info-text">💡 Dein Guthaben wird sofort nach der Aufladung verfügbar sein.</p>
    </div>
  </div>
</template>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Poppins:wght@400;600;700&family=Montserrat:wght@500;600&display=swap');

.dialog-container {
  width: 100%;
  background: linear-gradient(135deg, #1a1a1a 0%, #2d2d2d 100%);
  padding: 2rem;
  border-radius: 16px;
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

/* Current Balance Section */
.balance-section {
  text-align: center;
}

.section-title {
  font-family: 'Poppins', sans-serif;
  font-size: 16px;
  font-weight: 700;
  color: #9ca3af;
  text-transform: uppercase;
  letter-spacing: 1px;
  margin: 0 0 1.5rem 0;
}

.balance-card {
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.15) 0%, rgba(255, 215, 0, 0.05) 100%);
  border: 2px solid rgba(255, 215, 0, 0.3);
  border-radius: 16px;
  padding: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  transition: all 0.3s ease;
}

.balance-card:hover {
  border-color: rgba(255, 215, 0, 0.5);
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.2) 0%, rgba(255, 215, 0, 0.08) 100%);
  transform: translateY(-2px);
}

.currency-symbol {
  font-family: 'Poppins', sans-serif;
  font-size: 24px;
  font-weight: 700;
  color: #ffd700;
}

.balance-amount {
  font-family: 'Poppins', sans-serif;
  font-size: 48px;
  font-weight: 800;
  background: linear-gradient(135deg, #ffd700 100%, #ffed4e 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

/* Quick Top-Up Section */
.topup-section {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.amount-buttons {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(80px, 1fr));
  gap: 1rem;
}

.amount-btn {
  padding: 1rem;
  border: 2px solid rgba(255, 215, 0, 0.3);
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.1) 0%, rgba(255, 215, 0, 0.05) 100%);
  border-radius: 12px;
  color: #ffffff;
  font-family: 'Poppins', sans-serif;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 0.5rem;
}

.amount-btn:hover:not(:disabled) {
  background: linear-gradient(135deg, rgba(255, 215, 0, 0.25) 0%, rgba(255, 215, 0, 0.1) 100%);
  border-color: rgba(255, 215, 0, 0.6);
  transform: translateY(-4px);
  box-shadow: 0 4px 12px rgba(255, 215, 0, 0.2);
}

.amount-btn:active:not(:disabled) {
  transform: scale(0.95);
}

.amount-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.plus-icon {
  font-size: 18px;
  color: #ffd700;
  font-weight: bold;
}

/* Custom Amount Section */
.custom-section {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.input-container {
  display: flex;
  gap: 1rem;
  align-items: flex-start;
}

:deep(.custom-input) {
  width: 100%;
}

:deep(.custom-input .p-inputnumber-input) {
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

:deep(.custom-input .p-inputnumber-input:focus) {
  background-color: rgba(255, 255, 255, 0.08);
  border-color: #ffd700;
  box-shadow: 0 0 12px rgba(255, 215, 0, 0.2);
  outline: none;
}

:deep(.custom-input .p-inputnumber-input::placeholder) {
  color: rgba(255, 255, 255, 0.4);
}

.custom-btn {
  padding: 0.75rem 1.5rem;
  background: linear-gradient(135deg, #ffd700 0%, #ffed4e 100%);
  border: none;
  border-radius: 8px;
  color: #1a1a1a;
  font-family: 'Poppins', sans-serif;
  font-weight: 600;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.3s ease;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.custom-btn:hover:not(:disabled) {
  box-shadow: 0 6px 16px rgba(255, 215, 0, 0.3);
  transform: translateY(-2px);
}

.custom-btn:active:not(:disabled) {
  transform: scale(0.95);
}

.custom-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

/* Preview Section */
.preview-section {
  animation: slideIn 0.3s ease-out;
}

@keyframes slideIn {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.preview-card {
  background: rgba(255, 215, 0, 0.08);
  border: 2px solid rgba(255, 215, 0, 0.2);
  border-radius: 12px;
  padding: 1.5rem;
  display: grid;
  grid-template-columns: 1fr auto 1fr auto 1fr;
  gap: 1rem;
  align-items: center;
  justify-items: center;
}

.preview-item {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  text-align: center;
}

.preview-label {
  font-family: 'Montserrat', sans-serif;
  font-size: 11px;
  font-weight: 600;
  color: #9ca3af;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.preview-value {
  font-family: 'Poppins', sans-serif;
  font-size: 18px;
  font-weight: 700;
  color: #ffffff;
}

.preview-value.current {
  color: #ffd700;
}

.preview-value.add {
  color: #4ade80;
}

.preview-value.total {
  color: #ffd700;
  font-size: 20px;
}

.preview-item.total {
  grid-column: span 1;
}

.preview-plus,
.preview-equal {
  font-family: 'Poppins', sans-serif;
  font-size: 20px;
  font-weight: 700;
  color: rgba(255, 215, 0, 0.5);
}

/* Info Section */
.info-section {
  background: rgba(79, 172, 254, 0.1);
  border: 2px solid rgba(79, 172, 254, 0.2);
  border-radius: 12px;
  padding: 1rem;
  text-align: center;
}

.info-text {
  font-family: 'Poppins', sans-serif;
  font-size: 14px;
  color: #93c5fd;
  margin: 0;
  line-height: 1.5;
}

/* Responsive Design */
@media (max-width: 600px) {
  .dialog-container {
    padding: 1.5rem;
    gap: 1.5rem;
  }

  .balance-amount {
    font-size: 36px;
  }

  .amount-buttons {
    grid-template-columns: repeat(2, 1fr);
  }

  .preview-card {
    grid-template-columns: 1fr;
    gap: 0.75rem;
  }

  .preview-plus,
  .preview-equal {
    display: none;
  }
}
</style>
