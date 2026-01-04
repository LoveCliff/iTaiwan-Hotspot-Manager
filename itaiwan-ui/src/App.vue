<template>
  <div>
    <div v-if="!isAuthPage" class="nav">
      <div class="nav-content">
        <span class="logo">iTaiwan 管理系統</span>
        
        <div class="user-info" style="display: flex; align-items: center; gap: 15px;">
          
          <router-link to="/hotspots" style="text-decoration: none;">
            <el-button link type="primary">🏠 首頁</el-button>
          </router-link>

          <router-link to="/favorites" style="text-decoration: none;">
            <el-button link type="warning">⭐我的收藏❤️</el-button>
          </router-link>

          <el-button link type="primary" @click="$router.push('/profile')">
          <el-icon><User /></el-icon>個人資料</el-button>

          <el-button link type="danger" @click="logout">🚪退出</el-button>
        </div>
        </div>
    </div>

    <router-view />
  </div>
</template>

<script setup>
import { computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { User } from '@element-plus/icons-vue'

const route = useRoute()
const router = useRouter()

// 判斷當前路徑是否為登錄或註冊頁
const isAuthPage = computed(() => {
  return ['/login', '/register'].includes(route.path)
})

const logout = () => {
  localStorage.removeItem('token')
  router.push('/login')
}
</script>

<style>
/* 這裡的樣式只會影響那個灰條，不會影響登錄頁 */
.nav {
  height: 60px;
  background: #ffffff;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  position: relative;
  z-index: 10;
}

.nav-content {
  max-width: 1200px;
  margin: 0 auto;
  height: 100%;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 20px;
}

.logo {
  font-size: 20px;
  font-weight: bold;
  color: #409EFF;
}
</style>