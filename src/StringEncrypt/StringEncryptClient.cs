using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PELock.StringEncrypt;

/// <summary>
/// Stateful HTTP client: pass the activation key to the constructor, configure options with fluent
/// setters, then <c>SendAsync&lt;T&gt;</c> or the convenience methods.
/// </summary>
/// <remarks>
/// POST field <c>code</c> (activation key) is the value given at construction (empty string = demo).
/// When <see cref="SetCompression"/> is <c>true</c>, the API may return <c>source</c> as base64-encoded gzip;
/// the client decompresses automatically unless <see cref="SetDecompressEncryptSource"/> is <c>false</c>.
/// </remarks>
public sealed class StringEncrypt : IDisposable
{
    public const string DefaultApiUrl = "https://www.stringencrypt.com/api.php";

    private readonly HttpClient _http;
    private readonly bool _disposeClient;
    private readonly Encoding _latin1;

    private bool _preferCurl;
    private bool _decompressEncryptSource = true;
    private string? _command;

    private string _apiKey = "";

    private string _label = "Label";
    private string? _inputString;
    private byte[]? _inputBytes;

    private bool _compression;
    private string _language = Language.Php;
    private object _highlight = false;
    private int _cmdMin = 1;
    private int _cmdMax = 3;
    private bool _local;
    private bool _unicode = true;
    private string _langLocale = "en_US.utf8";
    private string _ansiEncoding = "WINDOWS-1250";
    private string _newLines = NewLine.Lf;
    private string? _template;
    private bool _returnTemplate;
    private bool _includeTags;
    private bool _includeExample;
    private bool _includeDebugComments;

    /// <param name="apiKey">Activation code for field <c>code</c> (empty = demo mode).</param>
    /// <param name="httpClient">Optional shared <see cref="HttpClient"/>; if null, an instance is created and disposed with this object.</param>
    /// <param name="preferCurl">Ignored; kept for parity with other SDKs.</param>
    public StringEncrypt(string apiKey = "", HttpClient? httpClient = null, bool preferCurl = false)
    {
        _apiKey = apiKey ?? "";
        _preferCurl = preferCurl;
        if (httpClient is null)
        {
            _http = new HttpClient();
            _disposeClient = true;
        }
        else
            _http = httpClient;

        _latin1 = Encoding.GetEncoding("ISO-8859-1");
    }

    /// <inheritdoc cref="StringEncrypt(string, HttpClient?, bool)"/>
    public StringEncrypt(HttpClient httpClient)
        : this("", httpClient, false)
    {
    }

    public bool GetPreferCurl() => _preferCurl;

    public StringEncrypt SetPreferCurl(bool preferCurl)
    {
        _preferCurl = preferCurl;
        return this;
    }

    public bool GetDecompressEncryptSource() => _decompressEncryptSource;

    public StringEncrypt SetDecompressEncryptSource(bool decompressEncryptSource)
    {
        _decompressEncryptSource = decompressEncryptSource;
        return this;
    }

    public string? GetCommand() => _command;

    public StringEncrypt SetCommand(string command)
    {
        _command = command;
        return this;
    }

    public string GetLabel() => _label;

    public StringEncrypt SetLabel(string label)
    {
        _label = label;
        return this;
    }

    /// <summary>UTF-8 text input; clears raw bytes input.</summary>
    public StringEncrypt SetString(string? str)
    {
        _inputString = str;
        _inputBytes = null;
        return this;
    }

    /// <summary>Raw binary input; clears string input.</summary>
    public StringEncrypt SetBytes(byte[]? bytes)
    {
        _inputBytes = bytes;
        _inputString = null;
        return this;
    }

    public bool GetCompression() => _compression;

    public StringEncrypt SetCompression(bool compression)
    {
        _compression = compression;
        return this;
    }

    public string GetLanguage() => _language;

    public StringEncrypt SetLanguage(string language)
    {
        _language = language;
        return this;
    }

    public object GetHighlight() => _highlight;

    /// <param name="highlight"><c>false</c>, or highlight mode string such as <c>geshi</c> / <c>js</c> when supported server-side.</param>
    public StringEncrypt SetHighlight(object highlight)
    {
        _highlight = highlight;
        return this;
    }

    public int GetCmdMin() => _cmdMin;

    public StringEncrypt SetCmdMin(int cmdMin)
    {
        _cmdMin = cmdMin;
        return this;
    }

    public int GetCmdMax() => _cmdMax;

    public StringEncrypt SetCmdMax(int cmdMax)
    {
        _cmdMax = cmdMax;
        return this;
    }

    public bool GetLocal() => _local;

    public StringEncrypt SetLocal(bool local)
    {
        _local = local;
        return this;
    }

    public bool GetUnicode() => _unicode;

    public StringEncrypt SetUnicode(bool unicode)
    {
        _unicode = unicode;
        return this;
    }

    public string GetLangLocale() => _langLocale;

    public StringEncrypt SetLangLocale(string langLocale)
    {
        _langLocale = langLocale;
        return this;
    }

    public string GetAnsiEncoding() => _ansiEncoding;

