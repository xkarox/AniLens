using AniLens.Core.Models;
using AniLens.Server.Services;
using AniLens.Shared;
using Microsoft.AspNetCore.Mvc;

namespace AniLens.Server.Controller;

[Route("api/[controller]")]
[ApiController]
public class UserController(UserService userService) : BaseController<User>
{
    private readonly UserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public override async Task<IActionResult> Get(string id)
    {
        var result = await _userService.Get(id);
        return result.IsSuccess
            ? Ok(result.Data)
            : NotFound(result.Error);
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<User>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<IActionResult> GetAll()
    {
        var result = await _userService.GetAll();
        return result.IsSuccess 
            ? Ok(result.Data)
            : StatusCode(StatusCodes.Status500InternalServerError, result.Error);
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(User))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<IActionResult> Update([FromBody] User user)
    {
        var result = await _userService.UpdateUser(user);
        return result switch
        {
            { IsSuccess: true } => Ok(result.Data),
            { ErrorType: Error.NotFound } => NotFound(result.Error),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
        };
    }
    
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<IActionResult> Delete(string id)
    {
        var result = await _userService.DeleteUser(id);
        return result switch
        {
            { IsSuccess: true } => Ok(),
            { ErrorType: Error.NotFound } => NotFound(result.Error),
            { ErrorType: Error.Parameter } => BadRequest(result.Error),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
        };
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(User))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<IActionResult> Add([FromBody] User user)
    {
        var result = await _userService.AddUser(user);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = user.Id }, user)
            : StatusCode(StatusCodes.Status500InternalServerError, result.Error);
    }
}