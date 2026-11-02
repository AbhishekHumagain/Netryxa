using System.Text.Json;
using System.Text.Json.Serialization;

namespace Static.Helper.JsonSerializer;

public static class JsonSerializerExtensions
{
    private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };

    public static string Serialize<T>(this T obj, JsonSerializerOptions? options = null) => System.Text.Json.JsonSerializer.Serialize(obj, options ?? DefaultJsonSerializerOptions);

    public static T? Deserialize<T>(this string value, JsonSerializerOptions? options = null) => System.Text.Json.JsonSerializer.Deserialize<T>(value, options ?? DefaultJsonSerializerOptions);
    
    public static byte[] SerializeToUtf8Bytes<T>(this T obj, JsonSerializerOptions? options = null) => System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(obj, options ?? DefaultJsonSerializerOptions);
    
    public static T? Deserialize<T>(this byte[] utf8Json, JsonSerializerOptions? options = null) => System.Text.Json.JsonSerializer.Deserialize<T>(utf8Json, options ?? DefaultJsonSerializerOptions);

}