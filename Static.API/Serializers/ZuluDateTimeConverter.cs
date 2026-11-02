using System.Text.Json;
using System.Text.Json.Serialization;

namespace Static.API.Serializers;

/// <summary>
/// Converts <see cref="DateTime"/> values to their ISO 8601 UTC string representation during JSON serialization.
/// </summary>
public class ZuluDateTimeConverter : JsonConverter<DateTime>
{
    /// <summary>
    /// The date and time format string used for serialization to ISO 8601 UTC without milliseconds.
    /// </summary>
    private const string Format = "yyyy-MM-ddTHH:mm:ssZ"; //"yyyy-MM-ddTHH:mm:ss.fffZ"

    /// <summary>
    /// Reads and converts the JSON representation of a <see cref="DateTime"/> value.
    /// </summary>
    /// <param name="reader">The reader to retrieve the <see cref="DateTime"/> value from JSON.</param>
    /// <param name="typeToConvert">The type of the object to convert (expected to be <see cref="DateTime"/>).</param>
    /// <param name="options">Options for serializer settings.</param>
    /// <returns>The deserialized <see cref="DateTime"/> value.</returns>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        // Use the default deserialization behavior to read the DateTimeOffset value from the JSON.
        reader.GetDateTime();

    /// <summary>
    /// Writes a <see cref="DateTime"/> value as a JSON string in ISO 8601 UTC format without milliseconds.
    /// </summary>
    /// <param name="writer">The writer to output the JSON string.</param>
    /// <param name="value">The <see cref="DateTime"/> value to convert and write.</param>
    /// <param name="options">Options for serializer settings.</param>
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToUniversalTime().ToString(Format));
}