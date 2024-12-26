using AniLens.Core.Interfaces;
using AniLens.Core.Models;
using AniLens.Server.Settings;
using AniLens.Shared;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AniLens.Server.Services;

public class UserService : IUserService
{
    private readonly IMongoCollection<User> _userCollection;
    
    public UserService(IOptions<UserDbSettings> userDbSettings)
    {
        ArgumentNullException.ThrowIfNull(userDbSettings);
        var mongoSettings = userDbSettings.Value;
        var client = new MongoClient(mongoSettings.ConnectionUri);
        var database = client.GetDatabase(mongoSettings.DatabaseName);
        _userCollection = database.GetCollection<User>(mongoSettings
            .CollectionName);
    }

    public async Task<Result<IEnumerable<User>>> GetAll()
    {
        try
        {
            var users = await _userCollection.Find(_ => true)
                .ToListAsync();
            return Result<IEnumerable<User>>.Success(users);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<User>>.Failure(
                $"Failed to retrieve users: {ex.Message}", Error.Internal);
        }
    }

    public async Task<Result<User>> Get(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                return Result<User>.Failure("Invalid user ID", 
                    Error.Parameter);

            var filter = Builders<User>.Filter.Eq(user => user.Id, id);
            var user = await _userCollection.Find(filter).FirstOrDefaultAsync();
            
            return user != null
                ? Result<User>.Success(user)
                : Result<User>.Failure($"User with ID {id} not found", 
                    Error.NotFound);
        }
        catch (Exception ex)
        {
            return Result<User>.Failure($"Failed to retrieve user: {ex.Message}", 
                Error.Internal);
        }
    }

    public async Task<Result<NoData>> AddUser(User user)
    {
        try
        {
            await _userCollection.InsertOneAsync(user);
            return Result<NoData>.Success();
        }
        catch (Exception ex)
        {
            return Result<NoData>.Failure($"Failed to add user: {ex.Message}", 
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

    public async Task<Result<User>> UpdateUser(User user)
    {
        try
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            var result = await _userCollection.ReplaceOneAsync(filter, user);
            
            if (!result.IsAcknowledged)
                return Result<User>.Failure("Database operation not acknowledged", 
                    Error.Internal);
            
            return result.ModifiedCount > 0
                ? Result<User>.Success(user)
                : Result<User>.Failure($"User with ID {user.Id} not found", 
                    Error.NotFound);
        }
        catch (Exception ex)
        {
            return Result<User>.Failure($"Failed to update user: {ex.Message}",
                Error.Internal);
        }
    }
}