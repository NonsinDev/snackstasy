import { defineStore } from 'pinia'
import type { AllStands, ItemsByStand } from '@/model/Items'

export const useFoodStore = defineStore('food', {
  state: () => ({
    currentStand: null as AllStands | null,
    currentItems: [] as ItemsByStand[],
  }),

  actions: {
    setStandAndItems(stand: AllStands, items: ItemsByStand[]) {
      this.currentStand = stand
      this.currentItems = items
    },

    clearStandData() {
      this.currentStand = null
      this.currentItems = []
    },
  },
})
