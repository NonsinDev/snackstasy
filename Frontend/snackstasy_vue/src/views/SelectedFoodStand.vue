<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { ItemsByStandId } from '@/services/Stands'
import type { ItemsByStand } from '@/model/Items'
import img1 from '@/assets/Burger.png'
import img2 from '@/assets/pizza.jpg'
import img3 from '@/assets/wok.jpg'
import img4 from '@/assets/corndog.jpg'
import img5 from '@/assets/sushi.jpg'
import img6 from '@/assets/tgaco.jpg'
import img7 from '@/assets/KeinBild.jpg'
import FoodMenu from './FoodMenu.vue'

// Router
const route = useRoute()
const router = useRouter()

// Daten
const standId = Number(route.params.standId)
const foodItems = ref<ItemsByStand[]>([])
const displayedItems = ref<ItemsByStand[]>([])

const isLoading = ref(true)
const isLoadingMore = ref(false)
const error = ref<string | null>(null)

// Infinity Scroll
const itemsPerLoad = 20
const currentPage = ref(0)
const hasMore = ref(true)

// 🖼️ Stand Infos (kannst du später aus API holen)
const standName = ref('Food Stand 🍔')
const standDescription = ref('Leckeres Essen frisch zubereitet')

// Navigation
const goBack = () => {
  router.back()
}

// Items laden
const loadMoreItems = () => {
  if (!hasMore.value || isLoadingMore.value) return

  isLoadingMore.value = true

  const start = currentPage.value * itemsPerLoad
  const end = start + itemsPerLoad

  const newItems = foodItems.value.slice(start, end)

  if (newItems.length === 0) {
    hasMore.value = false
    isLoadingMore.value = false
    return
  }

  displayedItems.value.push(...newItems)
  currentPage.value++

  isLoadingMore.value = false
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
  } catch (err) {
    console.error('Fehler beim Laden:', err)
    error.value = 'Fehler beim Laden der Gerichte'
  } finally {
    isLoading.value = false
  }
})

const getImageByStandId = (id: number) => {
  switch (id) {
    case 1:
      return img1
    case 2:
      return img2
    case 3:
      return img3
    case 4:
      return img4
    case 5:
      return img5
    case 6:
      return img6
    default:
      return img7 // fallback
  }
}
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
        <img :src="getImageByStandId(standId)" alt="Stand Bild" />
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
        <div v-if="hasMore" class="load-more-wrapper">
          <button class="load-more-btn" @click="loadMoreItems">Mehr laden</button>
        </div>

        <div v-else class="end-message">
          <p>Alle Gerichte geladen</p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.load-more-wrapper {
  display: flex;
  justify-content: center;
  padding: 20px 0;
}

.load-more-btn {
  background: linear-gradient(135deg, #0f0f0f, #1a1a1a);
  color: #f5f5f5;
  border: 1px solid rgba(255, 221, 0, 0.25); /* leichtes Gelb */
  padding: 12px 28px;
  font-size: 16px;
  font-weight: 600;
  border-radius: 14px;
  cursor: pointer;
  position: relative;
  overflow: hidden;
  transition: all 0.25s ease;
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.35);
}

/* Hover Effekt */
.load-more-btn:hover {
  transform: translateY(-2px);
  border-color: rgba(120, 255, 120, 0.6); /* grün */
  box-shadow:
    0 10px 25px rgba(120, 255, 120, 0.15),
    0 6px 25px rgba(255, 221, 0, 0.1);
}

/* Aktiver Klick */
.load-more-btn:active {
  transform: scale(0.97);
}

/* Glow Effekt (gelb + grün Mischung) */
.load-more-btn::before {
  content: '';
  position: absolute;
  top: 0;
  left: -50%;
  width: 200%;
  height: 100%;
  background: linear-gradient(
    120deg,
    transparent,
    rgba(255, 221, 0, 0.15),
    rgba(120, 255, 120, 0.15),
    transparent
  );
  transform: skewX(-20deg);
  transition: all 0.5s ease;
}

.load-more-btn:hover::before {
  left: 100%;
}

.selected-stand-container {
  display: flex;
  flex-direction: column;

  height: 100%;
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
