using WallStreetCompact.Models;

namespace WallStreetCompact.Services
{
    public interface IPredictionService
    {
        Task CreatePredictionAsync(double price, int id);

        List<Prediction> GetAllPredictions();
    }
}
