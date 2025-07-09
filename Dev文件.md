
# 將專案資料夾 `WebApplication1` 改成 `LolTeamTracker.WebApi`


## ✅ 解法一：使用 Visual Studio 修復

### 1️⃣ 開啟 `.sln` 檔（雖然會顯示 "Unloaded project"）    
	打開之後會出現以下錯誤 : 因為原專案還是連結到舊路徑

![[Pasted image 20250709223335.png]]
### 2️⃣ 右鍵 → `Remove` 原本載入失敗的專案    
	先將專案移除

![[Pasted image 20250709223411.png]]

### 3️⃣ 點選 `加入` > 現有專案 開啟 `LolTeamTracker.csproj`


![[Pasted image 20250709223801.png]]




### 4️⃣重新儲存 `.sln`

![[Pasted image 20250709223824.png]]


> ✅ **這是最簡單也最推薦的方法**，因為 Visual Studio 會自動幫你修正 `.sln` 的參考路徑。