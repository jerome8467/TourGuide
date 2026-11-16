using GpsUtil.Location;
using TourGuide.Services.Interfaces;

namespace TourGuide.Dto.NearbyAttraction
{
    public class CreateNearbyAttractionDto : ICreateNearbyAttractionDto
    {
        private readonly IRewardsService _rewardsService;

        public CreateNearbyAttractionDto(IRewardsService rewardsService)
        {
            _rewardsService = rewardsService;
        }

        public async Task<NearbyAttractionDto> Generate(Attraction attraction, VisitedLocation lastVisitedLocation)
        {
            return new NearbyAttractionDto
            {
                AttractionId = attraction.AttractionId,
                AttractionName = attraction.AttractionName,
                LatitudeAttraction = attraction.Latitude,
                LongitudeAttraction = attraction.Longitude,
                LatitudeUser = lastVisitedLocation.Location.Latitude,
                LongitudeUser = lastVisitedLocation.Location.Longitude,
                MilesDistance = _rewardsService.GetDistance(attraction, lastVisitedLocation.Location),
                RewardsPoints = await _rewardsService.GetRewardPoints(attraction, lastVisitedLocation.UserId)
            };
        }

    }
}
