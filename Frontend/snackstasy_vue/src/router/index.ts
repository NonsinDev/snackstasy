import { createRouter, createWebHistory, createWebHashHistory } from 'vue-router'
import { useAuth, initAuth } from "../services/Authentification";
import Login from '@/views/Login.vue'
import FoodMenu from '../views/FoodMenu.vue'
import SelectedFoodStand from '../views/SelectedFoodStand.vue'
import Checkout from '@/views/Checkout.vue'
import OrderStatus from '@/views/OrderStatus.vue'


const router = createRouter({
  history: createWebHashHistory(),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: Login,
      meta: { showHeader: false }
    },
    {
      path: '/',
      name: 'home',
      component: FoodMenu,
      meta: { requiresAuth: true,  
              showHeader: true },
    },
    {
      path: '/stand/:standId',
      name: 'selected-stand',
      component: SelectedFoodStand,
      meta: { requiresAuth: true,
              showHeader: true
      },
    },
    {
      path: '/checkout',
      name: 'Checkout',
      component: Checkout,
      meta: {
        requiresAuth: true,
        showHeader: true
      },
    },
    {
      path: '/order-status/:order_id?',
      name: 'order-status',
      component: OrderStatus,
      meta: {
        requiresAuth: true,
        showHeader: true
      },
    }
  ],
})


// 🔒 Globaler Login-Schutz
router.beforeEach(async (to) => {
  const { isAuthenticated, isLoading } = useAuth()

  if (isLoading.value) {
    await initAuth()
  }

  if (to.meta.requiresAuth && !isAuthenticated()) {
    return "/login"
  }

  return true
});

export default router
