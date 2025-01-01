using System.Security;
using AniLens.Shared;

namespace AniLens.Core.Interfaces;

public interface IHashService
{
    public Result<string> HashPassword(string password);

    public bool CheckPassword(string password, string hashedPassword);
}