using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using romeo_assistant_core.Models.Configuration;
using romeo_assistant_core.Models.NextBike;
using romeo_assistant_core.Models.Whatsapp;
using romeo_assistant_core.Utils;
using System.Globalization;

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
            var nearestPlaces = await client.GetAsync($"https://maps.nextbike.net/maps/nextbike.json?city=789&lat={lat.ToString(CultureInfo.InvariantCulture)}&lng={lng.ToString(CultureInfo.InvariantCulture)}&limit=5&distance=1000&bikes=0");

            var response = await nearestPlaces.Content.ReadAsStringAsync();
            var jsonObj = JObject.Parse(response);

            var placesArray = (JArray)jsonObj["countries"][0]["cities"][0]["places"];

            var places = placesArray.ToObject<List<Place>>();

            Place nearestPlace = places.FirstOrDefault(x => x.BikesAvailableToRent > 0);

            Console.WriteLine(nearestPlace);

            int? ebikes = nearestPlace.BikeTypes
                .Where(kv => kv.Key.Contains(((int)BikeType.Eletric).ToString()))
                .Select(kv => (int?)kv.Value)
                .FirstOrDefault();

            int? regularBikes = nearestPlace.BikeTypes
                .Where(kv => !kv.Key.Contains(((int)BikeType.Eletric).ToString()))
                .Sum(kv => kv.Value);

            var message = String.Format("La estación más cercana a ti con alguna bici disponible esta a {0} de distancia\r\n📍 {1} \r\n\r\n🚲 Bicis disponibles: {2} \r\n\r\n⚡ Eléctricas: {3} \r\n💩 Chustas: {4} ",
                Helper.ToHumanReadable(nearestPlace.Dist),
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
                distanceM2 = nearestPlace.Dist
            };
        }
    }
}