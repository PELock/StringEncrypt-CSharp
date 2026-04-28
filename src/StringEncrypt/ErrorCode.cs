namespace PELock.StringEncrypt;

/// <summary>API error codes returned in encrypt responses (<c>error</c> field).</summary>
/// <seealso href="https://www.stringencrypt.com/api/" />
public static class ErrorCode
{
    public const int Success = 0;
    public const int EmptyLabel = 1;
    public const int LengthLabel = 2;
    public const int EmptyString = 3;
    public const int EmptyBytes = 4;
    public const int EmptyInput = 5;
    public const int LengthString = 6;
    public const int InvalidLang = 7;
    public const int InvalidLocale = 8;
    public const int CmdMin = 9;
    public const int CmdMax = 10;
    public const int LengthBytes = 11;
    public const int Demo = 100;
}
