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
      meta: { requiresAuth: true, 
              showHeader: true
      },
    },

  ],
})

/* const router = createRouter({
  history: createWebHashHistory(),
  routes,
}); 

// 🔒 Globaler Login-Schutz
router.beforeEach(async (to, _from, next) => {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading.value) {
    await initAuth(); // 👈 Session im Backend prüfen
  }

  if (to.meta.requiresAuth && !isAuthenticated()) {
    next('/login');
  } else {
    next();
  }
});*/

// 🔒 Fake Frontend-Session (nur für Entwicklung)
router.beforeEach((to, _from, next) => {
  const isAuthenticated = sessionStorage.getItem('auth') === 'true'

  if (to.meta.requiresAuth && !isAuthenticated) {
    next('/login')
  } else if (to.name === 'login' && isAuthenticated) {
    next('/') // Bereits eingeloggt → direkt zur Home
  } else {
    next()
  }
})
export default router
