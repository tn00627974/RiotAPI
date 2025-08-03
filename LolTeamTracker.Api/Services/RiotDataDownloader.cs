using System;

namespace LolTeamTracker.Api.Services
{
    public class RiotDataDownloader
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;
        

        public RiotDataDownloader(IHttpClientFactory httpClientFactory , IWebHostEnvironment env)
        {
            _httpClientFactory = httpClientFactory;
            _env = env; 
        }

        /// <summary>
        /// 下載所有json的資料 => 放在 Data\\Static 資料夾
        /// </summary>
        /// <param name="latestVersion">最新版本號</param>
        /// <param name="fileName">檔案名稱</param>
        /// <param name="lang">地區格式</param>
        /// <returns></returns>
        private async Task<string> DownloadDataFileAsync(string latestVersion, string fileName ,string lang= "zh_TW")
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://ddragon.leagueoflegends.com/cdn/{latestVersion}/data/{lang}/{fileName}";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            string savePath = Path.Combine(_env.ContentRootPath, "Data", "Static", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
            await File.WriteAllTextAsync(savePath, content);
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return $"{now}:{fileName}下載版本{latestVersion}成功!";        
        }

        /// <summary>
        /// 取得最新的版本號
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetLatestVersionAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var versions = await client.GetFromJsonAsync<List<string>>("https://ddragon.leagueoflegends.com/api/versions.json");
            return versions!.First(); // 取得最新版本(不會有 null)
        }

        /// <summary>
        /// champion.json 英雄資料
        /// </summary>
        /// <returns></returns>
        public async Task<string> DownloadLatestChampionJsonAsync()
        {
            var version = await GetLatestVersionAsync();
            var result = await DownloadDataFileAsync(version, "champion.json");
            return result;
        }

        /// <summary>
        /// item.json 物品資料
        /// </summary>
        /// <returns></returns>
        public async Task<string> DownloadLatestItemJsonAsync()
        {
            var version = await GetLatestVersionAsync();
            var result = await DownloadDataFileAsync(version, "item.json");
            return result;
        }

        /// <summary>
        /// summoner.json 召喚師技能資料
        /// </summary>
        /// <returns></returns>
        public async Task<string> DownloadLatestSummonerJsonAsync()
        {
            var version = await GetLatestVersionAsync();
            var result = await DownloadDataFileAsync(version, "summoner.json");
            return result;
        }

        /// <summary>
        /// runesReforged.json 符文資料
        /// </summary>
        /// <returns></returns>
        public async Task<string> DownloadLatestRunesReforgedJsonAsync()
        {
            var version = await GetLatestVersionAsync();
            var result =  await DownloadDataFileAsync(version, "runesReforged.json");
            return result;
        }
    }
}
