using AniLens.Core.Models;
using AniLens.Shared;
using AniLens.Shared.DTO;

namespace AniLens.Core.Interfaces;

public interface IUserService
{
    public Task<Result<IEnumerable<UserDto>>> GetAll();
    public Task<Result<UserDto>> Get(string id);

    public Task<Result<User>> GetByName(string username);
    public Task<Result<UserDto>> AddUser(UserDto user);
    public Task<Result<NoData>> DeleteUser(string id);
    public Task<Result<UserDto>> UpdateUser(string id, UpdateUserDto user);
}