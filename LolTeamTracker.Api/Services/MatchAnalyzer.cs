using LolTeamTracker.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Signing;
using System.Diagnostics;
using System.Text.Json;

namespace LolTeamTracker.Api.Services
{
    public class MatchAnalyzer
    {
        private readonly RiotApiService _riotApiService;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;


        public MatchAnalyzer(RiotApiService riotApiService , IConfiguration config ,IWebHostEnvironment env,HttpClient httpClient)
        {
            _riotApiService = riotApiService;
            _env = env;
            _httpClient = httpClient;
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _apiKey = _config["RiotApi:ApiKey"] ?? throw new InvalidOperationException("RiotApi:ApiKey is not configured.");
            _baseUrl = _config["RiotApi:RegionBaseUrl"] ?? throw new InvalidOperationException("RiotApi:RegionBaseUrl is not configured.");
            _httpClient.DefaultRequestHeaders.Add("X-Riot-Token", _apiKey);
        }

        /*         
         ---------------------- match ----------------------         
         */

        /// <summary>
        /// 用 matchId 查比賽列表細節
        /// </summary>
        /// <param name="matchId">遊戲場次編號</param>
        /// <param name="puuid"></param>
        /// <param name="gameName">遊戲名稱</param>
        /// <param name="tagLine">#標籤</param>
        /// <returns></returns>
        public async Task<MatchSummary?> GetMatchSummaryAsync(string matchId, string puuid, string gameName, string tagLine)
        {
            var url = $"{_baseUrl}/lol/match/v5/matches/{matchId}"; // {matchId} : 遊戲對戰編號
            var data = await _httpClient.GetFromJsonAsync<JsonElement>(url);
            var participant = data.GetProperty("info").GetProperty("participants")
                                  .EnumerateArray()
                                  .FirstOrDefault(p => p.GetProperty("puuid").GetString() == puuid);

            if (participant.ValueKind == JsonValueKind.Undefined) // 如果找不到對應的參與者
                return null;

            // 處理UTC國際時間格式 => 轉為台灣時間 24小時格式
            var dateTimeUtc = DateTimeOffset.FromUnixTimeMilliseconds(data.GetProperty("info").GetProperty("gameStartTimestamp").GetInt64()).UtcDateTime;
            var taiwanZone = TimeZoneInfo.ConvertTimeFromUtc(dateTimeUtc, TimeZoneInfo.FindSystemTimeZoneById("Taipei Standard Time"));
            var taiwanTime = taiwanZone.ToString("yyyy/MM/dd HH:mm:ss");

            return new MatchSummary
            {
                GameName = gameName,
                TagLine = tagLine,
                Champion = participant.GetProperty("championName").GetString(),
                Kills = participant.GetProperty("kills").GetInt32(),
                Deaths = participant.GetProperty("deaths").GetInt32(),
                Assists = participant.GetProperty("assists").GetInt32(),
                Win = participant.GetProperty("win").GetBoolean(),
                GameDate = taiwanTime
            };
        }

        /// <summary>
        /// 載入 JSON 檔案中的隊伍成員資料，並取得每位成員的比賽摘要列表
        /// </summary>
        /// <returns></returns>
        public async Task<List<MatchSummary>> GetMatchSummariesTeamsAsync()
        {
            var players = await LoadTeamFromJson("team.json");
            var allResults = new List<MatchSummary>();

            // 要先用 RiotApiService 取得成員的 puuid 
            foreach (var player in players)
            {
                // 取得玩家的 puuid
                var summaries = await GetMatchSummariesPlayerAsync(player.gameName, player.tagLine ,10);
                if (summaries != null)
                    allResults.AddRange(summaries); // 將每個玩家的比賽列表加入到總結果中

                #region Old 
                //var puuid = await _riotApiService.GetPuuidAsync(player.gameName, player.tagLine);
                //if (string.IsNullOrEmpty(puuid))
                //{
                //    Console.WriteLine($"無法取得 {player.gameName} 的 puuid");
                //    continue;
                //}
                //// 取得玩家的比賽列表
                //var matchIds = await _riotApiService.GetMatchIdsAsync(puuid, 0, 10);
                //foreach (var matchId in matchIds)
                //{
                //    // 分析每場比賽
                //    var summary = await _matchAnalyzer.GetMatchSummaryAsync(matchId, puuid, player.gameName, player.tagLine);
                //    if (summary != null)
                //    {
                //        // 在這裡處理比賽摘要，例如儲存到資料庫或進行其他分析
                //        Console.WriteLine($"分析比賽 {matchId} 成功");
                //        allResults.Add(summary);
                //    }
                //}
                #endregion
            }
            return allResults;
        }

        /// <summary>
        /// 邏輯化 : 取得指定玩家的比賽列表
        /// </summary>
        /// <param name="gameName">遊戲名稱</param>
        /// <param name="tagLine">#標籤</param>
        /// <param name="count">搜尋場次</param>
        /// <returns></returns>
        public async Task<List<MatchSummary>> GetMatchSummariesPlayerAsync(string gameName, string tagLine, int count = 50)
        {
            var result = new List<MatchSummary>();
            var puuid = await _riotApiService.GetPuuidAsync(gameName, tagLine);
            var matchIds = await _riotApiService.GetMatchIdsAsync(puuid, 0, count);

            foreach (var matchId in matchIds)
            {
                try
                {
                    var summary = await GetMatchSummaryAsync(matchId, puuid, gameName, tagLine);
                    if (summary != null)
                        result.Add(summary);
                }
                catch (Exception ex)
                {
                    // log or skip ?
                }
            }

            return result;
        }

        /// <summary>
        /// 載入 JSON 檔案中的成員資料
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        private async Task<List<PlayerInfo>> LoadTeamFromJson(string fileName)
        {
            string savePath = Path.Combine(_env.ContentRootPath, "Data", "Team", fileName); // Path : Data/Team/team.json
            if (!File.Exists(savePath))
            {
                throw new FileNotFoundException($"檔案 {fileName} 不存在於 {savePath}");
            }
            string json = await File.ReadAllTextAsync(savePath);
            // 這裡可以解析 JSON 並返回所需的資料
            var team = JsonSerializer.Deserialize<List<PlayerInfo>>(json);
            return team ?? new List<PlayerInfo>();
        }
    }
}
