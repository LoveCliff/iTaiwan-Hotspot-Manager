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
            // 尝试将字符串转为double，兼容各种格式（如带逗号、空值）
            if (double.TryParse(reader.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
                return result;
            return 0; // 转换失败时返回默认值
        }
        return reader.GetDouble();
    }

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}
