<template>
  <el-card class="box-card">
    <template #header>
      <div class="header-actions">
        <div class="title-area">
          <h2>iTaiwan 熱點列表</h2>
        </div>

        <div class="search-bar">
          <el-radio-group v-model="viewMode" size="default" style="margin-right: 15px;">
            <el-radio-button label="list">
              <el-icon><List /></el-icon> 列表
            </el-radio-button>
            <el-radio-button label="map">
              <el-icon><MapLocation /></el-icon> 地圖
            </el-radio-button>
          </el-radio-group>

          <el-input 
            v-model="searchKeyword" 
            placeholder="輸入名稱或地址..." 
            style="width: 200px; margin-right: 10px;" 
            clearable
            @clear="handleSearch"
            @keyup.enter="handleSearch"
          />
          <el-button type="primary" @click="handleSearch">搜索</el-button>
          
          <el-button type="success" @click="getLocationAndSort" :loading="loadingLocation">
            <el-icon><Location /></el-icon> 離我最近
          </el-button>
        </div>
      </div>
    </template>

    <div v-if="viewMode === 'list'" style="height: 100%;">
      <el-table :data="hotspots" border style="width: 100%; min-height: 700px;" v-loading="loading">
        <el-table-column prop="name" label="熱點名稱" width="200" />
        <el-table-column prop="address" label="地址" />
        <el-table-column label="距離" width="120" align="center">
          <template #default="scope">
            <span v-if="scope.row.distanceKm" style="color: #409EFF; font-weight: bold;">
              {{ scope.row.distanceKm }} km
            </span>
            <span v-else>-</span>
          </template>
        </el-table-column>

        <el-table-column label="操作" width="150" align="center">
          <template #default="scope">
            <el-tooltip effect="dark" :content="isFavorite(scope.row.id) ? '取消收藏' : '加入收藏'" placement="top">
              <el-button 
                circle 
                :type="isFavorite(scope.row.id) ? 'warning' : 'default'"
                @click="toggleFavorite(scope.row)"
              >
                <el-icon>
                  <component :is="isFavorite(scope.row.id) ? 'StarFilled' : 'Star'" />
                </el-icon>
              </el-button>
            </el-tooltip>

            <el-tooltip effect="dark" content="開啟地圖導航" placement="top">
              <el-button circle plain type="primary" @click="openGoogleMaps(scope.row)">
                <el-icon><Position /></el-icon>
              </el-button>
            </el-tooltip>
          </template>
        </el-table-column>
      </el-table>

      <div class="pagination-container">
        <el-pagination
          v-model:current-page="currentPage"
          v-model:page-size="pageSize"
          :total="total"
          layout="total, prev, pager, next"
          background
          @current-change="handlePageChange"
        />
      </div>
    </div>

    <div v-else class="map-container">
      <l-map 
        ref="map" 
        v-if="mapCenter"
        v-model:zoom="zoom" 
        :center="mapCenter" 
        :use-global-leaflet="false"
      >
        <l-tile-layer
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          layer-type="base"
          name="OpenStreetMap"
        ></l-tile-layer>

        <l-marker 
          v-for="item in mapDisplayHotspots" 
          :key="item.id" 
          :lat-lng="[item.latitude || item.Latitude, item.longitude || item.Longitude]"
        >
          <l-popup>
            <div style="text-align: center; min-width: 150px;">
              <h3 style="margin: 5px 0;">{{ item.name }}</h3>
              <p style="margin: 5px 0 10px; color: #666; font-size: 13px;">{{ item.address }}</p>
              
              <div style="display: flex; justify-content: center; gap: 8px;">
                <el-button 
                  size="small" 
                  type="primary" 
                  @click="openGoogleMaps(item)"
                >
                  導航
                </el-button>

                <el-button 
                  size="small" 
                  :type="isFavorite(item.id) ? 'warning' : 'default'"
                  @click="toggleFavorite(item)"
                >
                  {{ isFavorite(item.id) ? '已收藏' : '收藏' }}
                </el-button>
              </div>
            </div>
          </l-popup>
        </l-marker>
      </l-map>
    </div>

  </el-card>
</template>

<script setup>
import { ref, onMounted, computed, watch } from 'vue'
import axios from 'axios'
import { ElMessage } from 'element-plus'
import { Location, List, MapLocation, Star, StarFilled, Position } from '@element-plus/icons-vue'
import "leaflet/dist/leaflet.css"
import { LMap, LTileLayer, LMarker, LPopup } from "@vue-leaflet/vue-leaflet"

// === 狀態變數 ===
const hotspots = ref([])
const searchKeyword = ref('')
const loading = ref(false)
const loadingLocation = ref(false)
const currentPage = ref(1)
const favoriteIds = ref(new Set()) // 這裡存 ID
const pageSize = ref(15)
const total = ref(0)
const userCoords = ref({ lat: null, lon: null })

