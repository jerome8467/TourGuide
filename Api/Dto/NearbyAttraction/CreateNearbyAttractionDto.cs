using GpsUtil.Location;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;

namespace TourGuide.Dto.NearbyAttraction
{
    public class CreateNearbyAttractionDto : ICreateNearbyAttractionDto
    {
        private readonly IRewardsService _rewardsService;
        private readonly IRewardCentral _rewardCentral;

        public CreateNearbyAttractionDto(IRewardsService rewardsService, IRewardCentral rewardCentral)
        {
            _rewardsService = rewardsService;
            _rewardCentral = rewardCentral;
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
                RewardsPoints = await _rewardCentral.GetAttractionRewardPoints(attraction.AttractionId, lastVisitedLocation.UserId)
            };
        }

    }
}
