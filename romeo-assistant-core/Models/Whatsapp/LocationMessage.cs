namespace romeo_assistant_core.Models.Whatsapp
{
    public class LocationMessage
    {
        public string? Title { get; set; }
        public string? Text { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }

        public double distanceM2 { get; set; }
    }
}