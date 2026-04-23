import { defineStore } from 'pinia'
import type { ItemsByStand } from '@/model/Items'

export interface OrderDetails {
  order_id: number
  items: ItemsByStand[]
  totalPrice: number
  pickupId: number
  standName: string
  orderStatus: 'preparing' | 'ready' | 'completed'
  timestamp: Date
}

export const useCartStore = defineStore('cart', {
    state: () => ({
        items: [] as ItemsByStand[],
        orderDetails: null as OrderDetails | null,
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

        removeItem(index: number) {
            if (index >= 0 && index < this.items.length) {
                this.items.splice(index, 1)
            }
        },

        createOrder(pickupId: number, standName: string, order_id: number) {
            this.orderDetails = {
                order_id,
                items: [...this.items],
                totalPrice: this.totalPrice,
                pickupId,
                standName,
                orderStatus: 'preparing',
                timestamp: new Date(),
            }
        },

        updateOrderStatus(status: 'preparing' | 'ready' | 'completed') {
            if (this.orderDetails) {
                this.orderDetails.orderStatus = status
            }
        },

        clearCart() {
            this.items = []
        },

        clearOrder() {
            this.orderDetails = null
        },
    },
}
)
