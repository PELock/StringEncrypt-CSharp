///////////////////////////////////////////////////////////////////////////////
//
// StringEncrypt WebApi interface usage example.
//
// In this example we will verify our activation code status.
//
// Version        : v1.0.1
// Language       : C#
// Author         : Bartosz Wójcik
// Project page   : https://www.stringencrypt.com
// Web page       : https://www.pelock.com
//
///////////////////////////////////////////////////////////////////////////////

using PELock.StringEncrypt;

using var stringEncrypt = new StringEncrypt("");

var result = await stringEncrypt.IsDemoAsync();

if (result is null)
{
    Console.WriteLine("Cannot connect to the API.");
    Environment.Exit(1);
}

if (result.Demo == true)
    Console.WriteLine("DEMO mode");
else
{
    Console.WriteLine("FULL mode");
    Console.WriteLine($"Credits left: {result.CreditsLeft}");
}

Console.WriteLine($"Label max length: {result.LabelLimit}");
Console.WriteLine($"String max length: {result.StringLimit}");
Console.WriteLine($"Bytes max length: {result.BytesLimit}");
Console.WriteLine($"cmd_min / cmd_max: {result.CmdMin} / {result.CmdMax}");
