///////////////////////////////////////////////////////////////////////////////
//
// StringEncrypt WebApi interface usage example.
//
// In this example we will encrypt sample file with default options.
//
// Version        : v1.0.0
// Language       : C#
// Author         : Bartosz Wójcik
// Project page   : https://www.stringencrypt.com
// Web page       : https://www.pelock.com
//
///////////////////////////////////////////////////////////////////////////////

using PELock.StringEncrypt;

using var stringEncrypt = new StringEncrypt("YOUR-API-KEY-HERE"); // leave empty for demo mode
stringEncrypt
    .SetCompression(false)
    .SetUnicode(true)
    .SetLangLocale("en_US.utf8")
    .SetNewLines(NewLine.Lf)
    .SetAnsiEncoding("WINDOWS-1250")
    .SetLanguage(Language.Php)
    .SetCmdMin(1)
    .SetCmdMax(3)
    .SetLocal(false);

var baseDir = AppContext.BaseDirectory;
var samplePath = Path.Combine(baseDir, "sample.bin");

// Full license: raw bytes from file (demo may return ERROR_DEMO).
var result = await stringEncrypt.EncryptFileContentsAsync(samplePath, "$label");

if (result is null)
{
    Console.WriteLine("Cannot connect to the API or file is missing/empty.");
    Environment.Exit(1);
}

if (result.Error != ErrorCode.Success)
{
    Console.WriteLine($"API error: {result.Error}");
    Environment.Exit(1);
}

Console.WriteLine((result.Source ?? "") + Environment.NewLine);
