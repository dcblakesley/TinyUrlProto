using TinyUrlProto.Models;

namespace TinyUrlProto.Repositories;
#pragma warning disable CS1998 // using async to mimic behavior of using a db server

/// <summary> Handles database operations, ensure TinyUrls are valid before trying to insert </summary>
public class TinyUrlRepository
{
    public readonly Database _database;
    public TinyUrlRepository(Database database) => _database = database; // Also inject a Logger

    /// <summary> Create short URLs with associated long URLs.</summary>
    /// <returns>
    /// A TinyUrl object if successful. <br/>
    /// null if the Id already exists or there was an error such as not having permission. 
    /// </returns>
    public async Task<TinyUrl?> AddTinyUrl(TinyUrl tinyUrl)
    {
        User? user = null;
        if (tinyUrl.UserId != null)
            user = _database.UsersTable.FirstOrDefault(x => x.Id == tinyUrl.UserId);

        // If the user isn't null, check if the longUrl already exists for that user
        if (user != null)
        {
            var existingTinyUrl = _database.TinyUrlsTable.FirstOrDefault(x => x.UserId == tinyUrl.UserId && x.LongUrl == tinyUrl.LongUrl);
            if (existingTinyUrl != null)
                return null;
        }

        // No existing entity, generate a new Id
        if(string.IsNullOrWhiteSpace(tinyUrl.Id))
            tinyUrl.Id = GenerateId();

        // Add to Db and send back to Customer
        _database.TinyUrlsTable.Add(tinyUrl);
        return tinyUrl;
    }

    /// <summary> Delete short URLs with associated long URLs. </summary>
    public async Task<bool> DeleteTinyUrl(Guid? userId, string tinyUrlId)
    {
        var target = _database.TinyUrlsTable.FirstOrDefault(x => x.Id == tinyUrlId);

        if (target == null) // Log user trying to delete a tinyUrl that doesn't exist
            return false;

        if (target.UserId != userId) // Log user does not have permission
            return false;

        return _database.TinyUrlsTable.Remove(target);
    }

    /// <summary> Gets the full url for redirect and increments the usage counter </summary>
    /// <returns>Full Url</returns>
    public async Task<string?> GetLongUrl(string tinyUrlId)
    {
        var tinyUrl = _database.TinyUrlsTable.FirstOrDefault(x => x.Id == tinyUrlId);
        if (tinyUrl == null)
            return null;

        tinyUrl.UseCount++;
        // context.SaveChanges
        return tinyUrl.LongUrl;
    }

    /// <summary> Get the number of times the LongUrl has been used </summary>
    public async Task<int?> GetUsageCount(string tinyUrlId) => _database.TinyUrlsTable.FirstOrDefault(x => x.Id == tinyUrlId)?.UseCount;

    // TODO: Implement UpdateTinyUrl
    public async Task<TinyUrl> UpdateTinyUrl(TinyUrl tinyUrl) => throw new NotImplementedException();

    public async Task<TinyUrl?> GetTinyUrl(string tinyUrlId) => _database.TinyUrlsTable.FirstOrDefault(x => x.Id == tinyUrlId);


    /// <remarks> Does not check for collisions which have a 1 in 2.82 trillion possibility of occurring </remarks>
    static string GenerateId() => Guid.NewGuid().ToString("N")[..8];
}