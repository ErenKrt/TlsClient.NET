using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace TlsClient.HttpClient.Helpers
{
    public static class HttpRequestMessageExtensions
    {
        private static readonly Regex Base64Regex = new Regex("^data:([^;]+);.*?base64,(.+)$");
        private static readonly HashSet<string> JoinWithComma = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Accept",
            "Accept-Encoding",
            "Accept-Language",
            "Cookie",
            "Cache-Control",
            "Connection",
            "Pragma"
        };

        private static readonly HashSet<string> JoinWithSpace = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "User-Agent",
            "Via"
        };

        public static Dictionary<string, string> GetHeaderDictionary(this HttpRequestMessage request)
        {
            var headers = new List<KeyValuePair<string, IEnumerable<string>>>();

            headers.AddRange(request.Headers);

            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var h in headers)
            {
                string value = JoinWithComma.Contains(h.Key)
                    ? string.Join(", ", h.Value)
                    : JoinWithSpace.Contains(h.Key)
                        ? string.Join(" ", h.Value)
                        : string.Join("", h.Value);

                result[h.Key] = value;
            }

            return result;
        }

        public static (string, string) ToParsedBase64(this string data)
        {
            var match = Base64Regex.Match(data);
            if (match.Success)
            {
                var mimeType = match.Groups[1].Value;
                var base64Data = match.Groups[2].Value;
                return (mimeType, base64Data);
            }

            return (string.Empty, string.Empty);
        }
    }
}
