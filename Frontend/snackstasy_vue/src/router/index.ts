import { createRouter, createWebHistory, createWebHashHistory } from 'vue-router'

import Login from '@/views/Login.vue'
import { useAuth, initAuth } from "../services/Authentification";
import FoodMenu from '../views/FoodMenu.vue';

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
      meta: { /* requiresAuth: true,  */
              showHeader: true
      },
    },

  ],
})


// 🔒 Globaler Login-Schutz
router.beforeEach(async (to, _from, next) => {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading.value) {
    await initAuth(); // 👈 Session im Backend prüfen
  }

  if (to.meta.requiresAuth && !isAuthenticated()) {
    next("/login");
  } else {
    next();
  }
});
export default router
