using GpsUtil.Location;
using Microsoft.AspNetCore.Mvc;
using TourGuide.Dto.NearbyAttraction;
using TourGuide.Services.Interfaces;
using TourGuide.Users;
using TripPricer;

namespace TourGuide.Controllers;

[ApiController]
[Route("[controller]")]
public class TourGuideController : ControllerBase
{
    private readonly ITourGuideService _tourGuideService;
    private readonly ICreateNearbyAttractionDto _createNearbyAttractionDto;

    public TourGuideController(ITourGuideService tourGuideService, ICreateNearbyAttractionDto createNearbyAttractionDto)
    {
        _tourGuideService = tourGuideService;
        _createNearbyAttractionDto = createNearbyAttractionDto;
    }

    [HttpGet("getLocation")]
    public ActionResult<VisitedLocation> GetLocation([FromQuery] string userName)
    {
        var location = _tourGuideService.GetUserLocation(GetUser(userName));
        return Ok(location);
    }

    [HttpGet("getNearbyAttractions")]
    public ActionResult<List<NearbyAttractionDto>> GetNearbyAttractions([FromQuery] string userName)
    {
        var visitedLocation = _tourGuideService.GetUserLocation(GetUser(userName));
        var attractions = _tourGuideService.GetNearByAttractions(visitedLocation);
        List<NearbyAttractionDto> nearbyAttractionDtos = new List<NearbyAttractionDto>();
        foreach (var attraction in attractions)
        {
            nearbyAttractionDtos.Add(_createNearbyAttractionDto.Generate(attraction, visitedLocation));
        }
        return Ok(nearbyAttractionDtos);
    }

    [HttpGet("getRewards")]
    public ActionResult<List<UserReward>> GetRewards([FromQuery] string userName)
    {
        var rewards = _tourGuideService.GetUserRewards(GetUser(userName));
        return Ok(rewards);
    }

    [HttpGet("getTripDeals")]
    public ActionResult<List<Provider>> GetTripDeals([FromQuery] string userName)
    {
        var deals = _tourGuideService.GetTripDeals(GetUser(userName));
        return Ok(deals);
    }

    private User GetUser(string userName)
    {
        return _tourGuideService.GetUser(userName);
    }
}
