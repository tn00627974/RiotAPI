using System.Text.Json;

namespace WebApplication1
{
    public class RiotApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public RiotApiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _apiKey = _config["RiotApi:ApiKey"];
            _baseUrl = _config["RiotApi:RegionBaseUrl"];
            _httpClient.DefaultRequestHeaders.Add("X-Riot-Token", _apiKey);
        }

        // 用 Riot ID 查 puuid
        public async Task<string> GetPuuidAsync(string gameName, string tagLine)
        {
            var url = $"https://asia.api.riotgames.com/riot/account/v1/accounts/by-riot-id/{gameName}/{tagLine}";
            var res = await _httpClient.GetFromJsonAsync<JsonElement>(url);
            return res.GetProperty("puuid").GetString();
        }

        // 用 puuid 查比賽列表 count預設為10,最多100上限 ( API限制 )
        public async Task<List<string>> GetMatchIdsAsync(string puuid,int start = 0,int count = 10) 
        {
            var url = $"{_baseUrl}/lol/match/v5/matches/by-puuid/{puuid}/ids?start=0&count={count}";
            return await _httpClient.GetFromJsonAsync<List<string>>(url);
        }


        //用 puuid 查比賽列表細節
        public async Task<MatchSummary?> GetMatchSummaryAsync(string matchId, string puuid, string gameName, string tag)
        {
            var url = $"{_baseUrl}/lol/match/v5/matches/{matchId}";
            var data = await _httpClient.GetFromJsonAsync<JsonElement>(url);
            var participant = data.GetProperty("info").GetProperty("participants")
                                  .EnumerateArray()
                                  .FirstOrDefault(p => p.GetProperty("puuid").GetString() == puuid);

            if (participant.ValueKind == JsonValueKind.Undefined)
                return null;

            return new MatchSummary
            {
                GameName = gameName,
                TagLine = tag,
                Champion = participant.GetProperty("championName").GetString(),
                Kills = participant.GetProperty("kills").GetInt32(),
                Deaths = participant.GetProperty("deaths").GetInt32(),
                Assists = participant.GetProperty("assists").GetInt32(),
                Win = participant.GetProperty("win").GetBoolean(),
                GameDate = DateTimeOffset.FromUnixTimeMilliseconds(data.GetProperty("info").GetProperty("gameStartTimestamp").GetInt64()).DateTime
            };
        }

    }
}
