import { createRouter, createWebHistory, createWebHashHistory } from 'vue-router'
import { useAuth, useEmployeeAuth, initAuth } from "../services/Authentification";
import Login from '@/views/Login.vue'
import EmployeeLogin from '@/views/EmployeeLogin.vue'
import EmployeeDashboard from '@/views/EmployeeDashboard.vue'
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
      path: '/employee-login',
      name: 'employee-login',
      component: EmployeeLogin,
      meta: { showHeader: false }
    },
    {
      path: '/employee-dashboard',
      name: 'employee-dashboard',
      component: EmployeeDashboard,
      meta: { requiresEmployeeAuth: true, showHeader: false }
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
  
  console.log('Router Guard - navigating to:', to.path)
  console.log('Router Guard - requires auth:', to.meta.requiresAuth)
  console.log('Router Guard - requires employee auth:', to.meta.requiresEmployeeAuth)

  if (isLoading.value) {
    await initAuth()
  }

  // Für Employee-Dashboard: überprüfe localStorage direkt
  if (to.meta.requiresEmployeeAuth) {
    const employeeAuthStored = localStorage.getItem('employeeAuth')
    const isEmployeeAuth = employeeAuthStored !== null
    
    console.log('Employee Auth Check - stored data:', employeeAuthStored)
    console.log('Employee Auth Check - isAuthenticated:', isEmployeeAuth)
    
    if (!isEmployeeAuth) {
      console.log('Router Guard - blocking, no employee auth')
      return "/employee-login"
    }
  }

  // Prüfe normale User Authentication
  if (to.meta.requiresAuth && !isAuthenticated()) {
    console.log('Router Guard - blocking user auth')
    return "/login"
  }

  console.log('Router Guard - allowing navigation')
  return true
});

export default router
