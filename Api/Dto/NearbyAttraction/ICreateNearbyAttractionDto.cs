using GpsUtil.Location;

namespace TourGuide.Dto.NearbyAttraction
{
    public interface ICreateNearbyAttractionDto
    {
        public Task<NearbyAttractionDto> Generate(Attraction attraction, VisitedLocation lastVisitedLocation);
    }
}
