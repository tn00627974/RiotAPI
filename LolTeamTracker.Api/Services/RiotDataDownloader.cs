namespace LolTeamTracker.Api.Services
{
    public class RiotDataDownloader
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IWebHostEnvironment _env;
        //private readonly HttpClient _httpClient;


        public RiotDataDownloader(IHttpClientFactory httpClientFactory , IWebHostEnvironment env)
        {
            //_httpClient = httpClientFactory.CreateClient(); // 使用命名客戶端也可
            _httpClientFactory = httpClientFactory;
            _env = env; 
        }

        // 下載 champion.json 的 C# 程式 => 放在 Static 資料夾
        public async Task DownloadLatestChampionJsonAsync()
        {
            string savePath = Path.Combine(_env.ContentRootPath, "Data", "Static", "champion.json");
            var client = _httpClientFactory.CreateClient();

            // 取得版本
            var versions = await client.GetFromJsonAsync<List<string>>("https://ddragon.leagueoflegends.com/api/versions.json");
            var latestVersion = versions.First();

            var lang = "zh_TW"; // 台服
            var url = $"https://ddragon.leagueoflegends.com/cdn/{latestVersion}/data/{lang}/champion.json";

            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
            await File.WriteAllTextAsync(savePath, content);
        }

    }
}
