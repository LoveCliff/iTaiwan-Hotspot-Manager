# iTaiwan-Hotspot-Manager 🇹🇼

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)
![Vue](https://img.shields.io/badge/Vue.js-3.0-green)
![MySQL](https://img.shields.io/badge/MySQL-8.0-orange)

## 📖 專案說明

**iTaiwan-Hotspot-Manager** 是一個現代化的前後端分離全端應用程式，旨在提供全台「iTaiwan 公共區域免費無線上網熱點」的視覺化地圖與個人化管理服務。

本專案不僅僅是資料的展示，更整合了完整的 **會員系統**。用戶可以註冊帳戶、收藏常用的熱點、並在列表與地圖模式間自由切換。後端採用高效的 .NET 9.0 處理數據與身分驗證，前端則使用 Vue 3 搭配 Element Plus 打造流暢的響應式介面。

## ✨ 主要功能 (Features)

### 🗺️ 熱點探索
* **雙模式瀏覽**：支援「列表視圖」與「地圖模式」一鍵切換，滿足不同查找需求。
* **即時導航**：利用Google Maps導航功能，快速規劃前往熱點的路線。
* **詳細資訊**：查看熱點的具體位置、服務單位與連接方式。

### 👤 會員系統 (User System)
* **身份驗證**：完整的註冊與登錄機制（基於 JWT Token 安全驗證）。
* **個人資料管理**：
    * 修改個人暱稱。
    * 更新頭像（支援圖片 URL 預覽）。
    * 綁定或修改聯絡電話。
    * 修改帳戶密碼。

### ❤️ 個人化收藏
* **加入/取消收藏**：將常去的熱點加入個人最愛。
* **收藏管理**：在專屬頁面查看並管理已收藏的熱點清單。

## 🛠️ 技術棧 (Tech Stack)

### Backend (後端)
* **Framework**: .NET 9.0 (ASP.NET Core Web API)
* **Database**: MySQL 8.0
* **ORM**: Entity Framework Core (Code First)
* **Auth**: ASP.NET Core Identity + JWT (JSON Web Token)

### Frontend (前端)
* **Framework**: Vue 3 (Composition API)
* **Build Tool**: Vite
* **UI Library**: Element Plus
* **Routing**: Vue Router
* **HTTP Client**: Axios
* **Map Integration**: Leaflet， Google Maps API 
  
## 📸 系統截圖 (Screenshots)
本專案包含多張系統操作截圖，請點擊下方連結查看：

👉 [**瀏覽所有系統截圖 (View all screenshots)**](./screenshots/README.md)
---
### 1. 資料準備
1. 前往 [政府開放資料平台]：[iTaiwan 公共區域免費無線上網熱點資訊](https://data.gov.tw/dataset/5962)
2. 將檔案更名為 `IpSelect_tw.json`。
3. 於 `ItaiwanAPI` 專案根目錄下建立 `App_Data` 資料夾，並將檔案放入：

### 2. 環境需求
* **.NET SDK**: 9.0 或更高版本
* **Node.js**: v16.0 或更高版本
* **MySQL Server**: 確保服務已啟動

### 3. 啟動後端 (ItaiwanAPI)
```bash
cd ItaiwanAPI

# 1. 設定資料庫
# 請打開 appsettings.json 修改 ConnectionStrings 中的 User Id 和 Password
# 確保 Jwt:Key 已設定（用於生成 Token）

# 2. 執行資料庫遷移 (建立 Table)
dotnet ef migrations add InitialCreate
dotnet ef database update

# 3. 啟動服務
dotnet run


## 開發環境需求
- **後端**：.NET 9.0 SDK、MySQL 資料庫
- **前端**：Node.js（v16+）


## 啟動步驟
### 1. 克隆專案
```bash
git clone (https://github.com/LoveCliff/iTaiwan-Hotspot-Manager.git)
cd iTaiwan-Hotspot-Manager

```
### 4. 前端環境配置 (Google Maps API)
本專案使用 Google Maps 進行地圖展示與導航，請確保您擁有有效的 Google Maps API Key。

1. 在 `itaiwan-ui` 根目錄下建立 `.env` 檔案 (或修改現有的 `.env`)。
2. 加入您的 API Key：
   ```env
   VITE_GOOGLE_MAPS_API_KEY=您的_Google_Maps_API_Key
   
### 5. 啟動前端 (itaiwan-ui)
cd itaiwan-ui

 1. 安裝依賴
npm install

 2. 啟動開發伺服器
npm run dev

前端頁面預設位址：http://localhost:5173


### 專案结构
```bash
iTaiwan-Hotspot-Manager/
├── ItaiwanAPI/               # Backend (.NET 9)
│   ├── App_Data/             # 原始 JSON 資料
│   ├── Controllers/          # API 控制器 (Auth, Favorites, Hotspots, UserProfile)
│   ├── Data/                 # DB Context 與 Migrations
│   ├── Models/               # 資料模型與 DTOs
│   ├── Services/             # 商業邏輯服務
│   └── Program.cs            # 程式入口與 DI 配置
│
├── itaiwan-ui/               # Frontend (Vue 3)
│   ├── src/
│   │   ├── api/              # Axios 封裝與 API 呼叫
│   │   ├── assets/           # 靜態資源
│   │   ├── components/       # 共用組件
│   │   ├── router/           # 路由設定
│   │   ├── views/            # 頁面 (Login, Register, Profile, Map, List)
│   │   └── main.js           # Vue 入口
│   └── vite.config.js        # Vite 配置 (包含 Proxy 設定)
└── README.md                 # 專案說明文件
