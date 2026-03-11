<script setup lang="ts">
import { ref } from 'vue'
import profileManImage from '@/assets/Profil_man.png'
import profileWomanImage from '@/assets/Profil_woman.png'

const props = defineProps<{
  currentImage: string
}>()

const emit = defineEmits<{
  updateProfileImage: [newImage: string]
}>()

const userName = ref('Max Mustermann')
const ticketId = ref('TICKET-2024-001')

const toggleProfileImage = () => {
  const newImage = props.currentImage === profileManImage ? profileWomanImage : profileManImage
  emit('updateProfileImage', newImage)
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
            <div class="value-display">{{ userName }}</div>
          </div>

          <div class="info-item">
            <label class="label">Ticket ID</label>
            <div class="value-display ticket-id">{{ ticketId }}</div>
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
}

.profile-card {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 2rem;
  align-items: center;
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
  gap: 2rem;
}

/* Info Card */
.info-card {
  background: rgba(255, 215, 0, 0.08);
  border: 2px solid rgba(255, 215, 0, 0.2);
  border-radius: 12px;
  padding: 1.5rem;
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

/* Edit Section */
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

/* Responsive Design */
@media (max-width: 768px) {
  .profile-card {
    grid-template-columns: 1fr;
    gap: 2rem;
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
  }
}
</style>
