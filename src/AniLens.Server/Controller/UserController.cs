using AniLens.Core.Extensions;
using AniLens.Core.Interfaces;
using AniLens.Core.Models;
using AniLens.Core.Services;
using AniLens.Server.Controller.Base;
using AniLens.Shared;
using AniLens.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;

namespace AniLens.Server.Controller;

[Route("api/[controller]")]
[ApiController]
public class UserController(IUserService userService) : CrudController<UserDto, UpdateUserDto>
{
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public override async Task<ActionResult<UserDto>> Get(string id)
    {
        var result = await userService.Get(id);
        return result.IsSuccess
            ? Ok(result.Data!)
            : NotFound(result.Error);
    }
    
    [Authorize(Roles = "User")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserDto>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
    {
        var result = await userService.GetAll();
        return result.IsSuccess 
            ? Ok(result.Data!)
            : StatusCode(StatusCodes.Status500InternalServerError, result.Error);
    }
    
    [Authorize]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateUserDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult<UpdateUserDto>> Update(string id, [FromBody]UpdateUserDto user)
    {
        var result = await userService.UpdateUser(id, user);
        return result switch
        {
            { IsSuccess: true } => Ok(result.Data),
            { ErrorType: Error.NotFound } => NotFound(result.Error),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
        };
    }
    
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult> Delete(string id)
    {
        var result = await userService.DeleteUser(id);
        return result switch
        {
            { IsSuccess: true } => Ok(),
            { ErrorType: Error.NotFound } => NotFound(result.Error),
            { ErrorType: Error.Parameter } => BadRequest(result.Error),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
        };
    }
    
    [Authorize(Roles = "Admin, Moderator")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserDto))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public override async Task<ActionResult<UserDto>> Add([FromBody] UserDto user)
    {
        var result = await userService.AddUser(user);
        return result.IsSuccess
            ? CreatedAtAction(nameof(Get), new { id = result.Data!.Id }, result.Data!)
            : StatusCode(StatusCodes.Status500InternalServerError, result.Error);
    }
}