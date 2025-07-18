﻿using LolTeamTracker.Api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace LolTeamTracker.Api.Services
{
    public class RiotApiService 
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly IHttpClientFactory _httpClientFactory; 

        public RiotApiService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
            _apiKey = _config["RiotApi:ApiKey"]; // 你的API KEY
            _baseUrl = _config["RiotApi:RegionBaseUrl"]; // https://sea.api.riotgames.com 網址
            _httpClient.DefaultRequestHeaders.Add("X-Riot-Token", _apiKey); 
        }

        // 用 Riot ID 查 puuid
        public async Task<string> GetPuuidAsync(string gameName, string tagLine)
        {
            try
            {
                var url = $"https://asia.api.riotgames.com/riot/account/v1/accounts/by-riot-id/{gameName}/{tagLine}";
                var res = await _httpClient.GetFromJsonAsync<JsonElement>(url);
                return res.GetProperty("puuid").GetString();
            }
            catch (HttpRequestException ex)
            {
                return $"API Error : {ex.StatusCode} - {ex.Message}";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // 查詢單場詳細資訊
        public async Task<string> GetMatchSummary(string matchId)
        {
            var client = _httpClientFactory.CreateClient();
            var url = $"{_baseUrl}/lol/match/v5/matches/{matchId}?api_key={_apiKey}";

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return json;
        }

        // 用 puuid 查比賽列表 count預設為10,最多100上限 ( API限制 )
        public async Task<List<string>> GetMatchIdsAsync(string puuid,int start = 0,int count = 10) 
        {
            var url = $"{_baseUrl}/lol/match/v5/matches/by-puuid/{puuid}/ids?start={start}&count={count}";
            return await _httpClient.GetFromJsonAsync<List<string>>(url);
        }

        /*         
         ---------------------- match ----------------------         
         */

        //用 matchId 查比賽列表細節
        public async Task<MatchSummary?> GetMatchSummaryAsync(string matchId, string puuid, string gameName, string tagLine)
        {
            var url = $"{_baseUrl}/lol/match/v5/matches/{matchId}"; // {matchId} : 遊戲對戰編號
            var data = await _httpClient.GetFromJsonAsync<JsonElement>(url);
            var participant = data.GetProperty("info").GetProperty("participants")
                                  .EnumerateArray()
                                  .FirstOrDefault(p => p.GetProperty("puuid").GetString() == puuid);

            if (participant.ValueKind == JsonValueKind.Undefined)
                return null;

            return new MatchSummary
            {
                GameName = gameName,
                TagLine = tagLine,
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
