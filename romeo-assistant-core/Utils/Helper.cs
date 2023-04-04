using romeo_assistant_core.Models.NextBike;
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

        public static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371e3; // Earth's radius in meters
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        public static Place FindNearestPlace(double lat, double lon, List<Place> places)
        {
            Place nearestPlace = null;
            var minDistance = double.MaxValue;

            foreach (var place in places)
            {
                var distance = HaversineDistance(lat, lon, place.Lat, place.Lng);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPlace = place;
                }
            }

            return nearestPlace;
        }

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

        private static double ToRadians(double degrees) => degrees * (Math.PI / 180);
    }
}