    public StringEncrypt SetAnsiEncoding(string ansiEncoding)
    {
        _ansiEncoding = ansiEncoding;
        return this;
    }

    public string GetNewLines() => _newLines;

    public StringEncrypt SetNewLines(string newLines)
    {
        _newLines = newLines;
        return this;
    }

    public string? GetTemplate() => _template;

    public StringEncrypt SetTemplate(string? template)
    {
        _template = template;
        return this;
    }

    public bool GetReturnTemplate() => _returnTemplate;

    public StringEncrypt SetReturnTemplate(bool returnTemplate)
    {
        _returnTemplate = returnTemplate;
        return this;
    }

    public bool GetIncludeTags() => _includeTags;

    public StringEncrypt SetIncludeTags(bool includeTags)
    {
        _includeTags = includeTags;
        return this;
    }

    public bool GetIncludeExample() => _includeExample;

    public StringEncrypt SetIncludeExample(bool includeExample)
    {
        _includeExample = includeExample;
        return this;
    }

    public bool GetIncludeDebugComments() => _includeDebugComments;

    public StringEncrypt SetIncludeDebugComments(bool includeDebugComments)
    {
        _includeDebugComments = includeDebugComments;
        return this;
    }

    /// <summary>Reset request fields to defaults (reuse the same client for another call).</summary>
    public StringEncrypt Reset()
    {
        _command = null;
        _label = "$label";
        _inputString = null;
        _inputBytes = null;
        _compression = false;
        _language = Language.Php;
        _highlight = false;
        _cmdMin = 1;
        _cmdMax = 3;
        _local = false;
        _unicode = true;
        _langLocale = "en_US.utf8";
        _ansiEncoding = "WINDOWS-1250";
        _newLines = NewLine.Lf;
        _template = null;
        _returnTemplate = false;
        _includeTags = false;
        _includeExample = false;
        _includeDebugComments = false;
        return this;
    }

    /// <summary>Activation status and limits for the key passed to the constructor.</summary>
    public async Task<DemoStatusResponse?> IsDemoAsync(CancellationToken cancellationToken = default)
    {
        var previous = _command;
        SetCommand(Command.IsDemo);
        var result = await SendAsync<DemoStatusResponse>(cancellationToken).ConfigureAwait(false);
        _command = previous;
        return result;
    }

