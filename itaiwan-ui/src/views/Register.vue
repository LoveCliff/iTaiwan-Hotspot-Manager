<template>
  <div class="register-container">
    
    <div class="absolute-logo">iTaiwan 熱點</div>

    <div class="form-section">
      <div class="form-wrapper">
        <div class="header-text">
          <h2>創建新帳號 🚀</h2>
          <p class="sub-text">只需幾秒鐘，開始探索全台熱點</p>
        </div>

        <el-form 
          ref="ruleFormRef"
          :model="form"
          :rules="rules"
          label-position="top"
          size="large"
          status-icon
          @keyup.enter="submitForm(ruleFormRef)"
        >
          <el-form-item label="設定用戶名" prop="username">
            <el-input v-model="form.username" placeholder="例如：TaiwanUser123" :prefix-icon="User" />
          </el-form-item>

          <el-form-item label="電子信箱" prop="email">
            <el-input 
                v-model="form.email" 
                placeholder="例如：example@email.com" 
                :prefix-icon="Message" 
            />
          </el-form-item>

          <el-form-item label="設定密碼" prop="password">
            <el-input v-model="form.password" type="password" placeholder="建議包含英文與數字" show-password :prefix-icon="Lock" />
          </el-form-item>

          <el-form-item label="確認密碼" prop="confirmPassword">
            <el-input v-model="form.confirmPassword" type="password" placeholder="請再次輸入密碼" show-password :prefix-icon="CircleCheck" />
          </el-form-item>

          <el-form-item>
            <el-button type="primary" @click="submitForm(ruleFormRef)" :loading="loading" class="submit-btn">
              立即註冊
            </el-button>
          </el-form-item>
        </el-form>

        <div class="footer-links">
          <span>已經有帳號了？</span>
          <el-button link type="primary" @click="$router.push('/login')">直接登錄</el-button>
        </div>
      </div>
    </div>

    <div class="image-section">
      <div class="overlay">
        <h1>加入連接</h1>
        <p>與我們一起構建最便捷的熱點地圖。</p>
      </div>
    </div>

  </div>
</template>

<script setup>
import { reactive, ref } from 'vue'
import axios from 'axios'
import { useRouter } from 'vue-router'
import { ElMessage } from 'element-plus'
// 👇 記得引入 Message 圖標
import { User, Lock, CircleCheck, Message } from '@element-plus/icons-vue'

const router = useRouter()
const ruleFormRef = ref()
const loading = ref(false)

// 👇 新增 email 欄位
const form = reactive({ username: '', email: '', password: '', confirmPassword: '' })

const validatePass2 = (rule, value, callback) => {
  if (value === '') callback(new Error('請再次輸入密碼'))
  else if (value !== form.password) callback(new Error('兩次輸入的密碼不一致!'))
  else callback()
}

const rules = reactive({
  username: [
      { required: true, message: '請輸入用戶名', trigger: 'blur' }, 
      { min: 3, message: '長度至少 3 個字符', trigger: 'blur' }
  ],
  // 👇 新增 Email 驗證規則
  email: [
      { required: true, message: '請輸入電子信箱', trigger: 'blur' },
      { type: 'email', message: '請輸入正確的 Email 格式', trigger: ['blur', 'change'] }
  ],
  password: [
      { required: true, message: '請輸入密碼', trigger: 'blur' }, 
      { min: 6, message: '密碼長度不能少於 6 位', trigger: 'blur' }
  ],
  confirmPassword: [{ validator: validatePass2, trigger: 'blur' }]
})

const submitForm = async (formEl) => {
  if (!formEl) return
  await formEl.validate(async (valid) => {
    if (valid) {
      loading.value = true
      try {
        // 👇 修改這裡：包含 email 欄位，並使用相對路徑
        await axios.post('/api/Auth/register', { 
            username: form.username, 
            email: form.email,
            password: form.password 
        })
        
        ElMessage.success('註冊成功！請登錄')
        router.push('/login')
      } catch (error) {
        let errorMsg = '註冊失敗'
        if (error.response?.data) {
             // 處理後端回傳的錯誤 (例如 Email 已存在)
             if (error.response.data.message) errorMsg = error.response.data.message
             else if (Array.isArray(error.response.data)) errorMsg = error.response.data.map(e => e.description).join('; ')
             else errorMsg = JSON.stringify(error.response.data)
        }
        ElMessage.error(errorMsg)
      } finally {
        loading.value = false
      }
    }
  })
}
</script>

<style scoped>
/* 保持你的樣式完全不變 */
.register-container {
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
  color: #409EFF;
  z-index: 100;
  letter-spacing: 1px;
}

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

.image-section {
  flex: 1;
  background-image: url('https://images.unsplash.com/photo-1497366216548-37526070297c?q=80&w=1974&auto=format&fit=crop');
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
  background: linear-gradient(to top, rgba(0,0,0,0.8), rgba(0,0,0,0.1));
}

.overlay {
  position: relative;
  z-index: 1;
  color: white;
  margin-bottom: 40px;
  text-align: right;
}
.overlay h1 { font-size: 3rem; margin-bottom: 10px; font-weight: bold; }
.overlay p { font-size: 1.2rem; opacity: 0.9; }

@media (max-width: 768px) {
  .image-section { display: none; }
  .form-section { flex: 1; padding: 20px; }
}
</style>