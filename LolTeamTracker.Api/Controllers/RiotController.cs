using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System;
using LolTeamTracker.Api.Services;
using System.Net.Http;
using System.Xml.Linq;

namespace LolTeamTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RiotController : ControllerBase
    {
        private readonly RiotApiService _riot;
        private readonly RiotDataDownloader _riotDataDownloader;
        private readonly IWebHostEnvironment _env;

        public RiotController(RiotApiService riot ,RiotDataDownloader riotDataDownloader, IWebHostEnvironment env)
        {
            _riot = riot;
            _riotDataDownloader = riotDataDownloader;
            _env = env;
        }

        /// <summary>
        /// 查該玩家的 puuid
        /// </summary>
        /// <param name="gameName">遊戲名稱</param>
        /// <param name="tagLine"> #標籤 </param>
        /// <returns></returns>
        [HttpGet("puuid")]
        public async Task<IActionResult> GetPuuid(string gameName, string tagLine)
        {
            var puuid = await _riot.GetPuuidAsync(gameName, tagLine);
            return Ok(puuid);
        }

        /// <summary>
        /// 查詢單場詳細資訊
        /// </summary>
        /// <param name="matchId">場次編號</param>
        /// <returns></returns>
        [HttpGet("matchId")]
        public async Task<IActionResult> GetMatchSummary(string matchId)
        {
            var result = await _riot.GetMatchSummary(matchId);
            if (result == null)
                return StatusCode(500, "查詢失敗");

            return Content(result, "application/json");
        }

        /// <summary>
        /// 查該玩家的比賽列表 : 最多100場,預設10場
        /// </summary>
        /// <param name="puuid"></param>
        /// <param name="count">資料比數</param>
        /// <returns></returns>
        [HttpGet("match-ids")]
        public async Task<IActionResult> GetMatchIds(string puuid, int count = 10)
        {
            var matchId = await _riot.GetMatchIdsAsync(puuid, count);
            return Ok(matchId);
        }


        /// <summary>
        /// 下載所有最新的json資料
        /// </summary>
        [HttpGet("download-all-json")]
        public async Task<IActionResult> DownloadChampionData()
        {
            try
            {               
                await _riotDataDownloader.DownloadLatestChampionJsonAsync();
                await _riotDataDownloader.DownloadLatestItemJsonAsync();
                await _riotDataDownloader.DownloadLatestSummonerJsonAsync();
                await _riotDataDownloader.DownloadLatestRunesReforgedJsonAsync();

                return Ok(new { message = "下載成功" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "下載失敗", error = ex.Message });
            }
        }
    }
}
