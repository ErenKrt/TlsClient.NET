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
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public static string ToBase64(this byte[] data) => Convert.ToBase64String(data);
        public static byte[] FromBase64(this string base64) => Convert.FromBase64String(base64);

        public static string ToJson(this object data) => JsonSerializer.Serialize(data, _jsonOptions);

        public static T FromJson<T>(this string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return default!;
            return JsonSerializer.Deserialize<T>(json, _jsonOptions)!;
        }

        private static byte[] ToBytes(string data) => Encoding.UTF8.GetBytes(data);
        public static string ToStringFromBytes(this byte[] data) => Encoding.UTF8.GetString(data);

        public static byte[] Prepare(object data) => ToBytes(ToJson(data));
        public static string PrepareBody(byte[] data) => Convert.ToBase64String(data);
        
    }
}
