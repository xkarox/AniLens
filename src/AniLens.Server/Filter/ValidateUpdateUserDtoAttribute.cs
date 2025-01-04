using AniLens.Shared;
using AniLens.Shared.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AniLens.Server.Filter;

public class ValidateUpdateUserDtoAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ActionArguments.TryGetValue("user", out var dto) || dto is not UpdateUserDto updateUserDto)
        {
            context.Result = new BadRequestObjectResult(Error.Internal.ToDescriptionString());
            return;
        }

        var validator = context.HttpContext.RequestServices.GetRequiredService<IValidator<UpdateUserDto>>();
        var validationResult = validator.Validate(updateUserDto);
        
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );
                
            context.Result = new BadRequestObjectResult(errors);
            return;
        }
    }
}