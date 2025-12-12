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

    <div v-if="viewMode === 'list'">
      <el-table :data="hotspots" border style="width: 100%; min-height: 500px;" v-loading="loading">
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
          v-for="item in hotspots" 
          :key="item.id" 
          :lat-lng="[item.latitude, item.longitude]"
        >
          <l-popup>
            <div style="text-align: center;">
              <h3 style="margin: 5px 0;">{{ item.name }}</h3>
              <p style="margin: 5px 0; color: #666;">{{ item.address }}</p>
              <p v-if="item.distanceKm" style="color: #409EFF; font-weight: bold;">
                距您 {{ item.distanceKm }} km
              </p>
            </div>
          </l-popup>
        </l-marker>
        
        <l-marker v-if="userCoords.lat" :lat-lng="[userCoords.lat, userCoords.lon]">
           <l-popup>我原本的位置</l-popup>
        </l-marker>
      </l-map>
    </div>

  </el-card>
</template>

<script setup>
import { ref, onMounted, computed,watch } from 'vue'
import axios from 'axios'
import { ElMessage } from 'element-plus'
import { Location, List, MapLocation } from '@element-plus/icons-vue' // 引入新圖標

// 引入 Leaflet 地圖組件
import "leaflet/dist/leaflet.css"
import { LMap, LTileLayer, LMarker, LPopup } from "@vue-leaflet/vue-leaflet"

// === 狀態變數 ===
const hotspots = ref([])
const searchKeyword = ref('')
const loading = ref(false)
const loadingLocation = ref(false)
const currentPage = ref(1)
const pageSize = ref(10) // 地圖模式下或許可以考慮顯示更多，但目前先保持一致
const total = ref(0)
const userCoords = ref({ lat: null, lon: null })

// 新增：視圖模式 ('list' 或 'map')
const viewMode = ref('list')
// 新增：地圖縮放和中心點 (默認台北 101 附近)
const zoom = ref(13)
const mapCenter = ref([25.0330, 121.5654]) 

// === 核心獲取數據方法 ===
const fetchData = async () => {
  loading.value = true
  try {
    // === 判斷當前模式 ===
    const isMapMode = viewMode.value === 'map'
    
    // 如果是地圖模式，我們請求 1000 筆 (或是更多，視你後端性能而定)
    // 如果是列表模式，我們只請求 10 筆 (pageSize.value)
    const requestPageSize = isMapMode ? 1000 : pageSize.value
    
    // 如果是地圖模式，強制作為第 1 頁
    const requestPage = isMapMode ? 1 : currentPage.value

    const params = {
      page: requestPage, 
      pageSize: requestPageSize,
      keyword: searchKeyword.value
    }

    // 附帶座標 (如果有)
    if (userCoords.value.lat) {
      params.lat = userCoords.value.lat
      params.lon = userCoords.value.lon
    }

    const response = await axios.get('http://localhost:5143/api/Hotspots', { params })
    
    // 兼容處理 (防止後端格式不同)
    if (response.data.items) {
      hotspots.value = response.data.items
      
      // 注意：只有在列表模式下，才需要更新「總頁數」
      // 這樣切換回列表時，分頁條才不會壞掉
      if (!isMapMode) {
        total.value = response.data.totalCount
      }
    } else {
      hotspots.value = response.data
    }
    
    // 如果切換到了地圖模式且有數據，自動定位到第一筆
    if (isMapMode && hotspots.value.length > 0) {
      // 這裡加個簡單判斷，如果地圖中心還在默認位置，就移動過去
      // 或者你可以選擇每次都移動
       const first = hotspots.value[0]
       // mapCenter.value = [first.latitude, first.longitude] // (可選：自動移動鏡頭)
    }

  } catch (error) {
    console.error(error)
    ElMessage.error('獲取數據失敗')
  } finally {
    loading.value = false
  }
}
// === 新增：監聽視圖切換 ===
// 當 viewMode 改變時 (從 list 變 map，或反之)，自動重新抓取數據
watch(viewMode, () => {
  fetchData()
})

// === 事件處理 ===
const handleSearch = () => {
  currentPage.value = 1
  fetchData()
}

const handlePageChange = (newPage) => {
  currentPage.value = newPage
  fetchData()
}

const getLocationAndSort = () => {
  if (!navigator.geolocation) return ElMessage.warning('不支持定位')
  loadingLocation.value = true
  
  navigator.geolocation.getCurrentPosition(
    (pos) => {
      // 1. 保存用戶位置
      userCoords.value.lat = pos.coords.latitude
      userCoords.value.lon = pos.coords.longitude
      
      // 2. 更新地圖中心到用戶位置
      mapCenter.value = [pos.coords.latitude, pos.coords.longitude]
      zoom.value = 15 // 拉近鏡頭

      loadingLocation.value = false
      ElMessage.success('已定位')
      
      // 3. 重新獲取數據 (後端會計算距離並排序)
      currentPage.value = 1
      fetchData()
    },
    (err) => {
      loadingLocation.value = false
      ElMessage.error('定位失敗')
    }
  )
}

onMounted(() => {
  fetchData()
})
</script>

<style scoped>
.header-actions {
  display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap;
}
.search-bar { display: flex; align-items: center; margin-top: 10px; }
.pagination-container { margin-top: 20px; display: flex; justify-content: flex-end; }

/* 地圖容器樣式 */
.map-container {
  height: 600px; /* 必須給高度，否則地圖不顯示 */
  width: 100%;
  border: 1px solid #dcdfe6;
  border-radius: 4px;
  overflow: hidden;
}
</style>