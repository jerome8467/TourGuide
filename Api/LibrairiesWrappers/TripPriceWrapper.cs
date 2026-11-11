using TourGuide.LibrairiesWrappers.Interfaces;
using TripPricer;

namespace TourGuide.LibrairiesWrappers
{
    public class TripPriceWrapper : ITripPricer
    {
        private readonly TripPricer.TripPricer _tripPricer;

        public TripPriceWrapper()
        {
            _tripPricer = new();
        }

        public async Task<List<Provider>> GetPrice(string apiKey, Guid attractionId, int adults, int children, int nightsStay, int rewardsPoints)
        {

             return await _tripPricer.GetPrice(apiKey, attractionId, adults, children, nightsStay, rewardsPoints);

        }

    }
}
