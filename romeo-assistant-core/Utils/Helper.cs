using SharpToken;

namespace romeo_assistant_core.Utils
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

        public static string ToHumanReadable(double distanceInMeters)
        {
            if (distanceInMeters < 1000)
            {
                return $"{Math.Round(distanceInMeters, 2)} m";
            }
            else
            {
                double distanceInKilometers = distanceInMeters / 1000.0;
                return $"{Math.Round(distanceInKilometers, 2)} km";
            }
        }

    }
}
