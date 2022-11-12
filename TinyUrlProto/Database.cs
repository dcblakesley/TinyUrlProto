using TinyUrlProto.Models;

namespace TinyUrlProto;


/// <summary>
/// A poor simulation of Sql for EF usage. <br/>
/// Doesn't allow Indexes or .Select to only return desired columns 
/// </summary>
public class Database
{
    /// <remarks> Note: Should have an index for UserIds </remarks>
    public List<TinyUrl> TinyUrlsTable { get; set; } = new();

    /// <summary>
    /// This only tracks what Users are doing in our service.<br/>
    /// Users are created by the Identity provider, we just track how they use our service
    /// </summary>
    public List<User> UsersTable { get; set; } = new();
}