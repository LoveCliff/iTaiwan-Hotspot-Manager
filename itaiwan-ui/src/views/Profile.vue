<template>
  <el-card class="profile-card">
    <template #header>
      <div class="card-header">
        <h2>個人資料管理</h2>
      </div>
    </template>

    <el-tabs v-model="activeTab">
      <el-tab-pane label="基本資料" name="info">
        <el-form :model="infoForm" label-width="100px" style="max-width: 500px; margin-top: 20px;">
          
          <el-form-item label="頭像預覽">
            <el-avatar :size="100" :src="infoForm.avatarUrl || 'https://cube.elemecdn.com/3/7c/3ea6beec64369c2642b92c6726f1epng.png'" />
          </el-form-item>

          <el-form-item label="暱稱">
            <el-input v-model="infoForm.nickname" placeholder="請輸入暱稱" />
          </el-form-item>

          <el-form-item label="頭像網址">
            <el-input v-model="infoForm.avatarUrl" placeholder="請輸入圖片 URL" />
          </el-form-item>

          <el-form-item label="電話">
            <el-input v-model="infoForm.phoneNumber" placeholder="請輸入電話" />
          </el-form-item>

          <el-form-item>
            <el-button type="primary" @click="updateInfo" :loading="loading">保存修改</el-button>
          </el-form-item>
        </el-form>
      </el-tab-pane>

      <el-tab-pane label="安全性" name="security">
        <el-form :model="pwdForm" :rules="pwdRules" ref="pwdFormRef" label-width="100px" style="max-width: 500px; margin-top: 20px;">
          
          <el-form-item label="舊密碼" prop="oldPassword">
            <el-input v-model="pwdForm.oldPassword" type="password" show-password />
          </el-form-item>

          <el-form-item label="新密碼" prop="newPassword">
            <el-input v-model="pwdForm.newPassword" type="password" show-password />
          </el-form-item>

          <el-form-item label="確認密碼" prop="confirmPassword">
            <el-input v-model="pwdForm.confirmPassword" type="password" show-password />
          </el-form-item>

          <el-form-item>
            <el-button type="danger" @click="changePassword" :loading="loading">修改密碼</el-button>
          </el-form-item>
        </el-form>
      </el-tab-pane>
    </el-tabs>
  </el-card>
</template>

<script setup>
import { ref, onMounted, reactive } from 'vue'
import axios from 'axios' // 假設你已經配置好 axios 攔截器帶 Token
import { ElMessage } from 'element-plus'

const activeTab = ref('info')
const loading = ref(false)

// 表單數據
const infoForm = reactive({
  nickname: '',
  avatarUrl: '',
  phoneNumber: ''
})

const pwdForm = reactive({
  oldPassword: '',
  newPassword: '',
  confirmPassword: ''
})

const pwdFormRef = ref(null)

// 密碼驗證規則
const validatePass2 = (rule, value, callback) => {
  if (value === '') {
    callback(new Error('請再次輸入密碼'))
  } else if (value !== pwdForm.newPassword) {
    callback(new Error('兩次輸入密碼不一致!'))
  } else {
    callback()
  }
}

const pwdRules = {
  oldPassword: [{ required: true, message: '請輸入舊密碼', trigger: 'blur' }],
  newPassword: [{ required: true, message: '請輸入新密碼', trigger: 'blur' }, { min: 6, message: '密碼長度不能小於 6 位', trigger: 'blur' }],
  confirmPassword: [{ validator: validatePass2, trigger: 'blur' }]
}

// 1. 初始化：獲取當前資料
const fetchProfile = async () => {
  try {
    const res = await axios.get('/api/UserProfile')
    Object.assign(infoForm, res.data) // 自動填入數據
  } catch (error) {
    ElMessage.error('獲取個人資料失敗')
  }
}

// 2. 更新基本資料
const updateInfo = async () => {
  loading.value = true
  try {
    await axios.put('/api/UserProfile/update-info', infoForm)
    ElMessage.success('資料更新成功')
    // 可選：更新成功後，可以通知 Pinia Store 更新全域 User 狀態
  } catch (error) {
    ElMessage.error('更新失敗')
  } finally {
    loading.value = false
  }
}

// 3. 修改密碼
const changePassword = async () => {
  if (!pwdFormRef.value) return
  
  await pwdFormRef.value.validate(async (valid) => {
    if (valid) {
      loading.value = true
      try {
        await axios.post('/api/UserProfile/change-password', {
          oldPassword: pwdForm.oldPassword,
          newPassword: pwdForm.newPassword
        })
        ElMessage.success('密碼修改成功，請重新登入')
        
        // 建議：修密碼後強制登出
        // localStorage.removeItem('token')
        // router.push('/login')
        
        // 這裡僅清空表單
        pwdForm.oldPassword = ''
        pwdForm.newPassword = ''
        pwdForm.confirmPassword = ''
      } catch (error) {
        ElMessage.error(error.response?.data?.message || '修改密碼失敗')
      } finally {
        loading.value = false
      }
    }
  })
}

onMounted(() => {
  fetchProfile()
})
</script>

<style scoped>
.profile-card {
  max-width: 800px;
  margin: 20px auto;
}
</style>