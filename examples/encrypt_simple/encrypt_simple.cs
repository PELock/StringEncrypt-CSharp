///////////////////////////////////////////////////////////////////////////////
//
// StringEncrypt WebApi interface usage example.
//
// In this example we will encrypt sample string with default options.
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

var result = await stringEncrypt.EncryptStringAsync("Hello!", "$label");

if (result is null)
{
    Console.WriteLine("Cannot connect to the API.");
    Environment.Exit(1);
}

if (result.Error != ErrorCode.Success)
{
    Console.WriteLine($"API error: {result.Error}");
    Environment.Exit(1);
}

Console.WriteLine((result.Source ?? "") + Environment.NewLine);