const viewMode = ref('list')
const zoom = ref(13)
const mapCenter = ref([25.0330, 121.5654]) 

// === 計算屬性：地圖顯示優化 ===
// 防止地圖模式下渲染 5000 個點卡死，這裡只取前 419 個有效坐標
// 這是「直接渲染」模式下保證不卡的關鍵
const mapDisplayHotspots = computed(() => {
  if (viewMode.value !== 'map') return []
  
  return hotspots.value
    .filter(h => {
        const lat = h.latitude || h.Latitude
        const lng = h.longitude || h.Longitude
        return lat && lng && !isNaN(parseFloat(lat)) && !isNaN(parseFloat(lng))
    })
    .slice(0, 419) // 限制數量，保證流暢
})

// === API ===
const fetchData = async () => {
  loading.value = true
  try {
    const isMapMode = viewMode.value === 'map'
    // 地圖模式稍微多拿一點數據，列表模式只拿一頁
    const limit = isMapMode ? 500 : pageSize.value
    
    const params = {
      page: isMapMode ? 1 : currentPage.value,
      pageSize: limit,
      keyword: searchKeyword.value
    }
    
    if (userCoords.value.lat) {
      params.lat = userCoords.value.lat
      params.lon = userCoords.value.lon
    }

    const response = await axios.get('http://localhost:5143/api/Hotspots', { params })
    const result = response.data
    const items = Array.isArray(result) ? result : (result.items || [])
    
    hotspots.value = items
    
    if (!isMapMode && result.totalCount) {
      total.value = result.totalCount
    }
  } catch (error) {
    console.error(error)
    ElMessage.error('獲取數據失敗')
  } finally {
    loading.value = false
  }
}

// === 收藏功能 (已修復變色問題) ===
const fetchFavoriteIds = async () => {
  try {
    const res = await axios.get('http://localhost:5143/api/favorites') 
    // 假設後端返回對象數組，轉成 ID 集合
    favoriteIds.value = new Set(res.data.map(item => item.id))
  } catch (error) {
    console.error('加載收藏失敗')
  }
}

// 判斷是否收藏
const isFavorite = (id) => {
    return favoriteIds.value.has(id)
}

const toggleFavorite = async (item) => {
  const id = item.id
  const isFav = favoriteIds.value.has(id)
  
  try {
    // 為了讓視圖更新，我們複製一個新的 Set
    const newSet = new Set(favoriteIds.value)
    
    if (isFav) {
      // 這裡請替換成你真實的取消收藏 API
      await axios.delete(`http://localhost:5143/api/favorites/${id}`)
      newSet.delete(id)
      ElMessage.success('已取消收藏')
    } else {
      // 這裡請替換成你真實的加入收藏 API
      await axios.post(`http://localhost:5143/api/favorites/${id}`)
      newSet.add(id)
      ElMessage.success('收藏成功')
    }
    
    // 重新賦值，觸發 Vue 更新（這就是變色的關鍵）
    favoriteIds.value = newSet
    
  } catch (error) {
    if (error.response?.status === 401) {
      ElMessage.warning('請先登錄')
    } else {
      ElMessage.error('操作失敗')
    }
  }
}

// === 其他功能 ===
const handleSearch = () => { currentPage.value = 1; fetchData() }
const handlePageChange = () => fetchData()

const getLocationAndSort = () => {
  if (!navigator.geolocation) return ElMessage.warning('不支持定位')
  loadingLocation.value = true
  
  navigator.geolocation.getCurrentPosition(
    (pos) => {
      userCoords.value = { lat: pos.coords.latitude, lon: pos.coords.longitude }
      mapCenter.value = [pos.coords.latitude, pos.coords.longitude]
      zoom.value = 14 // 拉近
      loadingLocation.value = false
      ElMessage.success('已定位')
      currentPage.value = 1
      fetchData()
    },
    (err) => { loadingLocation.value = false; ElMessage.error('定位失敗') }
  )
}

const openGoogleMaps = (item) => {
  const lat = item.latitude || item.Latitude
  const lng = item.longitude || item.Longitude
  const name = item.name
  window.open(`https://www.google.com/maps/dir/?api=1&destination=${lat},${lng}&destination_place_id=${encodeURIComponent(name)}`, '_blank')
}

// 切換模式時重新獲取數據
watch(viewMode, () => {
    hotspots.value = [] // 先清空，避免閃爍
    fetchData()
})

onMounted(() => {
  fetchData()
  fetchFavoriteIds() // 初始加載收藏列表
})
</script>

<style scoped>
.header-actions { display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; }
.search-bar { display: flex; align-items: center; margin-top: 10px; }
.pagination-container { margin-top: 20px; display: flex; justify-content: flex-end; }
.map-container {
  height: 70vh; 
  width: 100%;
  border: 1px solid #dcdfe6;
  z-index: 1;
}
</style>