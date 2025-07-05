# ✅ 1. 申請 Riot 開發者 API Key

1. 註冊 Riot 帳號並前往 Riot 開發者平台  https://developer.riotgames.com/
2. 申請開發者 Key（**開發用 Key 有效 24 小時**，正式環境需審核）
	-  API KEY : [[Ulysses/GitHub專案/未實施專案/Riot API Key|你申請的API KEY]]
3. 取得你的 `X-Riot-Token`，後續 API 都要用這個做身份驗證

📌 Step 1. 用 Riot ID 查 puuid
Simple :
```
GET https://asia.api.riotgames.com/riot/account/v1/accounts/by-riot-id/{gameName}/{tagLine}
Header:
X-Riot-Token: <你的 API Key>
```

GET 
```
https://asia.api.riotgames.com/riot/account/v1/accounts/by-riot-id/{你的id}/TW2
```

```json
{
    "puuid": "bemk1rOXSFHkuJO2M8c6WjzGo0YL-g-BdtAMk6FdbjKho3-j69Y8rVYYPM1BJuUrpYZn-puUMwBPkQ",
    "gameName": "深邃紅月",
    "tagLine": "tw2"
}
```

📌 Step 2. 用 puuid 查比賽列表 
GET  
✅要將asia.api.riotgames.com 更換成 **sea**.api.riotgames.com 
```
https://sea.api.riotgames.com/lol/match/v5/matches/by-puuid/{{puuid}}}/ids?start=0&count=
```

![[Ulysses/成長筆記本/資料工程師/附件圖片檔/Pasted image 20250705215341.png]]
![[Ulysses/成長筆記本/資料工程師/附件圖片檔/Pasted image 20250705223330.png]]

| 地區/伺服器   | Routing Region（用於 Match API） |
| -------- | ---------------------------- |
| 台灣、香港、越南 | **sea**（Southeast Asia）      |
| 韓國       | **asia**                     |
| 日本       | **asia**                     |
| 歐洲       | **europe**                   |
| 北美       | **americas**                 |


---

要開發一個像 OP.GG 這樣的遊戲數據分析平台，你可以把學習與開發分成以下幾大模組來規劃，以下是詳細的 **開發學習路線圖**，適合你這樣已有 C# 與資料工程背景的工程師：

### 🔧 一、系統架構規劃階段

**目標：理解 OP.GG 的系統組成與流量結構**

| 模組     | 內容說明                                                            |
| ------ | --------------------------------------------------------------- |
| 功能模擬   | 遊戲資料查詢、排行榜、使用者分析、API匯入、帳號系統、UI互動與圖表                             |
| 架構設計   | 分層架構（前端、後端、資料庫、API、爬蟲/匯入系統）                                     |
| 架構選擇   | 單頁式應用 SPA（React/Vue）或 ASP.NET MVC、.NET Web API、Redis 快取層、CDN 前置 |
| 資料來源分析 | 官方 API（如 Riot Games API）、非公開資料（爬蟲或第三方匯入）                        |

---

### 🌐 二、前端開發技能

|項目|技術學習|推薦資源|
|---|---|---|
|網頁框架|React（或 Next.js）/ Vue|[React 官方教學](https://chatgpt.com/c/f)、[Vue Mastery](https://chatgpt.com/c/f)|
|圖表呈現|Chart.js / Recharts / ECharts|[Recharts 文檔](https://chatgpt.com/c/f)|
|UI 設計|Tailwind CSS / Shadcn UI / Bootstrap|[Tailwind Cheat Sheet](https://chatgpt.com/c/f)|
|SEO / SSR|Next.js / Nuxt.js|[Next.js 教學](https://chatgpt.com/c/f)|

---

### 🧠 三、後端開發技能（C# or Node.js）

|項目|技術學習|推薦資源|
|---|---|---|
|API 架設|ASP.NET Core Web API / Node.js + Express|[官方教學](https://chatgpt.com/c/f)|
|快取設計|Redis（用來避免過度查詢 API）|[Redis 速成教學](https://chatgpt.com/c/f)|
|JWT 登入系統|ASP.NET Identity / Firebase Auth|[JWT 教學](https://chatgpt.com/c/f)|
|排程任務|Hangfire / Quartz.NET / Node-cron|[Hangfire 教學](https://chatgpt.com/c/f)|

---

### 🗃️ 四、資料與爬蟲系統（資料來源處理）

|    項目     |                       技術學習                       |       備註        | 執行  |                                           限制                                            |
| :-------: | :----------------------------------------------: | :-------------: | :-: | :-------------------------------------------------------------------------------------: |
| 官方 API 串接 |             Riot Games API（需申請 Key）              |    有限速，注意快取     |  ✅  | #### Rate Limits<br>20 requests every 1 seconds(s)  <br>100 requests every 2 minutes(s) |
| Python 爬蟲 | Requests + BeautifulSoup / Selenium / Playwright | 可抓戰績、ID、排行等非官方頁 |     |                                                                                         |
|   定時抓資料   |     Celery / Cron / .NET Background Service      |     定時更新資料庫     |     |                                                                                         |
|   資料清洗    |              Pandas / LINQ / Regex               |  處理錯誤資料或非結構化內容  |     |                                                                                         |

---

### 🛢️ 五、資料庫設計與分析

|項目|技術學習|重點|
|---|---|---|
|資料庫選型|PostgreSQL / SQL Server|支援 JSON/地理資料索引者佳|
|資料表設計|使用正規化 / 指標儲存 / Index 優化|分析用欄位要額外設計（KDA、WinRate）|
|資料倉儲（可選）|DuckDB / ClickHouse / BigQuery|做高效查詢用（可延伸）|

---

### 📈 六、數據分析與推薦系統（高階）

|項目|技術學習|說明|
|---|---|---|
|資料統計|Python Pandas / R|分析勝率、角色搭配等|
|ML 模型|Scikit-learn / TensorFlow（進階）|勝率預測、自動建議角色|
|使用者行為追蹤|GA4 / PostHog / Mixpanel|分析熱門頁面與互動行為|

---

### ☁️ 七、部署與運維

|項目|技術學習|推薦工具|
|---|---|---|
|雲端部署|Vercel / Netlify（前端）、Render / Fly.io / Linode（後端）|小專案可用免費層|
|Docker 容器化|Docker + Docker Compose|[Docker 官方指南](https://chatgpt.com/c/f)|
|CI/CD|GitHub Actions / Azure DevOps|自動部署前後端|
|監控|Uptime Kuma / Grafana / Prometheus|可簡單整合異常通知|

---
### 🧪 八、推薦開發順序（學習實作建議）

1. ✅ 先從 Riot API 資料匯入與儲存開始（用 .NET + Redis + PostgreSQL）    
2. ✅ 接著建立排行榜、查詢頁，開發 Web API + 前端頁面（React + Tailwind）    
3. ✅ 引入快取（Redis）與排程更新（Hangfire）    
4. ✅ 美化 UI、導入圖表呈現戰績分析    
5. ✅ 若有餘力，做角色推薦（簡易機器學習或統計模型）
    
---

如需實際起手專案範本、API 申請、或開發流程規劃，我可以幫你建立起步架構。你也可以指定 Riot API、後端語言等再進一步細化。

讓我知道你想從哪個模組先學，我可以給你[入門教學](https://chatgpt.com/c/f)、[專案架構模板](https://chatgpt.com/c/f)、或[Riot API 快速入門](https://chatgpt.com/c/f)。