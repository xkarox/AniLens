using Microsoft.AspNetCore.Mvc;

namespace AniLens.Server.Controller;

[ApiController]
public abstract class BaseController<T> : ControllerBase
{
    public abstract Task<IActionResult> Get(string id);
    public abstract Task<IActionResult> GetAll();
    public abstract Task<IActionResult> Update(T item);
    public abstract Task<IActionResult> Delete(string id);
    public abstract Task<IActionResult> Add(T item);
}