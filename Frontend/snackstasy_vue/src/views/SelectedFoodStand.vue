<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ItemsByStandId } from '@/services/Stands'
import type { ItemsByStand } from '@/model/Items'

// Router
const route = useRoute()
const router = useRouter()

// Daten
const foodItems = ref<ItemsByStand[]>([])
const displayedItems = ref<ItemsByStand[]>([])

const isLoading = ref(true)
const error = ref<string | null>(null)

// Infinity Scroll
const itemsPerLoad = 40
const currentPage = ref(0)
const hasMore = ref(true)
const scrollContainer = ref<HTMLElement | null>(null)

// 🖼️ Stand Infos (kannst du später aus API holen)
const standName = ref('Food Stand 🍔')
const standDescription = ref('Leckeres Essen frisch zubereitet')

// Bild (empfohlen: import)
import headerImage from '@/assets/corndog.jpg'

// Navigation
const goBack = () => {
  router.back()
}

// Items laden
const loadMoreItems = () => {
  if (!hasMore.value) return

  const start = currentPage.value * itemsPerLoad
  const end = start + itemsPerLoad
  const newItems = foodItems.value.slice(start, end)

  if (newItems.length === 0) {
    hasMore.value = false
    return
  }

  displayedItems.value.push(...newItems)
  currentPage.value++
}

// Scroll Handler
const handleScroll = () => {
  if (!scrollContainer.value) return

  const { scrollTop, scrollHeight, clientHeight } = scrollContainer.value

  if (scrollTop + clientHeight >= scrollHeight * 0.8) {
    loadMoreItems()
  }
}

// Lifecycle
onMounted(async () => {
  try {
    isLoading.value = true
    const standId = route.params.standId as string

    if (!standId) {
      error.value = 'Stand-ID nicht gefunden'
      return
    }

    const items = await ItemsByStandId(parseInt(standId))
    foodItems.value = items

    loadMoreItems()

    if (scrollContainer.value) {
      scrollContainer.value.addEventListener('scroll', handleScroll)
    }
  } catch (err) {
    console.error('Fehler beim Laden:', err)
    error.value = 'Fehler beim Laden der Gerichte'
  } finally {
    isLoading.value = false
  }
})

onUnmounted(() => {
  if (scrollContainer.value) {
    scrollContainer.value.removeEventListener('scroll', handleScroll)
  }
})
</script>

<template>
  <div class="selected-stand-container">
    <!-- Header -->
    <div class="header">
      <button class="back-btn" @click="goBack">← Zurück</button>
    </div>

    <!-- Loading -->
    <div v-if="isLoading" class="loading-container">
      <p>Gerichte werden geladen...</p>
    </div>

    <!-- Error -->
    <div v-else-if="error" class="error-container">
      <p>{{ error }}</p>
      <button class="back-btn" @click="goBack">Zurück</button>
    </div>

    <!-- Content -->
    <div v-else class="content-container">
      <!-- 🖼️ Hero -->
      <div class="hero">
        <img :src="headerImage" alt="Stand Bild" />
        <div class="overlay">
          <h1>{{ standName }}</h1>
          <p>{{ standDescription }}</p>
        </div>
      </div>

      <!-- 🍽️ Items -->
      <div ref="scrollContainer" class="food-items-section">
        <div class="items-grid">
          <div v-for="item in displayedItems" :key="item.item_id" class="food-item">
            <div class="item-header">
              <h3>{{ item.name }}</h3>
              <span class="price">{{ item.price.toFixed(2) }}€</span>
            </div>
            <p class="description">Bestand: {{ item.stock }}</p>
            <button class="add-btn">In den Warenkorb</button>
          </div>
        </div>

        <!-- Loader -->
        <div v-if="hasMore" class="loading">
          <p>Weitere Gerichte werden geladen...</p>
        </div>

        <div v-else class="end-message">
          <p>Alle Gerichte geladen</p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.selected-stand-container {
  display: flex;
  flex-direction: column;
  height: 100vh;
  background: #f9fafb;
}

/* Header */
.header {
  padding: 15px 20px;
  background: white;
  border-bottom: 1px solid #e5e7eb;
}

.back-btn {
  background: none;
  border: none;
  font-size: 16px;
  cursor: pointer;
  padding: 8px 12px;
  border-radius: 8px;
}

.back-btn:hover {
  background: #f3f4f6;
}

/* Layout */
.content-container {
  display: flex;
  flex-direction: column;
  height: 100%;
}

/* 🖼️ Hero */
.hero {
  position: relative;
  height: 200px;
  overflow: hidden;
}

.hero img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.overlay {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 16px;
  background: linear-gradient(to top, rgba(0, 0, 0, 0.6), transparent);
  color: white;
}

.overlay h1 {
  margin: 0;
  font-size: 20px;
}

.overlay p {
  margin: 4px 0 0;
  font-size: 14px;
}

/* Items */
.food-items-section {
  flex: 1;
  overflow-y: auto;
  padding: 20px;
}

.items-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 16px;
}

/* Card */
.food-item {
  background: white;
  border-radius: 12px;
  padding: 16px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.08);
  transition: 0.3s;
}

.food-item:hover {
  transform: translateY(-4px);
}

.item-header {
  display: flex;
  justify-content: space-between;
}

.price {
  font-weight: bold;
  color: #059669;
}

.description {
  font-size: 13px;
  color: #6b7280;
}

.add-btn {
  width: 100%;
  padding: 10px;
  background: #3b82f6;
  color: white;
  border: none;
  border-radius: 8px;
}

.add-btn:hover {
  background: #2563eb;
}

/* States */
.loading,
.end-message {
  text-align: center;
  padding: 20px;
  color: #6b7280;
}

.loading-container,
.error-container {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 100vh;
}

/* Mobile */
@media (max-width: 700px) {
  .items-grid {
    grid-template-columns: 1fr;
  }
}
</style>
