using TourGuide.LibrairiesWrappers.Interfaces;
using TripPricer;

namespace TourGuide.LibrairiesWrappers
{
    public class TripPriceWrapper : ITripPricer
    {
        private static readonly SemaphoreSlim rateLimiter = new(1000, 1000);
        private readonly TripPricer.TripPricer _tripPricer;

        public TripPriceWrapper()
        {
            _tripPricer = new();
        }

        public async Task<List<Provider>> GetPrice(string apiKey, Guid attractionId, int adults, int children, int nightsStay, int rewardsPoints)
        {
            await rateLimiter.WaitAsync();
            try
            {
                return await _tripPricer.GetPrice(apiKey, attractionId, adults, children, nightsStay, rewardsPoints);
            }
            finally { rateLimiter.Release(); }
        }

    }
}
