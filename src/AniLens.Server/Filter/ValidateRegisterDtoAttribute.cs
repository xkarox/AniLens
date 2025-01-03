using System.Text;
using System.Text.Json;
using AniLens.Shared;
using AniLens.Shared.DTO;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AniLens.Server.Filter;

public class ValidateRegisterDtoAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ActionArguments.TryGetValue("registerDto", out var dto) || dto is not RegisterDto registerDto)
        {
            context.Result = new BadRequestObjectResult(Error.Internal.ToDescriptionString());
            return;
        }

        var validator = context.HttpContext.RequestServices.GetRequiredService<IValidator<RegisterDto>>();
        var validationResult = validator.Validate(registerDto);
        
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
