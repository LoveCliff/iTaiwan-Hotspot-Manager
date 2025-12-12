using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ItaiwanAPI.Converts;
public class StringToDoubleConverter : JsonConverter<double>
{
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            
            if (double.TryParse(reader.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;
            return 0; 
        }
        return reader.GetDouble();
    }

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}
