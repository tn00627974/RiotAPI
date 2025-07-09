namespace LolTeamTracker.Services
{
    public class RiotDataDownloader
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public RiotDataDownloader(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // 下載 champion.json 的 C# 程式 => 放在 Static 資料夾
        public async Task DownloadLatestChampionJsonAsync(string savePath)
        {
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
