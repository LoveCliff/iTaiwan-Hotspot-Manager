import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  server: {
    host: '0.0.0.0', // 允許局域網訪問 (可選)
    port: 5173,      // 1. 強制指定端口為 5173
    strictPort: true // 2. 如果端口被佔用，直接報錯退出，而不是自動變更
  }
})
