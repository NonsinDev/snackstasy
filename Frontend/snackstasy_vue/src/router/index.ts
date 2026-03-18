import { createRouter, createWebHistory, createWebHashHistory } from 'vue-router'

import Login from '@/views/Login.vue'
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


// 🔒 Globaler Login-Schutz
router.beforeEach((to, _from, next) => {
  const isAuthenticated = sessionStorage.getItem("auth") === "true";

  if (to.meta.requiresAuth && !isAuthenticated) {
    next("/login");
  } 
  else if (to.path === "/login" && isAuthenticated) {
    next("/");
  } 
  else {
    next();
  }
});
export default router
