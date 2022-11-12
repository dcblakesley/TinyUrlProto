using TinyUrlProto;

namespace Tests;

[TestClass]
public class Tests
{
    readonly TinyUrlRepository _tinyUrlRepository;
    readonly Guid _customerId;
    public Tests()
    {
        _tinyUrlRepository = new TinyUrlRepository(new Database());
        _customerId = Guid.Parse("9B978B0D-902E-47A5-B863-B00E0AB0D7E2");
    }


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
    public void Create_TinyUrl(string longUrl, string domain, bool useCustomer, bool expectToPass, string explanationIfNotExpectedToPass)
    {
        // Arrange
        Guid? userId = null;
        if (useCustomer)
            userId = _customerId;

        // Act
        var tinyUrl = _tinyUrlRepository.CreateTinyUrl(userId, longUrl, domain);

        // Assert
        if (expectToPass)
            Assert.IsNotNull(tinyUrl);
        else
            Assert.IsNull(tinyUrl);
    }
    
    public void Delete_TinyUrl()
    {
        // Add some
    }






}

