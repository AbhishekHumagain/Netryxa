using System.Text.Json;
using System.Text.Json.Serialization;

namespace Static.API.Serializers;

/// <summary>
/// Converts <see cref="DateTimeOffset"/> values to their ISO 8601 UTC string representation during JSON serialization.
/// </summary>
public class ZuluDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    /// <summary>
    /// The date and time format string used for serialization to ISO 8601 UTC without milliseconds.
    /// </summary>
    private const string Format = "yyyy-MM-ddTHH:mm:ssZ"; //"yyyy-MM-ddTHH:mm:ss.fffZ"

    /// <summary>
    /// Reads and converts the JSON representation of a <see cref="DateTimeOffset"/> value.
    /// </summary>
    /// <param name="reader">The reader to retrieve the <see cref="DateTimeOffset"/> value from JSON.</param>
    /// <param name="typeToConvert">The type of the object to convert (expected to be <see cref="DateTimeOffset"/>).</param>
    /// <param name="options">Options for serializer settings.</param>
    /// <returns>The deserialized <see cref="DateTimeOffset"/> value.</returns>
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        // Use the default deserialization behavior to read the DateTimeOffset value from the JSON.
        reader.GetDateTimeOffset();

    /// <summary>
    /// Writes a <see cref="DateTimeOffset"/> value as a JSON string in ISO 8601 UTC format with milliseconds.
    /// </summary>
    /// <param name="writer">The writer to output the JSON string.</param>
    /// <param name="value">The <see cref="DateTimeOffset"/> value to convert and write.</param>
    /// <param name="options">Options for serializer settings.</param>
    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) =>
        // Convert the DateTimeOffset value to UTC and format it as an ISO 8601 string with milliseconds,
        // appending 'Z' to indicate UTC time.
        writer.WriteStringValue(value.UtcDateTime.ToString(Format));
}