namespace TinyUrlProto;

public class User
{
    public User(Guid id, string name)
    {
        Id=id;
        Name=name;
    }

    public Guid Id { get; set; }
    public string Name { get; set; }

    /// <summary>
    /// Not stored in the Table
    /// </summary>
    public List<TinyUrl> TinyUrls { get; set; } = new();
}