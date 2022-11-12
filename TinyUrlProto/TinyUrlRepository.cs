namespace TinyUrlProto;
#pragma warning disable CS1998 // using async to mimic behavior of using real storage

/// <summary> Handles database operations, ensure TinyUrls are valid before trying to insert </summary>
public class TinyUrlRepository
{
    public readonly Database _database;
    public TinyUrlRepository(Database database) => _database = database; // Also inject a Logger


    // Necessary 
    //  - Create short URLs with associated long URLs.
    //  - Delete short URLs with associated long URLs.
    //  - Getting statistics on the number of times a short URL has been "clicked" i.e. the number of times its long URL has been retrieved
    //  - 

    /// <returns>
    /// A TinyUrl object if successful. <br/>
    /// null if there was an error such as validation failed.
    /// </returns>
    public async Task<TinyUrl?> Add(TinyUrl tinyUrl)
    {
        User? user = null;
        if (tinyUrl.UserId != null)
            user = _database.UsersTable.FirstOrDefault(x => x.Id == tinyUrl.UserId);
        
        // If the user isn't null, check if the longUrl already exists for that user
        if (user != null)
        {
            var existingTinyUrl = _database.TinyUrlsTable.FirstOrDefault(x => x.UserId == tinyUrl.UserId && x.LongUrl == tinyUrl.LongUrl);
            if (existingTinyUrl != null)
                return existingTinyUrl;
        }

        // No existing entity, generate a new Id
        tinyUrl.Id = GenerateId();

        // Add to Db and send back to Customer
        _database.TinyUrlsTable.Add(tinyUrl);
        return tinyUrl;
    }

    public async Task<bool> Delete(Guid? userId, string tinyUrlId)
    {
        var target = _database.TinyUrlsTable.FirstOrDefault(x => x.Id == tinyUrlId);
        
        if (target == null) // Log user trying to delete a tinyUrl that doesn't exist
            return false;

        if(target.UserId != userId) // Log user does not have permission
            return false;
        
        return _database.TinyUrlsTable.Remove(target);
    }

    /// <summary> Add Usages/Clicks to the TinyUrl </summary>
    /// <param name="tinyUrlId">The 8 digit TinyUrl.Id</param>
    /// <param name="count">The number of clicks to add to the Count, this can be more than 1 to allow batch adds</param>
    /// <returns></returns>
    public async Task AddClicks(string tinyUrlId, int count)
    {
        var tinyUrl = _database.TinyUrlsTable.FirstOrDefault(x => x.Id == tinyUrlId);
        
        if (tinyUrl == null) // Log user trying access tinyUrl that doesn't exist 
            return;

        tinyUrl.UseCount += count;
    }

    public async Task<string?> GetLongUrl(string tinyUrlId) => _database.TinyUrlsTable.FirstOrDefault(x => x.Id == tinyUrlId)?.LongUrl;
    

    /// <remarks> Does not check for collisions which have a 1 in 2.82 trillion possibility </remarks>
    static string GenerateId() => Guid.NewGuid().ToString("N")[..8];
    public string? GetLongUrlFromTinyUrlString(string tinyUrl) => _database.TinyUrlsTable.FirstOrDefault(x => x.Id == tinyUrl)?.LongUrl;
}

/// <summary> Handles Validation and any other necessary work before sending the request to the Repository or Database </summary>
public class TinyUrlService
{
    readonly TinyUrlRepository _tinyUrlRepository;
    public TinyUrlService(TinyUrlRepository tinyUrlRepository) => _tinyUrlRepository = tinyUrlRepository;

    /// <returns>
    /// A TinyUrl object if successful. <br/>
    /// null if there was an error such as validation failed.
    /// </returns>
    public async Task<TinyUrl?> CreateTinyUrl(Guid? userId, TinyUrl tinyUrl)
    {
        // Check for problems before inserting
        if (tinyUrl.Validate() != null)
            return null;

        // Always get the Users' Id from the Authentication Provider
        tinyUrl.UserId = userId;

        // Add to Db and send back to Customer
        return await _tinyUrlRepository.Add(tinyUrl);
    }

    public async Task<bool> DeleteTinyUrl(Guid? userId, string tinyUrlId) => await _tinyUrlRepository.Delete(userId, tinyUrlId);
    
    /// <remarks>Note: This is low priority, so if we have times where there are bursts too high for the hardware, add a caching mechanism to keep this from affecting adding links</remarks>>
    public async Task AddClick(string tinyUrlId) => await _tinyUrlRepository.AddClicks(tinyUrlId, 1);
}