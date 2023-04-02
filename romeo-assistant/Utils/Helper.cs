using SharpToken;

namespace romeo_assistant.Utils
{
    public static class Helper
    {
        public static int CalculateTokenSize(string message)
        {
            var encoding = GptEncoding.GetEncoding("cl100k_base");
            var encoded = encoding.Encode(message);

            return encoded.Count;
        }

        public static DateTime ToLocalTime(long timestamp) => DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime.ToLocalTime();
    }
}
