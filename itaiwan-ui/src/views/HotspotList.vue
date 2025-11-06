<template>
  <el-card>
    <template #header>
      <h2>iTaiwan热点列表</h2>
    </template>
    <el-table :data="hotspots" border style="width: 100%">
      <el-table-column prop="Name" label="热点名称" width="200" />
      <el-table-column prop="Address" label="地址" />
      <el-table-column prop="Latitude" label="纬度" width="150" />
      <el-table-column prop="Longitude" label="经度" width="150" />
    </el-table>
  </el-card>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'

const hotspots = ref([])

onMounted(async () => {
  const response = await axios.get('https://localhost:5143/api/Hotspots')
  hotspots.value = response.data
})
</script>