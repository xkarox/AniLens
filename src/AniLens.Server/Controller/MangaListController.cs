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

    public override Task<ActionResult<IEnumerable<MangaListDto>>> GetAll(int page, int pageSize)
    {
        throw new NotImplementedException();
    }

    public override Task<ActionResult<MangaListDto>> Update(string id, MangaListDto item)
    {
        throw new NotImplementedException();
    }

    public override Task<ActionResult> Delete(string id)
    {
        throw new NotImplementedException();
    }

    public override Task<ActionResult<MangaListDto>> Add(MangaListDto item)
    {
        throw new NotImplementedException();
    }
}