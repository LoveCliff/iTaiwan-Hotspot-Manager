<template>
  <div style="padding: 20px;">
    <h2>❤️ 我的收藏夾</h2>

    <el-table :data="favorites" border style="width: 100%; margin-top: 20px;">
      <el-table-column prop="id" label="ID" width="80" />
      <el-table-column prop="name" label="熱點名稱" />
      <el-table-column prop="address" label="地址" />
      
      <el-table-column label="操作" width="120" align="center">
        <template #default="scope">
          <el-button 
            type="info" 
            size="small"
            @click="handleRemove(scope.row.id)"
          >
            取消收藏
          </el-button>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { ElMessage } from 'element-plus'

const favorites = ref([])

// 載入我的收藏
const loadFavorites = async () => {
  try {
    const res = await axios.get('/api/favorites')
    favorites.value = res.data
  } catch (error) {
    ElMessage.error('無法載入收藏列表')
  }
}

// 移除收藏
const handleRemove = async (id) => {
  try {
    await axios.delete(`/api/favorites/${id}`)
    ElMessage.success('已移除')
    // 移除成功後，重新載入列表，或者直接從前端數組刪除
    loadFavorites() 
  } catch (error) {
    ElMessage.error('移除失敗')
  }
}

onMounted(() => {
  loadFavorites()
})
</script>