﻿namespace TinyUrlProto;

public class TinyUrl
{
    public TinyUrl(string id, Guid? userId, string longUrl, string domain)
    {
        Id=id;
        UserId=userId;
        LongUrl=longUrl;
        Domain=domain;
    }
    
    /// <summary> TinyUrl / PrimaryKey - Unique Id Generated by TinyUrl and shown to users </summary>
    public string Id { get; set; }

    /// <summary> Input URL input from the user, can be changed later </summary>
    public string LongUrl { get; set; }

    /// <summary>
    /// always 'TinyUrl' for non-paying customers <br/>
    /// Paying customers can have custom Domains
    /// </summary>
    public string Domain { get; set; }

    /// <summary> Foreign Key to Users.Id </summary>
    public Guid? UserId { get; set; }

    public int UseCount { get; set; }

    // Methods
    
    /// <returns>
    /// Valid = null<br/>
    /// Invalid = List<string> with an explanation of why validation failed
    /// </returns>
    public List<string>? Validate()
    {
        //  Rules: LongUrl and Domain must be valid
        if (Uri.IsWellFormedUriString(LongUrl, UriKind.RelativeOrAbsolute) 
            && Uri.IsWellFormedUriString($"{Domain}.com", UriKind.RelativeOrAbsolute)) {
            return null;
        }
        
        // Explain why validation failed
        var results = new List<string>();
        if (!Uri.IsWellFormedUriString(LongUrl, UriKind.RelativeOrAbsolute))
            results.Add($"Invalid LongUrl: {LongUrl}");
        if (!Uri.IsWellFormedUriString($"{Domain}.com", UriKind.RelativeOrAbsolute))
            results.Add($"Invalid Domain: {Domain}");
        
        return results;
    }
    public override string ToString() => $"{Id} {(UserId == null ? "" : $" - {UserId}")} - {LongUrl}";
}