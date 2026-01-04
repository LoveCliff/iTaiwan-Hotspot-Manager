<template>
  <div class="login-container">
    
    <div class="absolute-logo">iTaiwan 熱點</div>

    <div class="image-section">
      <div class="overlay">
        <h1>歡迎回來</h1>
        <p>探索全台公共無線網絡，隨時隨地保持連線。</p>
      </div>
    </div>

    <div class="form-section">
      <div class="form-wrapper">
        <div class="header-text">
          <h2>登錄系統 👋</h2>
          <p class="sub-text">請輸入您的帳號密碼以繼續</p>
        </div>

        <el-form 
          ref="loginFormRef"
          :model="form" 
          :rules="rules"
          label-position="top"
          size="large"
          @keyup.enter="handleLogin(loginFormRef)"
        >
          <el-form-item label="帳號" prop="account">
            <el-input 
                v-model="form.account" 
                placeholder="請輸入 使用者名稱 或 Email" 
                :prefix-icon="User"
            />
          </el-form-item>

          <el-form-item label="密碼" prop="password">
            <el-input 
              v-model="form.password" 
              type="password" 
              placeholder="請輸入密碼" 
              show-password 
              :prefix-icon="Lock"
            />
          </el-form-item>

          <el-form-item>
            <el-button 
              type="primary" 
              :loading="loading" 
              class="submit-btn" 
              @click="handleLogin(loginFormRef)"
            >
              登 錄
            </el-button>
          </el-form-item>
        </el-form>

        <div class="footer-links">
          <span>還沒有帳號？</span>
          <el-button link type="primary" @click="$router.push('/register')">
            立即註冊
          </el-button>
        </div>
      </div>
    </div>

  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import axios from 'axios'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
import { User, Lock } from '@element-plus/icons-vue'

const router = useRouter()
const loginFormRef = ref()
const loading = ref(false)

// 👇 修改這裡：把 username 改成 account
const form = reactive({ account: '', password: '' })

const rules = reactive({
  account: [{ required: true, message: '請輸入帳號或 Email', trigger: 'blur' }],
  password: [{ required: true, message: '請輸入密碼', trigger: 'blur' }]
})

const handleLogin = async (formEl) => {
  if (!formEl) return
  
  await formEl.validate(async (valid) => {
    if (valid) {
      loading.value = true
      try {
        // 👇 修改這裡：
        // 1. 使用相對路徑 /api/... (依賴 vite proxy)
        // 2. 傳送 account 欄位給後端
        const res = await axios.post('/api/Auth/login', {
            account: form.account,
            password: form.password
        })
        
        localStorage.setItem('token', res.data.token)
        ElMessage.success('登錄成功')
        router.push('/') 
        
      } catch (err) {
        console.error(err)
        // 顯示後端具體回傳的錯誤訊息 (如果有的話)
        const msg = err.response?.data?.message || '登錄失敗，請檢查帳號密碼'
        ElMessage.error(msg)
      } finally {
        loading.value = false
      }
    }
  })
}
</script>

<style scoped>
/* 保持你原本的樣式不變 */
.login-container {
  display: flex;
  height: 100vh;
  width: 100%;
  overflow: hidden; 
}

.absolute-logo {
  position: absolute;
  top: 30px;
  left: 40px;
  font-size: 24px;
  font-weight: 800;
  color: #fff;
  z-index: 100;
  letter-spacing: 1px;
  text-shadow: 0 2px 4px rgba(0,0,0,0.3);
}

.image-section {
  flex: 1;
  background-image: url('https://images.unsplash.com/photo-1519681393784-d8e5b5a45742?q=80&w=2070&auto=format&fit=crop');
  background-size: cover;
  background-position: center;
  position: relative;
  display: flex;
  flex-direction: column;
  justify-content: flex-end;
  padding: 40px;
}

.image-section::before {
  content: '';
  position: absolute;
  top: 0; left: 0; right: 0; bottom: 0;
  background: linear-gradient(to bottom, rgba(0,0,0,0.3), rgba(0,0,0,0.8));
}

.overlay {
  position: relative;
  z-index: 1;
  color: white;
  margin-bottom: 40px;
}
.overlay h1 { font-size: 3rem; margin-bottom: 10px; font-weight: bold; }
.overlay p { font-size: 1.2rem; opacity: 0.9; }

.form-section {
  flex: 1;
  display: flex;
  justify-content: center;
  align-items: center;
  background-color: #ffffff;
  padding: 40px;
}

.form-wrapper {
  width: 100%;
  max-width: 420px;
}

.header-text { margin-bottom: 30px; }
.header-text h2 { font-size: 2rem; margin-bottom: 10px; color: #333; }
.sub-text { color: #888; }
.submit-btn { width: 100%; padding: 20px 0; font-size: 16px; font-weight: bold; margin-top: 10px; }
.footer-links { text-align: center; margin-top: 20px; font-size: 14px; color: #666; }

@media (max-width: 768px) {
  .image-section { display: none; }
  .form-section { flex: 1; padding: 20px; }
  .absolute-logo { color: #409EFF; text-shadow: none; }
}
</style>