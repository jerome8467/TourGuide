using TripPricer;

namespace TourGuide.LibrairiesWrappers.Interfaces
{
    public interface ITripPricer
    {
        public Task<List<Provider>> GetPrice(string apiKey, Guid attractionId, int adults, int children, int nightsStay, int rewardsPoints);
    }
}
