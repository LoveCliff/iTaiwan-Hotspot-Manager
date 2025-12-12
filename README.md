# iTaiwan-Hotspot-Manager

## 專案說明
此為**前後端分離架構**的應用程式，基於 `.NET 9.0 + Vue 3` 技術棧，用於解析、儲存與視覺化展示「iTaiwan 公共區域免費無線上網熱點」資料。後端透過 MySQL 管理熱點數據並提供 API 服務，前端則以 Vue 3 呈現熱點列表與詳細資訊。


## 資料來源
政府開放資料平台：[iTaiwan 公共區域免費無線上網熱點資訊](https://data.gov.tw/dataset/126997)


## 資料準備
1. 從上述連結下載**JSON 格式**的熱點資料，並更名為 `IpSelect_tw.json`。
2. 於後端子專案 `ItaiwanAPI` 中，手動建立 `App_Data` 目錄，並將 `IpSelect_tw.json` 放入該目錄。


## 開發環境需求
- **後端**：.NET 9.0 SDK、MySQL 資料庫
- **前端**：Node.js（v16+）


## 啟動步驟
### 1. 克隆專案
```bash
git clone (https://github.com/LoveCliff/iTaiwan-Hotspot-Manager.git)
cd iTaiwan-Hotspot-Manager
```

### 2.啟動後端服務（ItaiwanAPI）
```bash
cd ItaiwanAPI
# 配置資料庫連接：修改 appsettings.json 中的 MySqlConnection（替換帳號密碼）
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
# 後端 API 預設地址：(http://localhost:5143)
```

### 3. 啟動前端介面（itaiwan-ui）
```bash
cd itaiwan-ui
npm install
npm run dev
# 前端頁面預設地址：(http://localhost:5173)
```

## 專案结构
```bash
iTaiwan-Hotspot-Manager/
├── ItaiwanAPI/        # 後端 API 服務模組
│   ├── App_Data/      # 熱點資料來源目錄
│   ├── Controllers/   # API 控制器（提供熱點數據介面）
│   ├── Data/          # 資料庫上下文（EF Core 配置）
│   ├── Models/        # 熱點資料模型
│   └── Services/      # 業務服務（JSON 導入資料庫等邏輯）
├── itaiwan-ui/        # 前端 Vue 應用模組
│   ├── src/
│   │   ├── views/     # 頁面組件（如熱點列表頁）
│   │   ├── router/    # 路由配置
│   │   └── main.js    # 前端入口文件
└── README.md          # 專案說明文件
```
