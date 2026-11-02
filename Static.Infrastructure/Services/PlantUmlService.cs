using System.IO.Compression;
using System.Text;
using Microsoft.Extensions.Logging;
using Static.Application.Common.Interfaces;
using Static.Helper.Constants;

namespace Static.Infrastructure.Services;

/// <summary>
/// Renders PlantUML source to SVG by encoding the text with the PlantUML compression
/// scheme (UTF-8 → Deflate → PlantUML base64 alphabet) and calling the PlantUML server.
/// </summary>
internal sealed class PlantUmlService(
    IHttpClientFactory httpClientFactory,
    ILogger<PlantUmlService> logger) : IPlantUmlService
{
    // PlantUML uses a custom 64-char alphabet for its base64 encoding
    private const string PlantUmlAlphabet =
        "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz-_";

    public async Task<string?> RenderToSvgAsync(string umlContent, CancellationToken cancellationToken = default)
    {
        try
        {
            var encoded  = Encode(umlContent);
            var client   = httpClientFactory.CreateClient(StaticConstants.HttpClients.PlantUml);
            var response = await client.GetAsync($"/svg/{encoded}", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogWarning(
                    "PlantUML server returned {StatusCode} for encoded content. Body: {Body}",
                    (int)response.StatusCode, body);
                return null;
            }

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex,
                "PlantUML render failed — is the server reachable? Check PlantUml:BaseUrl in appsettings.");
            return null;
        }
    }

    // ──────────────────────────────────────────────────────────────────────────
    // PlantUML encoding: UTF-8 bytes → Deflate (raw, no header) → custom base64
    // Reference: https://plantuml.com/text-encoding
    // ──────────────────────────────────────────────────────────────────────────

    private static string Encode(string text)
    {
        var utf8Bytes   = Encoding.UTF8.GetBytes(text);
        var compressed  = Deflate(utf8Bytes);
        return ToPlantUmlBase64(compressed);
    }

    private static byte[] Deflate(byte[] data)
    {
        using var output = new MemoryStream();
        // DeflateStream writes raw DEFLATE (no zlib/gzip header)
        using (var deflate = new DeflateStream(output, CompressionLevel.Optimal, leaveOpen: true))
        {
            deflate.Write(data, 0, data.Length);
        }
        return output.ToArray();
    }

    /// <summary>
    /// Encode bytes using PlantUML's custom base-64 alphabet (6 bits per char, groups of 3 bytes → 4 chars).
    /// </summary>
    private static string ToPlantUmlBase64(byte[] data)
    {
        var sb = new StringBuilder((data.Length * 4 + 2) / 3);

        for (int i = 0; i < data.Length; i += 3)
        {
            int b0 = data[i];
            int b1 = i + 1 < data.Length ? data[i + 1] : 0;
            int b2 = i + 2 < data.Length ? data[i + 2] : 0;

            sb.Append(PlantUmlAlphabet[(b0 >> 2) & 0x3F]);
            sb.Append(PlantUmlAlphabet[((b0 << 4) | (b1 >> 4)) & 0x3F]);
            sb.Append(PlantUmlAlphabet[((b1 << 2) | (b2 >> 6)) & 0x3F]);
            sb.Append(PlantUmlAlphabet[b2 & 0x3F]);
        }

        return sb.ToString();
    }
}
