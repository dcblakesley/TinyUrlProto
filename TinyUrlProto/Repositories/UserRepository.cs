using TinyUrlProto.Models;

namespace TinyUrlProto.Repositories;
#pragma warning disable CS1998

public class UserRepository
{
    readonly Database _database;
    public UserRepository(Database database) => _database = database;

    /// <summary> Gets the User, if the user isn't in the DB yet it creates a new one.  </summary>
    /// <param name="userId">The userId from the Identify Provider</param>
    /// <returns>The user object </returns>
    public async Task<User> GetUser(Guid userId)
    {
        var user = _database.UsersTable.FirstOrDefault(x => x.Id == userId);

        if (user == null)
        {
            // The user hasn't been added to the DB yet, add a User with no name.
            user = new User(userId);
            _database.UsersTable.Add(user);
        }

        var dto = new User(userId, user.Name)
        {
            TinyUrls = _database.TinyUrlsTable.Where(x => x.UserId == userId).ToList()
        };

        return dto;
    }
    public async Task<User> UpdateUser(Guid userId, User user)
    {
        var existingUser = _database.UsersTable.FirstOrDefault(x => x.Id == userId);
        if (existingUser == null)
        {
            existingUser = new User(userId);
            _database.UsersTable.Add(existingUser);
        }
        existingUser.Name = user.Name;
        // SaveChanges
        return existingUser;
    }
}