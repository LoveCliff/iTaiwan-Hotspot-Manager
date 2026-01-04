import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import router from './router'
import axios from 'axios'
// === 新增：全局請求攔截器 (關鍵代碼) ===
// 每次發送請求之前，這段代碼都會自動執行
axios.interceptors.request.use(config => {
    // 如果請求的網址包含 'login' 或 'register'，就不帶 Token
  if (config.url.includes('login') || config.url.includes('register')) {
     return config
  }
  // 1. 從口袋 (localStorage) 裡拿出 Token
  const token = localStorage.getItem('token')
  
  // 2. 如果有 Token，就把它黏貼在請求頭 (Header) 上
  if (token) {
    // 注意：格式必須是 "Bearer <token>"
    config.headers.Authorization = `Bearer ${token}`
  }
  
  return config
}, error => {
  return Promise.reject(error)
})

//全局響應攔截器 
// 如果後端告訴我們 "Token 過期了" (401)，我們就自動踢回登錄頁
axios.interceptors.response.use(response => {
  return response
}, error => {
  if (error.response && error.response.status === 401) {
    // 清除過期的 token
    localStorage.removeItem('token')
    // 強制跳轉回登錄頁
    router.push('/login')
  }
  return Promise.reject(error)
})

const app = createApp(App)
app.use(router)
app.use(ElementPlus)
app.mount('#app')
