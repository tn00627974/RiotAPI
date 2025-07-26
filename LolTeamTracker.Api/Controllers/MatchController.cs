using LolTeamTracker.Api.Models;
using LolTeamTracker.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LolTeamTracker.Api.Controllers;

[ApiController]
//[Route("[controller]")]
[Route("api/match")]
public class MatchController : ControllerBase
{
    private readonly RiotApiService _riot;

    public MatchController (RiotApiService riot)
    {
        _riot = riot;
    }

    /// <summary>
    /// ���o���w���a�����ɦC��
    /// </summary>
    /// <remarks>
    /// �d�ҡG
    /// GET /api/match/Faker/TW2
    /// </remarks>
    /// <param name="gameName">�l��v�W�١A�ҦpFaker</param>
    /// <param name="tagLine">�a�ϥN�X�A�Ҧp TW2</param>
    /// <param name="count">�j�M����</param>
    /// <returns></returns>
    [HttpGet("{gameName}/{tagLine}")]
    public async Task<IActionResult> GetMatchList(string gameName, string tagLine, int count = 50)
    {
        // �w�]0~100���o�W�LAPI����
        //if (count <= 0) { count = 50; }
        //else if (count > 100) { count = 100; }
        count = Math.Clamp(count, 50, 100);

        var puuid = await _riot.GetPuuidAsync(gameName, tagLine); 
        var matchIds = await _riot.GetMatchIdsAsync(puuid,0,count);
        var result = new List<MatchSummary>();

        foreach (var matchId in matchIds)
        {
            try
            {
                var summary = await _riot.GetMatchSummaryAsync(matchId, puuid, gameName, tagLine);
                if (summary != null)
                    result.Add(summary);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ū������ {matchId} ���ѡG{ex.Message}");
                    
            }
        }
        return this.Ok(new
        {
            count = result.Count,
            data = result
        });
    }


}
