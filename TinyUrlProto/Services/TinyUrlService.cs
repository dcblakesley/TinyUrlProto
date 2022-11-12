using TinyUrlProto.Models;
using TinyUrlProto.Repositories;

namespace TinyUrlProto.Services;

#pragma warning disable CS1998
/// <summary> Handles Validation and any other necessary work before sending the request to the Repository/Database </summary>
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
        return await _tinyUrlRepository.AddTinyUrl(tinyUrl);
    }

    public async Task<bool> DeleteTinyUrl(Guid? userId, string tinyUrlId) => await _tinyUrlRepository.DeleteTinyUrl(userId, tinyUrlId);

    public async Task<int?> GetTinyUrlUsageCount(string tinyUrlId) => await _tinyUrlRepository.GetUsageCount(tinyUrlId);

    /// <summary> Used for the typical redirect, increments the UsageCounter </summary>
    /// <returns>the full url</returns>
    public async Task<string?> GetLongUrl(string tinyUrlId) => await _tinyUrlRepository.GetLongUrl(tinyUrlId);
}