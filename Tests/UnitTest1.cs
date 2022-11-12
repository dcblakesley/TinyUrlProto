using TinyUrlProto;

namespace Tests;


[TestClass]
public class DemoTest
{
    readonly Guid _userId = Guid.Parse("9B978B0D-902E-47A5-B863-B00E0AB0D7E2");

    public async Task Integration()
    {
        // Typically these parts would be setup through the IOC framework in Asp.net or some other framework
        var repo = new TinyUrlRepository(new Database());
        var service = new TinyUrlService(repo);
        
        await Seed(service);

        // 

        // Act
        // Assert
    }

    /// <summary> Seed through the service, adds TinyUrls for anonymous users and customers </summary>
    async Task Seed(TinyUrlService tinyUrlService)
    {
        // Only seeding valid TinyUrls, testing invalid ones is handled by the Unit Tests
        List<TinyUrl> tinyUrlsToAdd = new()
        {
            // Anonymous Users
            new("www.yahoo.com", "TinyUrl", null),
            new("yahoo.com", "TinyUrl", null),
            new("a.com", "TinyUrl", null),
            new("google.com", "TinyUrl", null),
            new("tomshardware.com", "CustomDomain", null),

            // Customer
            new("www.yahoo.com", "TinyUrl", _userId),
            new("yahoo.com", "TinyUrl", _userId),
            new("a.com", "TinyUrl", _userId),
            new("google.com", "TinyUrl", _userId),
            new("tomshardware.com", "CustomDomain", _userId)
        };

        foreach (var tinyUrl in tinyUrlsToAdd)
        {
            _ = await tinyUrlService.CreateTinyUrl(tinyUrl.UserId, tinyUrl);
        }
    }

    async Task 
}


[TestClass]
public class TinyUrlServiceTests
{
    readonly Guid _userId = Guid.Parse("9B978B0D-902E-47A5-B863-B00E0AB0D7E2");

    [TestMethod]

    // Anonymous Users
    [DataRow("www.yahoo.com", "TinyUrl", false, true, "")]
    [DataRow("yahoo.com", "TinyUrl", false, true, "")]
    [DataRow("a.com", "TinyUrl", false, true, "")]
    [DataRow("yahoo.l", "TinyUrl", false, true, "")]
    [DataRow("yahoo.l", "Tiny Url", false, false, "Invalid character 'space' in domain name")]
    [DataRow("yah oo.l", "TinyUrl", false, false, "Invalid character 'space' in url")]

    // Account Holders
    [DataRow("www.yahoo.com", "TinyUrl", true, true, "")]
    [DataRow("yahoo.com", "TinyUrl", true, true, "")]
    [DataRow("a.com", "TinyUrl", true,true, "")]
    [DataRow("yahoo.l", "TinyUrl", true, true, "")]
    [DataRow("yahoo.l", "Tiny Url", true, false, "Invalid character 'space' in domain name")]
    [DataRow("yah oo.l", "TinyUrl", true, false, "Invalid character 'space' in url")]
    public async Task Create_TinyUrl(string longUrl, string domain, bool useUser, bool expectToPass, string explanationIfNotExpectedToPass)
    {
        // Arrange
        var repo = new TinyUrlRepository(new Database());
        var service = new TinyUrlService(repo);
        var tinyUrl = new TinyUrl(longUrl, domain);

        TinyUrl? result = null;

        // Act
        result = useUser 
            ? await service.CreateTinyUrl(_userId, tinyUrl) 
            : await service.CreateTinyUrl(null, tinyUrl);

        // Assert
        if (expectToPass)
            Assert.IsNotNull(result);
        else
            Assert.IsNull(result);
    }
}