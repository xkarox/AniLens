using AniLens.Core.Interfaces;
using AniLens.Shared;
using DevOne.Security.Cryptography.BCrypt;

namespace AniLens.Core.Services;

public class HashService : IHashService
{
    public Result<string> HashPassword(string password)
    {
        var salt = BCryptHelper.GenerateSalt();
        var hashedPassword = BCryptHelper.HashPassword(password, salt);

        return hashedPassword != null
            ? Result<string>.Success(hashedPassword)
            : Result<string>.Failure(Error.Internal.ToDescriptionString());
    }

    public bool CheckPassword(string password, string hashedPassword)
    {
        return BCryptHelper.CheckPassword(password, hashedPassword);
    }
}