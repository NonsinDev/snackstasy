<script setup lang="ts">
import { ref } from 'vue'
import InputText from 'primevue/inputtext'

interface FoodStand {
  id: number
  name: string
  category: string
  location: string
  isOpen: boolean
  image: string
}

const search = ref('')

const stands = ref<FoodStand[]>([
  {
    id: 1,
    name: 'Burger Heaven',
    category: 'Burger',
    location: 'Zone A',
    isOpen: true,
    image: 'https://images.unsplash.com/photo-1550547660-d9450f859349',
  },
  {
    id: 2,
    name: 'Pizza World',
    category: 'Pizza',
    location: 'Zone B',
    isOpen: false,
    image: 'https://images.unsplash.com/photo-1548365328-8b849e9c7b2a',
  },
  {
    id: 3,
    name: 'Asia Wok',
    category: 'Asiatisch',
    location: 'Zone C',
    isOpen: true,
    image: 'https://images.unsplash.com/photo-1604908176997-431d6d45e13b',
  },
  {
    id: 1,
    name: 'Burger Heaven',
    category: 'Burger',
    location: 'Zone A',
    isOpen: true,
    image: 'https://images.unsplash.com/photo-1550547660-d9450f859349',
  },
  {
    id: 2,
    name: 'Pizza World',
    category: 'Pizza',
    location: 'Zone B',
    isOpen: false,
    image: 'https://images.unsplash.com/photo-1548365328-8b849e9c7b2a',
  },
  {
    id: 3,
    name: 'Asia Wok',
    category: 'Asiatisch',
    location: 'Zone C',
    isOpen: true,
    image: 'https://images.unsplash.com/photo-1604908176997-431d6d45e13b',
  },
])

const filteredStands = () => {
  return stands.value.filter((s) => s.name.toLowerCase().includes(search.value.toLowerCase()))
}
</script>

<template>
  <div class="main-container">
    <!-- 🔍 Search -->
    <div class="search-bar">
      <InputText v-model="search" placeholder="Suche nach Essen..." />
    </div>

    <!-- 📄 Liste -->
    <div class="list">
      <div v-for="stand in filteredStands()" :key="stand.id" class="list-card">
        <!-- 🖼️ Bild -->
        <div class="image-wrapper">
          <img :src="stand.image" />

          <div class="status" :class="stand.isOpen ? 'open' : 'closed'">
            {{ stand.isOpen ? 'Offen' : 'Geschlossen' }}
          </div>
        </div>

        <!-- 📋 Infos -->
        <div class="content">
          <div class="top">
            <h2>{{ stand.name }}</h2>
            <span class="badge">{{ stand.category }}</span>
          </div>

          <p class="location">📍 {{ stand.location }}</p>

          <div class="bottom">
            <span class="open-text" :class="stand.isOpen ? 'green' : 'red'">
              {{ stand.isOpen ? 'Jetzt geöffnet' : 'Geschlossen' }}
            </span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.main-container {
  padding: 20px;
}

/* 🔍 Search */
.search-bar {
  margin-bottom: 20px;
  border: 2px solid black;
  border-radius: 20px;
}

.search-bar :deep(input) {
  width: 100%;
  padding: 12px;
  border-radius: 12px;
}

/* 📄 Liste */
.list {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

/* 📄 Card */
.list-card {
  display: flex;
  background: white;
  border-radius: 20px;
  overflow: hidden;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.08);
}

.list-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
}

/* 🖼️ Bild */
.image-wrapper {
  position: relative;
  width: 200px;
  min-width: 200px;
  height: 150px;
  overflow: hidden;
}

.image-wrapper img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.4s ease;
}

.list-card:hover img {
  transform: scale(1.1);
}

/* Status Badge */
.status {
  position: absolute;
  top: 10px;
  left: 10px;
  padding: 6px 12px;
  border-radius: 999px;
  font-size: 12px;
  font-weight: bold;
  color: white;
}

.open {
  background: #22c55e;
}

.closed {
  background: #ef4444;
}

/* 📋 Content */
.content {
  flex: 1;
  padding: 15px 20px;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}

.top {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.top h2 {
  margin: 0;
  font-size: 20px;
}

/* Kategorie Badge */
.badge {
  background: #f3f4f6;
  padding: 6px 10px;
  border-radius: 10px;
  font-size: 12px;
}

/* Location */
.location {
  margin: 10px 0;
  color: #666;
}

/* Bottom */
.bottom {
  display: flex;
  justify-content: flex-end;
}

.open-text.green {
  color: #22c55e;
  font-weight: bold;
}

.open-text.red {
  color: #ef4444;
  font-weight: bold;
}

/* 📱 Mobile */
@media (max-width: 700px) {
  .list-card {
    flex-direction: column;
  }

  .image-wrapper {
    width: 100%;
    height: 160px;
  }
}
</style>
