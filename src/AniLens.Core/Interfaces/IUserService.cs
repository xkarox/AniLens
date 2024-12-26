using AniLens.Core.Models;
using AniLens.Shared;

namespace AniLens.Core.Interfaces;

public interface IUserService
{
    public Task<Result<IEnumerable<User>>> GetAll();
    public Task<Result<User>> Get(string id);
    public Task<Result<NoData>> AddUser(User user);
    public Task<Result<NoData>> DeleteUser(string id);
    public Task<Result<User>> UpdateUser(User user);
}