    /// <summary>Encrypt raw file contents (binary).</summary>
    public async Task<EncryptResponse?> EncryptFileContentsAsync(string filePath, string label, CancellationToken cancellationToken = default)
    {
        byte[] raw;
        try
        {
            raw = await File.ReadAllBytesAsync(filePath, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            return null;
        }

        if (raw.Length == 0)
            return null;

        var savedCommand = _command;
        var savedString = _inputString;
        var savedBytes = _inputBytes;
        var savedLabel = _label;

        SetCommand(Command.Encrypt).SetBytes(raw).SetLabel(label);
        var result = await SendAsync<EncryptResponse>(cancellationToken).ConfigureAwait(false);

        _command = savedCommand;
        _inputString = savedString;
        _inputBytes = savedBytes;
        _label = savedLabel;

        return result;
    }

    /// <summary>Encrypt a string (UTF-8).</summary>
    public async Task<EncryptResponse?> EncryptStringAsync(string str, string label, CancellationToken cancellationToken = default)
    {
        var savedCommand = _command;
        var savedString = _inputString;
        var savedBytes = _inputBytes;
        var savedLabel = _label;

        SetCommand(Command.Encrypt).SetString(str).SetLabel(label);
        var result = await SendAsync<EncryptResponse>(cancellationToken).ConfigureAwait(false);

        _command = savedCommand;
        _inputString = savedString;
        _inputBytes = savedBytes;
        _label = savedLabel;

        return result;
    }

    /// <summary>Build the POST body (for debugging).</summary>
    public IReadOnlyDictionary<string, object?> ToRequestDictionary()
    {
        if (_command is null)
            throw new InvalidOperationException("Command must be set (use SetCommand()).");

        return _command switch
        {
            Command.Info => BuildInfoParams(),
            Command.IsDemo => BuildIsDemoParams(),
            Command.Encrypt => BuildEncryptParams(),
            _ => throw new InvalidOperationException("Unknown command."),
        };
    }

    /// <summary>Send the request and return parsed JSON, or null on transport / JSON failure.</summary>
    public async Task<T?> SendAsync<T>(CancellationToken cancellationToken = default)
    {
        var raw = await PostFormAsync(FormEncodeParams(ToRequestDictionary()), cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrEmpty(raw))
            return default;

        try
        {
            if (typeof(T) == typeof(EncryptResponse))
            {
                var enc = JsonSerializer.Deserialize<EncryptResponse>(raw, JsonOptions);
                if (enc is null)
                    return default;
                return (T)(object)ApplyDecryptorSourceDecompression(enc);
            }

            return JsonSerializer.Deserialize<T>(raw, JsonOptions);
        }
        catch
        {
            return default;
        }
    }

    /// <summary>Send without a typed wrapper (e.g. for <c>info</c>).</summary>
    public async Task<JsonDocument?> SendJsonAsync(CancellationToken cancellationToken = default)
    {
        var raw = await PostFormAsync(FormEncodeParams(ToRequestDictionary()), cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrEmpty(raw))
            return null;
        try
        {
            return JsonDocument.Parse(raw);
        }
        catch
        {
            return null;
        }
    }

    private EncryptResponse ApplyDecryptorSourceDecompression(EncryptResponse response)
    {
        if (_command != Command.Encrypt || !_compression || !_decompressEncryptSource)
            return response;
        if (response.Error != ErrorCode.Success)
            return response;
        var src = response.Source;
        if (string.IsNullOrEmpty(src))
            return response;

        byte[] binary;
        try
        {
            binary = Convert.FromBase64String(src);
        }
        catch
        {
            return response;
        }

        try
        {
            using var ms = new MemoryStream(binary);
            using var gz = new GZipStream(ms, CompressionMode.Decompress);
            using var outMs = new MemoryStream();
            gz.CopyTo(outMs);
            var plain = Encoding.UTF8.GetString(outMs.ToArray());
            return response with { Source = plain };
        }
        catch
        {
            return response;
        }
    }

    private Dictionary<string, object?> BuildInfoParams() => new()
    {
        ["command"] = Command.Info,
        ["code"] = _apiKey,
    };

    private Dictionary<string, object?> BuildIsDemoParams() => new()
    {
        ["command"] = Command.IsDemo,
        ["code"] = _apiKey,
    };

    private Dictionary<string, object?> BuildEncryptParams()
    {
        var p = new Dictionary<string, object?>
        {
            ["command"] = Command.Encrypt,
            ["code"] = _apiKey,
            ["label"] = _label,
            ["compression"] = _compression,
            ["lang"] = _language,
            ["cmd_min"] = _cmdMin,
            ["cmd_max"] = _cmdMax,
            ["local"] = _local,
            ["unicode"] = _unicode,
            ["lang_locale"] = _langLocale,
            ["ansi_encoding"] = _ansiEncoding,
            ["new_lines"] = _newLines,
            ["return_template"] = _returnTemplate,
            ["include_tags"] = _includeTags,
            ["include_example"] = _includeExample,
            ["include_debug_comments"] = _includeDebugComments,
        };

        if (_inputString is not null)
            p["string"] = _inputString;
        else if (_inputBytes is not null)
            p["bytes"] = _inputBytes;

        if (!Equals(_highlight, false))
            p["highlight"] = _highlight;

        if (_template is not null)
            p["template"] = _template;

        return p;
    }

    private string FormEncodeParams(IReadOnlyDictionary<string, object?> data)
    {
        var sb = new StringBuilder();
        var first = true;
        foreach (var kv in data)
        {
            if (kv.Value is null)
                continue;
            if (!first)
                sb.Append('&');
            first = false;
            sb.Append(Uri.EscapeDataString(kv.Key));
            sb.Append('=');
            var valueStr = kv.Value switch
            {
                bool bl => bl ? "1" : "0",
                byte[] buf => _latin1.GetString(buf),
                int i => i.ToString(System.Globalization.CultureInfo.InvariantCulture),
                long l => l.ToString(System.Globalization.CultureInfo.InvariantCulture),
                string s => s,
                _ => kv.Value.ToString() ?? "",
            };
            sb.Append(Uri.EscapeDataString(valueStr));
        }
        return sb.ToString();
    }

    private async Task<string?> PostFormAsync(string body, CancellationToken cancellationToken)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Post, DefaultApiUrl)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"),
            };
            req.Headers.TryAddWithoutValidation("User-Agent", "pelock/stringencrypt (+https://www.stringencrypt.com)");
            var res = await _http.SendAsync(req, cancellationToken).ConfigureAwait(false);
            return await res.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            return null;
        }
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
    };

    public void Dispose()
    {
        if (_disposeClient)
            _http.Dispose();
    }
}

/// <summary>Response for encrypt command.</summary>
public sealed record EncryptResponse
{
    [JsonPropertyName("error")]
    public int Error { get; init; }

    [JsonPropertyName("source")]
    public string? Source { get; init; }

    [JsonPropertyName("expired")]
    public bool? Expired { get; init; }

    [JsonPropertyName("credits_left")]
    public int? CreditsLeft { get; init; }

    [JsonPropertyName("credits_total")]
    public int? CreditsTotal { get; init; }
}

/// <summary>Response for is_demo command.</summary>
public sealed record DemoStatusResponse
{
    [JsonPropertyName("demo")]
    public bool? Demo { get; init; }

    [JsonPropertyName("label_limit")]
    public int? LabelLimit { get; init; }

    [JsonPropertyName("string_limit")]
    public int? StringLimit { get; init; }

    [JsonPropertyName("bytes_limit")]
    public int? BytesLimit { get; init; }

    [JsonPropertyName("credits_left")]
    public int? CreditsLeft { get; init; }

    [JsonPropertyName("credits_total")]
    public int? CreditsTotal { get; init; }

    [JsonPropertyName("cmd_min")]
    public int? CmdMin { get; init; }

    [JsonPropertyName("cmd_max")]
    public int? CmdMax { get; init; }
}
