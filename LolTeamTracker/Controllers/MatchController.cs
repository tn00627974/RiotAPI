using Microsoft.AspNetCore.Mvc;

namespace LolTeamTracker.Api.Controllers;

[ApiController]
//[Route("[controller]")]
[Route("api/match")]
public class MatchController : ControllerBase
{
    private readonly RiotApiService _riot;

    public MatchController(RiotApiService riot)
    {
        _riot = riot;
    }


    /// <summary>
    /// ���o���w���a�����ɦC�� : Simple > {gameName}:Faker {tagLine}:TW2
    /// </summary>
    /// <param name="gameName"></param>
    /// <param name="tagLine"></param>
    /// <returns></returns>
    [HttpGet("{gameName}/{tagLine}")]
    public async Task<IActionResult> GetMatchList(string gameName, string tagLine)
    {
        var puuid = await _riot.GetPuuidAsync(gameName, tagLine);
        var matchIds = await _riot.GetMatchIdsAsync(puuid);
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
