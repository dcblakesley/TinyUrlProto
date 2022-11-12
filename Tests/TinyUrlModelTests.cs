using TinyUrlProto.Models;

namespace Tests;

[TestClass]
public class TinyUrlModelTests
{
    [TestMethod]
    [DataRow("abacabb8", "AAA", "BBB", true, "")]
    [DataRow("abacabb", "AAA", "BBB", false, "Id has the wrong number of characters")]
    [DataRow("abacabb8", "", "BBB", false, "No longUrl")]
    [DataRow("abacabb8", "AAA", "", false, "No domain")]
    [DataRow("abacabb8", "A AA", "BBB", false, "Invalid longUrl")]
    [DataRow("abacabb8", "AAA", "BB B", false, "Invalid domain")]
    public void Validation_Success(string id, string longUrl, string domain, bool expectPass, string reasonItShouldNotPass)
    {
        // Arrange
        var tinyUrl = new TinyUrl(id, longUrl, domain);

        // Act
        var result = tinyUrl.Validate();

        // Assert
        if(expectPass)
            Assert.AreEqual(null, result);
        else
            Assert.AreNotEqual(null, result);
    }

}