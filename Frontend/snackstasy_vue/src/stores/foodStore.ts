import { ref } from 'vue'
import type { AllStands, ItemsByStand } from '@/model/Items'

export const useFoodStore = () => {
  const currentStand = ref<AllStands | null>(null)
  const currentItems = ref<ItemsByStand[]>([])

  const setStandAndItems = (stand: AllStands, items: ItemsByStand[]) => {
    currentStand.value = stand
    currentItems.value = items
  }

  const clearStandData = () => {
    currentStand.value = null
    currentItems.value = []
  }

  return {
    currentStand,
    currentItems,
    setStandAndItems,
    clearStandData,
  }
}
