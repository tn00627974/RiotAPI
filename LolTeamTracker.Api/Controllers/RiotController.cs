using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System;
using LolTeamTracker.Api.Services;
using System.Net.Http;

namespace LolTeamTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RiotController : ControllerBase
    {
        private readonly RiotApiService _riot;
        private readonly RiotDataDownloader _riotDataDownloader;
        private readonly IWebHostEnvironment _env;

        public RiotController(RiotDataDownloader riotDataDownloader, IWebHostEnvironment env)
        {
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
        /// 下載最新的 champion.json 資料
        /// </summary>
        [HttpGet("download")]
        public async Task<IActionResult> DownloadChampionData()
        {
            try
            {
                //string savePath = _env.ContentRootPath; // 專案根目錄
                await _riotDataDownloader.DownloadLatestChampionJsonAsync();
                return Ok(new { message = "下載成功", path = "Data/Static/champion.json" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "下載失敗", error = ex.Message });
            }
        }
    }
}
