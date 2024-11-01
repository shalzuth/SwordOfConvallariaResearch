using System.Net;
using System.Reflection;

namespace NetworkProtocol;

public static class HttpClientExtensions
{
    public static string ToFormUrlEncoded<T>(this T obj)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var keyValuePairs = new List<KeyValuePair<string, string>>();

        foreach (var prop in properties)
        {
            var value = prop.GetValue(obj);
            if (value != null)
            {
                string encodedKey = WebUtility.UrlEncode(prop.Name);
                string encodedValue = WebUtility.UrlEncode(value.ToString());
                keyValuePairs.Add(new KeyValuePair<string, string>(encodedKey, encodedValue));
            }
        }
        return string.Join("&", keyValuePairs.Select(kvp => $"{kvp.Key}={kvp.Value}"));
    }
}
