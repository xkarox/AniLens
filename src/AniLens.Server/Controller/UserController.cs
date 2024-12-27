using AniLens.Core.Extensions;
using AniLens.Core.Models;
using AniLens.Core.Services;
using AniLens.Server.Controller.Base;
using AniLens.Shared;
using AniLens.Shared.DTO;
using Microsoft.AspNetCore.Mvc;

namespace AniLens.Server.Controller;

[Route("api/[controller]")]
[ApiController]
public class UserController(UserService userService) : CrudController<UserDto, UpdateUserDto>
{
    private readonly UserService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public override async Task<ActionResult<UserDto>> Get(string id)
    {
        var result = await _userService.Get(id);
        return result.IsSuccess
            ? Ok(result.Data!)
            : NotFound(result.Error);
    }
    
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDto>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var result = await _userService.GetAll();
        return result.IsSuccess 
            ? Ok(result.Data!)
            : StatusCode(StatusCodes.Status500InternalServerError, result.Error);
    }
    
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateUserDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult<UpdateUserDto>> Update(string id, [FromBody]UpdateUserDto user)
    {
        var result = await _userService.UpdateUser(id, user);
        return result switch
        {
            { IsSuccess: true } => Ok(result.Data),
            { ErrorType: Error.NotFound } => NotFound(result.Error),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
        };
    }
    
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> Delete(string id)
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
    
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult<UserDto>> Add([FromBody] UserDto user)
    {
        var result = await _userService.AddUser(user);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = result.Data!.Id }, result.Data!)
            : StatusCode(StatusCodes.Status500InternalServerError, result.Error);
    }
}