using GpsUtil.Location;
using System.Collections.Concurrent;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services.Interfaces;
using TourGuide.Users;

namespace TourGuide.Services;

public class RewardsService : IRewardsService
{
    private const double StatuteMilesPerNauticalMile = 1.15077945;
    private readonly int _defaultProximityBuffer = 10;
    private int _proximityBuffer;
    private readonly int _attractionProximityRange = 200;
    private readonly IGpsUtil _gpsUtil;
    private readonly IRewardCentral _rewardsCentral;
    private readonly ParallelOptions _parallelOptions = new ParallelOptions
    {
        MaxDegreeOfParallelism = 1000,
    };

    public RewardsService(IGpsUtil gpsUtil, IRewardCentral rewardCentral)
    {
        _gpsUtil = gpsUtil;
        _rewardsCentral =rewardCentral;
        _proximityBuffer = _defaultProximityBuffer;
    }

    public void SetProximityBuffer(int proximityBuffer)
    {
        _proximityBuffer = proximityBuffer;
    }

    public void SetDefaultProximityBuffer()
    {
        _proximityBuffer = _defaultProximityBuffer;
    }

    /*public async Task CalculateRewards(User user)
    {
        List<VisitedLocation> userLocations = user.GetAllVisitedLocations();
        List<UserReward> userRewards = user.GetAllUserRewards();
        List<Attraction> attractions = await _gpsUtil.GetAttractions();

        foreach (var visitedLocation in userLocations)
        {
            foreach (var attraction in attractions)
            {
                if (!userRewards.Any(r => r.Attraction.AttractionName == attraction.AttractionName))
                {
                    if (NearAttraction(visitedLocation, attraction))
                    {
                        UserReward userReward = new UserReward(visitedLocation, attraction, await GetRewardPoints(attraction, user));
                        userRewards.Add(userReward);
                        user.AddUserReward(userReward);
                    }
                }
            }
        }
    }*/

    public async Task CalculateRewards(User user)
    {
        List<VisitedLocation> userLocations = user.GetAllVisitedLocations();
        List<UserReward> userRewards = user.GetAllUserRewards();
        List<Attraction> allAttractions = await _gpsUtil.GetAttractions();

        List<Attraction> attractions = allAttractions
        .Where(a => !userRewards.Any(r => r.Attraction.AttractionName == a.AttractionName))
        .ToList();

        ConcurrentBag<UserReward> rewards = new ConcurrentBag<UserReward>();

        await Parallel.ForEachAsync(attractions, _parallelOptions, async (Attraction attraction, CancellationToken token) =>
        {
            VisitedLocation? nearLocation = userLocations.FirstOrDefault(v => NearAttraction(v, attraction));
            if (nearLocation != null)
            {
                UserReward userReward = new UserReward(nearLocation, attraction, await GetRewardPoints(attraction, user));
                rewards.Add(userReward);
            }
        });

        foreach (var reward in rewards)
        {
            user.AddUserReward(reward);
        }
    }

    // SOLUTION B
    /*public async Task CalculateAllUserRewards(List<User> users)
    {
        List<Task> tasks = new List<Task>();

        foreach (var user in users)
        {
            tasks.Add(CalculateRewards(user));
        }

        await Task.WhenAll(tasks);
    }*/

    public bool IsWithinAttractionProximity(Attraction attraction, Locations location)
    {
        Console.WriteLine(GetDistance(attraction, location));
        return GetDistance(attraction, location) <= _attractionProximityRange;
    }

    private bool NearAttraction(VisitedLocation visitedLocation, Attraction attraction)
    {
        return GetDistance(attraction, visitedLocation.Location) <= _proximityBuffer;
    }

    private async Task<int> GetRewardPoints(Attraction attraction, User user)
    {
        return await _rewardsCentral.GetAttractionRewardPoints(attraction.AttractionId, user.UserId);
    }

    public double GetDistance(Locations loc1, Locations loc2)
    {
        double lat1 = Math.PI * loc1.Latitude / 180.0;
        double lon1 = Math.PI * loc1.Longitude / 180.0;
        double lat2 = Math.PI * loc2.Latitude / 180.0;
        double lon2 = Math.PI * loc2.Longitude / 180.0;

        double angle = Math.Acos(Math.Sin(lat1) * Math.Sin(lat2)
                                + Math.Cos(lat1) * Math.Cos(lat2) * Math.Cos(lon1 - lon2));

        double nauticalMiles = 60.0 * angle * 180.0 / Math.PI;
        return StatuteMilesPerNauticalMile * nauticalMiles;
    }
}
