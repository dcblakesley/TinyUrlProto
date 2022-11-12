using TinyUrlProto;
using TinyUrlProto.Models;
using TinyUrlProto.Repositories;
using TinyUrlProto.Services;

namespace Tests;

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