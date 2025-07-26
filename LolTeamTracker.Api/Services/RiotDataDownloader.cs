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

        // 下載所有json的資料 => 放在 Data\\Static 資料夾
        private async Task DownloadDataFileAsync(string latestVersion, string fileName ,string lang= "zh_TW")
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"https://ddragon.leagueoflegends.com/cdn/{latestVersion}/data/{lang}/{fileName}";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            string savePath = Path.Combine(_env.ContentRootPath, "Data", "Static", fileName);
            Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
            await File.WriteAllTextAsync(savePath, content);
        }

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
        public async Task DownloadLatestChampionJsonAsync()
        {
            var version = await GetLatestVersionAsync();
            await DownloadDataFileAsync(version, "champion.json");
        }

        /// <summary>
        /// item.json 物品資料
        /// </summary>
        /// <returns></returns>
        public async Task DownloadLatestItemJsonAsync()
        {
            var version = await GetLatestVersionAsync();
            await DownloadDataFileAsync(version, "item.json");
        }

        /// <summary>
        /// summoner.json 召喚師技能資料
        /// </summary>
        /// <returns></returns>
        public async Task DownloadLatestSummonerJsonAsync()
        {
            var version = await GetLatestVersionAsync();
            await DownloadDataFileAsync(version, "summoner.json");
        }

        /// <summary>
        /// runesReforged.json 符文資料
        /// </summary>
        /// <returns></returns>
        public async Task DownloadLatestRunesReforgedJsonAsync()
        {
            var version = await GetLatestVersionAsync();
            await DownloadDataFileAsync(version, "runesReforged.json");
        }



    }
}
