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
            var nextBikeEndPoint = _appSettings.Value.NextBike?.NextBikeEndPoint!;

            var nearestPlaces = await client.GetAsync(String.Format(nextBikeEndPoint, lat.ToString(CultureInfo.InvariantCulture), lng.ToString(CultureInfo.InvariantCulture)));

            var response = await nearestPlaces.Content.ReadAsStringAsync();
            var jsonObj = JObject.Parse(response);

            var placesArray = (JArray)jsonObj["countries"][0]["cities"][0]["places"];

            var places = placesArray.ToObject<List<Place>>();

            Place nearestPlace = places.FirstOrDefault(x => x.BikesAvailableToRent > 0);

            int? ebikes = nearestPlace.BikeTypes
                .Where(kv => kv.Key.Contains(((int)BikeType.Eletric).ToString()))
                .Select(kv => (int?)kv.Value)
                .FirstOrDefault();

            int? regularBikes = nearestPlace.BikeTypes
                .Where(kv => !kv.Key.Contains(((int)BikeType.Eletric).ToString()))
                .Sum(kv => kv.Value);

            var foundBikeMessageSuccess = _appSettings.Value.NextBike?.FoundBikeMessageSuccess!;
            var message = String.Format(foundBikeMessageSuccess,
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