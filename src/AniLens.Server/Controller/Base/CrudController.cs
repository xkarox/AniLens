using Microsoft.AspNetCore.Mvc;

namespace AniLens.Server.Controller.Base;

[ApiController]
public abstract class CrudController<TDto, TUpdateDto>(
    // ILogger<CrudController<T, TDto>> logger
    )
    : ControllerBase
    where TUpdateDto : class
    where TDto : class
{
    // protected readonly ILogger<CrudController<T, TDto>> Logger = logger;

    [HttpGet("{id}")]
    public abstract Task<ActionResult<TDto>> Get(string id);

    [HttpGet]
    public abstract Task<ActionResult<IEnumerable<TDto>>> GetAll();

    [HttpPut("{id}")]
    public abstract Task<ActionResult<TUpdateDto>> Update(string id, [FromBody]TUpdateDto item);

    [HttpDelete("{id}")]
    public abstract Task<ActionResult> Delete(string id);

    [HttpPost]
    public abstract Task<ActionResult<TDto>> Add(TDto item);
}