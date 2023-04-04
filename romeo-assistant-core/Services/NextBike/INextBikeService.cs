using romeo_assistant_core.Models.Whatsapp;

namespace romeo_assistant_core.Services.NextBike
{
    public interface INextBikeService
    {
        Task<LocationMessage> GetNextBikeDataByLocationAsync(double lat, double lng);
    }
}
