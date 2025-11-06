import { createRouter, createWebHistory } from 'vue-router'
import HotspotList from '../views/HotspotList.vue'

const routes = [
  {
    path: '/',
    name: 'HotspotList',
    component: HotspotList
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router