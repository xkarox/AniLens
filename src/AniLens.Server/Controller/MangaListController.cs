using AniLens.Core.Interfaces;
using AniLens.Core.Models;
using AniLens.Server.Controller.Base;
using AniLens.Shared;
using AniLens.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AniLens.Server.Controller;

[ApiController]
[Route("api/[controller]")]
public class MangaListController(IMangaListService mangaListService) : CrudController<MangaListDto, MangaListDto>
{
    public override async Task<ActionResult<MangaListDto>> Get([FromQuery] string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest(Error.Parameter.ToDescriptionString());

        var result = await mangaListService.GetList(id);

        return result.IsSuccess
            ? Ok(result.Data!)
            : NotFound(result.Error);
    }

    public override async Task<ActionResult<IEnumerable<MangaListDto>>> GetAll([FromQuery]int page = 1, [FromQuery]int pageSize = 20)
    {
        var query = new MangaListQueryDto();
        var result = await mangaListService.Query(query, page, pageSize);
        
        return result.IsSuccess 
            ? Ok(result.Data!)
            : StatusCode(StatusCodes.Status500InternalServerError,
                Error.Internal.ToDescriptionString());
    }

    public override async Task<ActionResult<MangaListDto>> Update(string id, [FromBody] MangaListDto item)
    {
        var result = await mangaListService.UpdateList(item);
        return result switch
        {
            { IsSuccess: true } => Ok(result.Data!),
            { ErrorType: Error.NotFound } => NotFound(),
            { ErrorType: Error.Internal } => StatusCode(
                StatusCodes.Status500InternalServerError,
                Error.Internal.ToDescriptionString())
        };
    }

    public override async Task<ActionResult> Delete([FromQuery] string id)
    {
        if (string.IsNullOrEmpty(id))
            return BadRequest(Error.Parameter.ToDescriptionString());
        
        var result = await mangaListService.DeleteList(id);
        return result switch
        {
            { IsSuccess: true } => Ok(result.Data!),
            { ErrorType: Error.NotFound } => NotFound(),
            { ErrorType: Error.Internal } => StatusCode(
                StatusCodes.Status500InternalServerError,
                Error.Internal.ToDescriptionString())
        };
    }

    public override async Task<ActionResult<MangaListDto>> Add([FromBody] MangaListDto item)
    {
        var result = await mangaListService.CreateList(item);
        return result.IsSuccess
            ? Ok(result.Data!)
            : StatusCode(StatusCodes.Status500InternalServerError,
                Error.Internal.ToDescriptionString());
    }
}