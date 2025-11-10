using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Blogsphere.Search.Api.Converters;

public class CustomDateTimeConverter : IsoDateTimeConverter
{
    public CustomDateTimeConverter()
    {
        DateTimeFormat = "yyyy-MM-dd HH.mm.ss";
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.Value == null)
            return null;

        if (reader.Value is DateTime dateTime)
            return dateTime;

        if (reader.Value is string dateString)
        {
            // Try the custom format first
            if (DateTime.TryParseExact(dateString, "yyyy-MM-dd HH.mm.ss", 
                System.Globalization.CultureInfo.InvariantCulture, 
                System.Globalization.DateTimeStyles.None, out DateTime result))
            {
                return result;
            }

            // Fallback to standard parsing
            if (DateTime.TryParse(dateString, out DateTime parsedDate))
            {
                return parsedDate;
            }

            // If parsing fails, return default (min value) or null
            if (objectType == typeof(DateTime?) || Nullable.GetUnderlyingType(objectType) != null)
                return null;
            
            return DateTime.MinValue;
        }

        return base.ReadJson(reader, objectType, existingValue, serializer);
    }
}

