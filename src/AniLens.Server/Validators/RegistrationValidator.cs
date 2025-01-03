using AniLens.Core.Interfaces;
using AniLens.Shared.DTO;
using FluentValidation;

namespace AniLens.Server.Validators;

public class RegistrationValidator : AbstractValidator<RegisterDto>
{
    public RegistrationValidator(IConfiguration configuration)
    {
        var credentialPolicy = configuration.GetSection("RegistrationPolicy");
        var passwordPolicy = credentialPolicy.GetSection("Password");
        var usernamePolicy = credentialPolicy.GetSection("Username");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(passwordPolicy.GetValue<int>("MinLength"))
                .WithMessage($"Password must be at least {passwordPolicy.GetValue<int>("MinLength")} characters long.")
            .MaximumLength(passwordPolicy.GetValue<int>("MaxLength"))
                .WithMessage($"Password must not exceed {passwordPolicy.GetValue<int>("MaxLength")} characters.")
            .Must(password => !passwordPolicy.GetValue<bool>("RequiredNumber") || password.Any(char.IsDigit))
                .WithMessage("Password must contain at least one number.")
            .Must(password => !passwordPolicy.GetValue<bool>("RequiredSymbol") || 
                              password.Any(c => passwordPolicy.GetSection("AllowedSymbols").Get<string[]>().Contains(c.ToString())))
                .WithMessage("Password must contain at least one allowed symbol.")
            .Must(password => !passwordPolicy.GetValue<bool>("RequiredUppercase") || password.Any(char.IsUpper))
                .WithMessage("Password must contain at least one uppercase letter.")
            .Must(password => !passwordPolicy.GetValue<bool>("RequiredLowercase") || password.Any(char.IsLower))
                .WithMessage("Password must contain at least one lowercase letter.")
            .Must(password => password.All(c => char.IsLetterOrDigit(c) || 
                              passwordPolicy.GetSection("AllowedSymbols").Get<string[]>().Contains(c.ToString())))
                .WithMessage("Password contains invalid characters.");

        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(usernamePolicy.GetValue<int>("MinLength"))
                .WithMessage($"Username must be at least {usernamePolicy.GetValue<int>("MinLength")} characters long.")
            .MaximumLength(usernamePolicy.GetValue<int>("MaxLength"))
                .WithMessage($"Username must not exceed {usernamePolicy.GetValue<int>("MaxLength")} characters.")
            .Must(username => !usernamePolicy.GetValue<bool>("RequiredNumber") || username.Any(char.IsDigit))
                .WithMessage("Username must contain at least one number.")
            .Must(username => !usernamePolicy.GetValue<bool>("RequiredSymbol") || 
                              username.Any(c => usernamePolicy.GetSection("AllowedSymbols").Get<string[]>().Contains(c.ToString())))
                .WithMessage("Username must contain at least one allowed symbol.")
            .Must(username => username.All(c => char.IsLetterOrDigit(c) || 
                              usernamePolicy.GetSection("AllowedSymbols").Get<string[]>().Contains(c.ToString())))
                .WithMessage("Username contains invalid characters.");
    }  
}