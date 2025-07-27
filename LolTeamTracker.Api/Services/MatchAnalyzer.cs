using LolTeamTracker.Api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace LolTeamTracker.Api.Services
{
    public class MatchAnalyzer
    {
        private readonly RiotApiService _riotApiService;
        private readonly IWebHostEnvironment _env;
        private readonly HttpClient _httpClient;


        public MatchAnalyzer(RiotApiService riotApiService, IWebHostEnvironment env,HttpClient httpClient)
        {
            _riotApiService = riotApiService;
            _env = env;
            _httpClient = httpClient;
        }

        // RunAsync 功能未完
        public async Task RunAsync()
        {
            var players = await LoadTeamFromJson("team.json"); 
            // 要先用 RiotApiService 取得成員的 puuid 
            foreach (var player in players)
            {
                // 取得玩家的 puuid
                var puuid = await _riotApiService.GetPuuidAsync(player.gameName, player.tagLine);
                if (string.IsNullOrEmpty(puuid))
                {
                    Console.WriteLine($"無法取得 {player.gameName} 的 puuid");
                    continue;
                }
                // 取得玩家的比賽列表
                var matchIds = await _riotApiService.GetMatchIdsAsync(puuid, 0, 10);
                foreach (var matchId in matchIds)
                {
                    // 分析每場比賽
                    var summary = await _riotApiService.GetMatchSummaryAsync(matchId, puuid, player.gameName, player.tagLine);
                    if (summary != null)
                    {
                        // 在這裡處理比賽摘要，例如儲存到資料庫或進行其他分析
                        Console.WriteLine($"分析比賽 {matchId} 成功");
                    }
                    else
                    {
                        Console.WriteLine($"分析比賽 {matchId} 失敗");
                    }
                }
            }
        }

        /// <summary>
        /// 載入 JSON 檔案中的成員資料
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        private async Task<List<PlayerInfo>> LoadTeamFromJson(string fileName)
        {
            string savePath = Path.Combine(_env.ContentRootPath, "Data", "Team", fileName);
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
