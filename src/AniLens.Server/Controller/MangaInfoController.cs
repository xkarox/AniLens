using AniLens.Core.Interfaces;
using AniLens.Core.Models;
using AniLens.Shared;
using AniLens.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AniLens.Server.Controller;

[ApiController]
[Route("api/[controller]")]
public class MangaInfoController(IMangaInfoService mangaInfoService) : ControllerBase
{
    private readonly IMangaInfoService _mangaInfoService = mangaInfoService;
    
    [AllowAnonymous]
    [HttpGet("search")]
    [Consumes("application/json")]
    // [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<Manga>>> Search(
        [FromQuery] string query)
    {
        var result = await mangaInfoService.Search(query);
        if (result.IsFailure)
            return StatusCode(StatusCodes.Status500InternalServerError, Error.Internal.ToDescriptionString());

        return Ok(result.Data);
    }
}