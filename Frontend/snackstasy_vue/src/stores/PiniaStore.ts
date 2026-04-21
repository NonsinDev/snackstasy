import { defineStore } from 'pinia'
import type { ItemsByStand } from '@/model/Items'

export const useCartStore = defineStore('cart', {
  state: () => ({
    items: [] as ItemsByStand[],
  }),

  getters: {
    totalPrice: (state) =>
      state.items.reduce((sum, item) => sum + item.price, 0),

    itemCount: (state) => state.items.length,
  },

  actions: {
    addItem(item: ItemsByStand) {
      this.items.push(item)
    },

    clearCart() {
      this.items = []
    },
  },
})