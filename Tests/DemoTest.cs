using TinyUrlProto;
using TinyUrlProto.Models;
using TinyUrlProto.Repositories;
using TinyUrlProto.Services;

namespace Tests;

[TestClass]
public class DemoTest
{
    readonly Guid _userId = Guid.Parse("9B978B0D-902E-47A5-B863-B00E0AB0D7E2");

    [TestMethod]
    public async Task Demo()
    {
        // Typically these parts would be setup through the IOC framework in Asp.net or some other framework
        var database = new Database();
        var tinyUrlRepository = new TinyUrlRepository(database);
        var tinyUrlService = new TinyUrlService(tinyUrlRepository);
        var userRepository = new UserRepository(database);
        var userService = new UserService(userRepository);

        var tinyUrlsList = await Seed(tinyUrlService);

        // Simulate users using the service by clicking links
        await SimulateUsersClickingLinks(tinyUrlService, tinyUrlsList);
        
        // Get the User and statistics for all their TinyUrl clicks
        var user = await userService.GetUser(_userId);
        Assert.AreEqual(user.TinyUrls.Count, 5);

        // Check the count for the Users' first TinyUrl clicks
        var firstTinyUrlForUser = user.TinyUrls.First();
        Assert.AreEqual(firstTinyUrlForUser.UseCount, 6);

        // Check the same thing from the TinyUrlService
        var countFromTinyUrlService = await tinyUrlService.GetTinyUrlUsageCount(firstTinyUrlForUser.Id);
        Assert.AreEqual(countFromTinyUrlService, 6);

        // Delete the Users' first link
        var deleteResponse = await tinyUrlService.DeleteTinyUrl(_userId, firstTinyUrlForUser.Id);
        Assert.IsTrue(deleteResponse);

        // Retrieve the User again to verify that their TinyUrl is gone
        user = await userService.GetUser(_userId);
        Assert.AreEqual(user.TinyUrls.Count, 4);

        // Add a TinyUrl with a custom Id
        var customTinyUrl = new TinyUrl("abacabb8", "www.bbbb.com", "NeonClear", _userId);
        var customTinyUrlResponse = await tinyUrlService.CreateTinyUrl(_userId, customTinyUrl);

        // Verify that it was added to the User
        user = await userService.GetUser(_userId);
        Assert.AreEqual(user.TinyUrls.Count, 5);

        // Fail adding a custom Id
        var customTinyUrlFail = new TinyUrl("abacabb8", "www.bbbb.com", "NeonClear", _userId);
        var failResponse = await tinyUrlService.CreateTinyUrl(_userId, customTinyUrlFail);
        Assert.AreEqual(failResponse, null);
        
        // Verify that it was added to the User
        user = await userService.GetUser(_userId);
        Assert.AreEqual(user.TinyUrls.Count, 5);
    }

    /// <summary> Seed through the service, adds TinyUrls for anonymous users and customers </summary>
    async Task<List<TinyUrl>> Seed(TinyUrlService tinyUrlService)
    {
        var list = new List<TinyUrl>();

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
            var result = await tinyUrlService.CreateTinyUrl(tinyUrl.UserId, tinyUrl);
            if (result != null) list.Add(result);
        }
        return list;
    }

    /// <summary> Add clicks to each TinyUrl </summary>
    async Task SimulateUsersClickingLinks(TinyUrlService tinyUrlService, List<TinyUrl> existingTinyUrls)
    {
        var counter = 0;
        foreach (var tinyUrl in existingTinyUrls)
        {
            counter++;

            // Add a different number of clicks for each TinyUrl
            for (var i = 0; i < counter; i++)
                _ = await tinyUrlService.GetLongUrl(tinyUrl.Id);
        }
    }
}