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
        /// 根據遊戲名稱和標籤獲取puuid
        /// </summary>
        /// <param name="gameName">遊戲名稱</param>
        /// <param name="tagLine">#標籤</param>
        /// <returns></returns>
        [HttpGet("players/puuid")]
        public async Task<IActionResult> GetPuuid(string gameName, string tagLine)
        {
            var puuid = await _riot.GetPuuidAsync(gameName, tagLine);
            return Ok(puuid);
        }

        /// <summary>
        /// 根據玩家的puuid獲取遊戲名稱和標籤
        /// </summary>
        /// <param name="puuid"></param>
        /// <returns></returns>
        [HttpGet("players/{puuid}")]
        public async Task<IActionResult> GetGameName(string puuid)
        {
            var playerInfo = await _riot.GetGameNameAsync(puuid);
            return Ok(playerInfo);
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
        /// 查詢單場詳細資訊 (含時間軸)
        /// </summary>
        /// <param name="matchId">場次編號</param>
        /// <returns></returns>
        [HttpGet("matchId-timeline")]
        public async Task<IActionResult> GetMatchIdsTimeList(string matchId)
        {
            var result = await _riot.GetMatchSummaryTimeLine(matchId);
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
                var resultList = new List<string>();

                #region Old : 若失敗就只回傳單一成功與失敗
                //resultList.Add(await _riotDataDownloader.DownloadLatestChampionJsonAsync());
                //resultList.Add(await _riotDataDownloader.DownloadLatestItemJsonAsync());
                //resultList.Add(await _riotDataDownloader.DownloadLatestSummonerJsonAsync());
                //resultList.Add(await _riotDataDownloader.DownloadLatestRunesReforgedJsonAsync());
                #endregion

                #region 進階 : 先下載並返回成功與失敗的結果
                async Task TryDownload(Func<Task<string>> downloadFunc, string name)
                {
                    try
                    {
                        var result = await downloadFunc();
                        resultList.Add($"{name}: ✅ {result}");
                    }
                    catch (Exception ex)
                    {
                        resultList.Add($"{name}: ❌ 錯誤 - {ex.Message}");
                    }
                }

                await TryDownload(_riotDataDownloader.DownloadLatestChampionJsonAsync, "Champion");
                await TryDownload(_riotDataDownloader.DownloadLatestItemJsonAsync, "Item");
                await TryDownload(_riotDataDownloader.DownloadLatestSummonerJsonAsync, "Summoner");
                await TryDownload(_riotDataDownloader.DownloadLatestRunesReforgedJsonAsync, "Runes");
                #endregion

                return Ok(new
                {
                    message = "所有檔案處理完畢",
                    results = resultList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { 
                    message = "下載失敗", 
                    error = ex.Message 
                });
            }
        }
    }
}
