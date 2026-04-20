<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import InputText from 'primevue/inputtext'
import { GetAllStands, ItemsByStandId } from '@/services/Stands'
import { useFoodStore } from '@/stores/foodStore'
import type { AllStands } from '@/model/Items'
import img from '@/assets/corndog.jpg'

const router = useRouter()
const foodStore = useFoodStore()
const search = ref('')
const stands = ref<AllStands[]>([])
const loading = ref(false)
const error = ref('')

onMounted(async () => {
  await fetchStands()
})

const fetchStands = async () => {
  loading.value = true
  error.value = ''
  try {
    stands.value = await GetAllStands()
  } catch (e) {
    console.error('Fehler beim Laden der Stände:', e)
    error.value = 'Stände konnten nicht geladen werden'
  } finally {
    loading.value = false
  }
}

const filteredStands = () => {
  return stands.value.filter((s) => s.name.toLowerCase().includes(search.value.toLowerCase()))
}

const selectStand = async (stand: AllStands) => {
  try {
    loading.value = true
    const items = await ItemsByStandId(stand.stand_id)
    foodStore.setStandAndItems(stand, items)
    router.push({
      name: 'selected-stand',
      params: { standId: stand.stand_id },
    })
  } catch (e) {
    console.error('Fehler beim Laden der Items:', e)
    error.value = 'Items konnten nicht geladen werden'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="main-container">
    <!-- 🔍 Search -->
    <div class="search-bar">
      <InputText v-model="search" placeholder="Suche nach Essen..." />
    </div>

    <!-- 📄 Liste -->
    <div v-if="loading" class="loading-message">Stände werden geladen...</div>
    <div v-else-if="error" class="error-message">{{ error }}</div>
    <div v-else class="list">
      <div
        v-for="stand in filteredStands()"
        :key="stand.stand_id"
        class="list-card"
        @click="selectStand(stand)"
      >
        <!-- 🖼️ Bild -->
        <div class="image-wrapper">
          <img :src="img" />
          <div class="status">{{ stand.name }}</div>
        </div>

        <!-- 📋 Infos -->
        <div class="content">
          <div class="top">
            <h2>{{ stand.name }}</h2>
          </div>
          <div class="bottom">
            <span class="open-text green">Stand ID: {{ stand.stand_id }}</span>
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
  font-weight: 800;
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

.loading-message,
.error-message {
  text-align: center;
  padding: 40px 20px;
  font-size: 16px;
}

.error-message {
  color: #ef4444;
  background: #fee2e2;
  border-radius: 12px;
  margin: 20px;
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
