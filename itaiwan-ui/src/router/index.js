import { createRouter, createWebHistory } from 'vue-router'
// 引入你的頁面組件
import HotspotList from '../views/HotspotList.vue'
import Login from '../views/Login.vue'
import Register from '../views/Register.vue'
import MyFavorites from '../views/MyFavorites.vue'
import Profile from '../views/Profile.vue'
const routes = [
  { 
    path: '/', 
    name: 'Home',
    component: HotspotList,
    meta: { requiresAuth: true } // 標記：這個頁面需要登錄才能看
  },
  {
    path: '/hotspots',
    name: 'HotspotList',
    component: HotspotList,
    meta: { requiresAuth: true }
  },


  { 
    path: '/login', 
    name: 'Login',
    component: Login 
  },
  { 
    path: '/register', 
    name: 'Register',
    component: Register 
  },
  { path: '/favorites', 
    component: MyFavorites,
     meta: { requiresAuth: true } 
    },
    {
    path: '/profile',
    name: 'Profile',
    component: Profile,
    meta: { requiresAuth: true } // 標記需要登入才能看 (選填)
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

// === 全局導航守衛 (保全系統) ===
router.beforeEach((to, from, next) => {
  // 1. 檢查該路由是否需要驗證 (有沒有 requiresAuth 標記)
  if (to.meta.requiresAuth) {
    // 2. 檢查本地是否有 Token
    const token = localStorage.getItem('token')
    
    if (token) {
      // 有 Token，放行
      next()
    } else {
      // 沒 Token，踢去登錄頁
      next('/login')
    }
  } else {
    // 不需要驗證的頁面 (如 Login/Register)，直接放行
    next()
  }
})

export default router