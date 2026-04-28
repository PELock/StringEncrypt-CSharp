namespace PELock.StringEncrypt;

/// <summary>Web API <c>command</c> parameter.</summary>
public static class Command
{
    /// <summary>Encrypt a string or raw bytes (<c>encrypt</c>).</summary>
    public const string Encrypt = "encrypt";

    /// <summary>Activation code limits and demo vs full mode (<c>is_demo</c>).</summary>
    public const string IsDemo = "is_demo";

    /// <summary>Engine version and supported languages (<c>info</c>).</summary>
    public const string Info = "info";
}
