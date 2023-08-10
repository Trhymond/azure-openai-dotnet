 
namespace Rhymond.OpenAI.Extensions;

internal static class ObjectExtensions
{
    internal static T? ReadFromJson<T>(this HttpRequestData req)
    {
        Stream stream = req.Body;
        using StreamReader reader = new(stream);        
        string jsonString = reader.ReadToEnd() ?? "";
        T? result = (T?)JsonConvert.DeserializeObject<T>(jsonString);

        return result;
    }

    internal static string? WriteToJson<T>(this T data)
    {
        string result = JsonConvert.SerializeObject(data);

        return result;
    }
}


