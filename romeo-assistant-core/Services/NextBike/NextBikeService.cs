using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using romeo_assistant_core.Models.Configuration;
using romeo_assistant_core.Models.NextBike;
using romeo_assistant_core.Models.Whatsapp;
using romeo_assistant_core.Utils;

namespace romeo_assistant_core.Services.NextBike
{
    public class NextBikeService : INextBikeService
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public NextBikeService(IOptions<AppSettings> appSettings, IHttpClientFactory httpClientFactory)
        {
            _appSettings = appSettings;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<LocationMessage> GetNextBikeDataByLocationAsync(double lat, double lng)
        {
            var client = _httpClientFactory.CreateClient();
            var x = await client.GetAsync("https://maps.nextbike.net/maps/nextbike-live.json?city=789&domains=ea&list_cities=0&bikes=0");

            var response = await x.Content.ReadAsStringAsync();
            var jsonObj = JObject.Parse(response);

            var placesArray = (JArray)jsonObj["countries"][0]["cities"][0]["places"];

            var places = placesArray.ToObject<List<Place>>();

            Place nearestPlace = Helper.FindNearestPlace(lat, lng, places.Where(x => x.BikesAvailableToRent > 0).ToList());
            Console.WriteLine(nearestPlace);


            var distance = Helper.HaversineDistance(lat, lng, nearestPlace.Lat, nearestPlace.Lng);


            int? ebikes = nearestPlace.BikeTypes
                .Where(kv => kv.Key.Contains(((int)BikeType.Eletric).ToString()))
                .Select(kv => (int?)kv.Value)
                .FirstOrDefault();

            int? regularBikes = nearestPlace.BikeTypes
                .Where(kv => !kv.Key.Contains(((int)BikeType.Eletric).ToString()))
                .Sum(kv => kv.Value);

            var message = String.Format("La estación más cercana a ti con alguna bici disponible esta a {0} de distancia\r\n📍 {1} \r\n\r\n🚲 Bicis disponibles: {2} \r\n\r\n⚡ Eléctricas: {3} \r\n💩 Chustas: {4} ",
                Helper.ToHumanReadable(distance),
                nearestPlace.Name,
                nearestPlace.Bikes,
                ebikes ?? 0,
                regularBikes ?? 0
                );

            return new LocationMessage
            {
                Title = $"📍 {nearestPlace.Name}",
                Lat = nearestPlace.Lat,
                Lng = nearestPlace.Lng,
                Text = message,
                distanceM2 = distance
            };
        }
    }
}