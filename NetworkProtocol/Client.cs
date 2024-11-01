using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace NetworkProtocol
{
    internal static class Client
    {
        internal static HttpClient client = new HttpClient { };
        internal static async Task<JsonNode> Request(string url, string json)
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadFromJsonAsync<JsonNode>();
                return responseContent;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
            return null;
        }
        internal static async Task<JsonNode> Get(string url)
        {
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadFromJsonAsync<JsonNode>();
                return responseContent;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
            return null;
        }
        internal static async Task<JsonNode> Post(string url, object obj)
        {
            var content = new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
            var response = await client.PostAsync(url, content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadFromJsonAsync<JsonNode>();
                return responseContent;
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
            return null;
        }
    }
}
