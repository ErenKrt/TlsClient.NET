using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace TlsClient.Core.Helpers
{
    public static class RequestHelpers
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // Null değerleri yoksay
            WriteIndented = true,                                        // Indent edilmiş JSON
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase             // camelCase isimlendirme
        };

        public static string ConvertJson(object data) => JsonSerializer.Serialize(data, _jsonOptions);
        private static byte[] GetBytes(string data) => Encoding.UTF8.GetBytes(data);
        public static byte[] Prepare(object data) => GetBytes(ConvertJson(data));
        public static string PrepareBody(byte[] data) => Convert.ToBase64String(data);
        public static T ConvertObject<T>(string json) => JsonSerializer.Deserialize<T>(json, _jsonOptions)!;
    }
}
