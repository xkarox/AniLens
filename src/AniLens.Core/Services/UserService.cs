using System.Diagnostics;
using AniLens.Core.Extensions;
using AniLens.Core.Interfaces;
using AniLens.Core.Models;
using AniLens.Server.Settings;
using AniLens.Shared;
using AniLens.Shared.DTO;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AniLens.Core.Services;

public class UserService : IUserService
{
    private readonly IMongoCollection<User> _userCollection;
    private readonly IHashService _hashService;
    
    public UserService(IOptions<UserDbSettings> userDbSettings, IHashService hashService)
    {
        Debug.Assert(userDbSettings != null, nameof(userDbSettings) + " != null");
        var mongoSettings = userDbSettings.Value;
        var client = new MongoClient(mongoSettings.ConnectionUri);
        var database = client.GetDatabase(mongoSettings.DatabaseName);
        _userCollection = database.GetCollection<User>(mongoSettings
            .CollectionName);

        _hashService = hashService;
    }

    public async Task<Result<IEnumerable<UserDto>>> GetAll(int page = 1, int pageSize = 10)
    {
        try
        {
            var users = await _userCollection.Find(_ => true)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
            return Result<IEnumerable<UserDto>>.Success(users.ToDto());
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<UserDto>>.Failure(
                $"Failed to retrieve users: {ex.Message}", Error.Internal);
        }
    }

    public async Task<Result<UserDto>> Get(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                return Result<UserDto>.Failure("Invalid user ID", 
                    Error.Parameter);

            var filter = Builders<User>.Filter.Eq(user => user.Id, id);
            var user = await _userCollection.Find(filter).FirstOrDefaultAsync();
            
            return user != null
                ? Result<UserDto>.Success(user.ToDto())
                : Result<UserDto>.Failure($"User with ID {id} not found", 
                    Error.NotFound);
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure($"Failed to retrieve user: {ex.Message}", 
                Error.Internal);
        }
    }

    public async Task<Result<User>> GetByName(string username)
    {
        try
        {
            if (string.IsNullOrEmpty(username))
            {
                return Result<User>.Failure("No username provided",
                    Error.Parameter);
            }

            var filter =
                Builders<User>.Filter.Eq(user => user.Username, 
                    username);
            var user = await _userCollection.Find(filter).FirstOrDefaultAsync();

            return user != null
                ? Result<User>.Success(user)
                : Result<User>.Failure(
                    $"User with Username {username} not found",
                    Error.NotFound);
        }
        catch (MongoException ex)
        {
            return Result<User>.Failure($"Database error: {ex.Message}",
                Error.Internal);
        }
        catch (Exception ex)
        {
            return Result<User>.Failure($"Unexpected error: {ex.Message}",
                Error.Internal);
        }
    }
    public async Task<Result<UserDto>> AddUser(UserDto user)
    {
        try
        {
            if (await UsernameTaken(user.Username))
                return Result<UserDto>.Failure("Username already taken",
                    Error.UserTaken);
            var userModel = user.ToUser();
            userModel.Id = null;
            userModel.CreatedAt = DateTime.UtcNow;
            userModel.UpdatedAt = DateTime.UtcNow;
            await _userCollection.InsertOneAsync(userModel);
            return Result<UserDto>.Success(userModel.ToDto());
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure($"Failed to add user: {ex.Message}", 
                Error.Internal);
        }
    }
    
    public async Task<Result<UserDto>> AddUser(RegisterDto user)
    {
        try
        {
            if (await UsernameTaken(user.Username))
                return Result<UserDto>.Failure("Username already taken",
                    Error.UserTaken);
            var hashResult = _hashService.HashPassword(user.Password);
            if (hashResult.IsFailure)
                return Result<UserDto>.Failure("Failed to hash password",
                    Error.Internal);
            user.Password = hashResult.Data!;
            
            var userModel = user.ToUser();
            userModel.CreatedAt = DateTime.UtcNow;
            userModel.UpdatedAt = DateTime.UtcNow;
            await _userCollection.InsertOneAsync(userModel);
            return Result<UserDto>.Success(userModel.ToDto());
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure($"Failed to add user: {ex.Message}", 
                Error.Internal);
        }
    }

    public async Task<Result<NoData>> DeleteUser(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                return Result<NoData>.Failure("Invalid user ID", 
                    Error.Parameter);

            var filter = Builders<User>.Filter.Eq(user => user.Id, id);
            var result = await _userCollection.DeleteOneAsync(filter);
            
            return result.DeletedCount == 1
                ? Result<NoData>.Success()
                : Result<NoData>.Failure($"User with ID {id} not found", 
                    Error.NotFound);
        }
        catch (Exception ex)
        {
            return Result<NoData>.Failure($"Failed to delete user: {ex.Message}", 
                Error.Internal);
        }
    }

    public async Task<Result<UserDto>> UpdateUser(string id, UpdateUserDto user)
    {
        try
        {
            var foundUser = await _userCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
            if (foundUser == null)
                return Result<UserDto>.Failure($"User with ID {id} not found", Error.NotFound);
            
            if (!string.IsNullOrEmpty(user.Username))
                foundUser.Username = user.Username;
            if (!string.IsNullOrEmpty(user.Password))
            {
                var hashResult = _hashService.HashPassword(user.Password);
                foundUser.PasswordHash = hashResult.IsSuccess
                    ? hashResult.Data
                    : foundUser.PasswordHash;
            }
            if (!string.IsNullOrEmpty(user.Email))
                foundUser.Email = user.Email;
        
            foundUser.UpdatedAt = DateTime.UtcNow;
            
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            var result = await _userCollection.ReplaceOneAsync(filter, foundUser);
            
            if (!result.IsAcknowledged)
                return Result<UserDto>.Failure("Database operation not acknowledged", 
                    Error.Internal);
            
            return result.ModifiedCount > 0
                ? Result<UserDto>.Success(foundUser.ToDto())
                : Result<UserDto>.Failure($"User with ID {foundUser.Id} not found", 
                    Error.NotFound);
        }
        catch (Exception ex)
        {
            return Result<UserDto>.Failure($"Failed to update user: {ex.Message}",
                Error.Internal);
        }
    }

    public async Task<bool> UsernameTaken(string username)
    {
        var foundUser = await _userCollection.Find(u => u.Username == username).FirstOrDefaultAsync();
        return foundUser != null;
    }
}