namespace TinyUrlProto;

public class UserRepository
{
    readonly Database _database;
    public UserRepository(Database database) => _database = database;

    /// <summary> Utilizes the userId from the Identity provider to track  </summary>
    /// <param name="userId">The userId from the Identify Provider</param>
    /// <returns></returns>
    public User? GetOrCreateUser(Guid userId)
    {
        var user = _database.UsersTable.FirstOrDefault(x => x.Id == userId);
        
        if (user == null)
        {
            // The user hasn't been added to the DB yet. 
            
        }

        return user;
    }
    public User UpdateUser(User user)
    {
        return null;
    }

}