using Newtonsoft.Json;

namespace romeo_assistant_core.Models.NextBike
{
    public class Place
    {
        [JsonProperty("uid")] public long Uid { get; set; }

        [JsonProperty("lat")] public double Lat { get; set; }

        [JsonProperty("lng")] public double Lng { get; set; }

        [JsonProperty("bike")] public bool Bike { get; set; }

        [JsonProperty("name")] public string Name { get; set; }

        [JsonProperty("address")] public string Address { get; set; }

        [JsonProperty("spot")] public bool Spot { get; set; }

        [JsonProperty("number")] public int Number { get; set; }

        [JsonProperty("booked_bikes")] public int BookedBikes { get; set; }

        [JsonProperty("bikes")] public int Bikes { get; set; }

        [JsonProperty("bikes_available_to_rent")]
        public int BikesAvailableToRent { get; set; }

        [JsonProperty("bike_racks")] public int BikeRacks { get; set; }

        [JsonProperty("free_racks")] public int FreeRacks { get; set; }

        [JsonProperty("special_racks")] public int SpecialRacks { get; set; }

        [JsonProperty("free_special_racks")] public int FreeSpecialRacks { get; set; }

        [JsonProperty("maintenance")] public bool Maintenance { get; set; }

        [JsonProperty("terminal_type")] public string TerminalType { get; set; }

        [JsonProperty("bike_numbers")] public List<string> BikeNumbers { get; set; }

        [JsonProperty("bike_types")] public Dictionary<string, int> BikeTypes { get; set; }

        [JsonProperty("place_type")] public string PlaceType { get; set; }

        [JsonProperty("rack_locks")] public bool RackLocks { get; set; }

        [JsonProperty("dist")] public double Dist { get; set; }
    }
}
