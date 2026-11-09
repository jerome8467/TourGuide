using GpsUtil.Location;

namespace TourGuide.Dto.NearbyAttraction
{
    public interface ICreateNearbyAttractionDto
    {
        public NearbyAttractionDto Generate(Attraction attraction, VisitedLocation lastVisitedLocation);
    }
}
