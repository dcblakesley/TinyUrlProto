namespace TinyUrlProto;

public class TinyUrlRepository
{
    readonly Database _database;
    public TinyUrlRepository(Database database) => _database = database;

    // Necessary 
    //  - Create short URLs with associated long URLs.
    //  - Delete short URLs with associated long URLs.
    //  - Getting statistics on the number of times a short URL has been "clicked" i.e. the number of times its long URL has been retrieved
    //  - 

    /// <returns>
    /// A TinyUrl object if successful. <br/>
    /// null if there was an error such as validation failed.
    /// </returns>
    public TinyUrl? CreateTinyUrl(Guid? userId, string newLongUrl, string desiredDomain)
    {
        User? user = null;
        if (userId != null)
            user = _database.UsersTable.FirstOrDefault(x => x.Id == userId);
        
        // If the user isn't null, check if the longUrl already exists for that user
        if (user != null)
        {
            var existingTinyUrl = _database.TinyUrlsTable.FirstOrDefault(x => x.UserId == userId && x.LongUrl == newLongUrl);
            if (existingTinyUrl != null)
                return existingTinyUrl;
        }

        // No existing user or entity, generate a new one
        var tinyUrl = new TinyUrl(GenerateId(), userId, newLongUrl, desiredDomain);

        // Check for problems before inserting
        if (tinyUrl.Validate() != null)
            return null;
        
        // Add to Db and send back to Customer
        _database.TinyUrlsTable.Add(tinyUrl);
        return tinyUrl;
    }

    public bool DeleteTinyUrl(Guid? userId, string tinyUrlId)
    {


        return true;
    }

    // Methods

    /// <remarks> Does not check for collisions which have a 1 in 2.82 trillion possibility </remarks>
    static string GenerateId() => Guid.NewGuid().ToString("N")[..8];
    public string? GetLongUrlFromTinyUrlString(string tinyUrl) => _database.TinyUrlsTable.FirstOrDefault(x => x.Id == tinyUrl)?.LongUrl;
}