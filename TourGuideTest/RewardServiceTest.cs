using GpsUtil.Location;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourGuide.Users;
using TourGuide.Utilities;
using Xunit.Abstractions;

namespace TourGuideTest;

public class RewardServiceTest : IClassFixture<DependencyFixture>
{
    private readonly DependencyFixture _fixture;
    private readonly ITestOutputHelper _output;

    public RewardServiceTest(DependencyFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;
    }

    [Fact]
    public async Task UserGetRewards()
    {
        _fixture.Initialize(0);
        var user = new User(Guid.NewGuid(), "jon", "000", "jon@tourGuide.com");
        var attractions = await _fixture.GpsUtil.GetAttractions();
        var attraction = attractions.First();
        user.AddToVisitedLocations(new VisitedLocation(user.UserId, attraction, DateTime.Now));
        await _fixture.TourGuideService.TrackUserLocation(user);
        var userRewards = user.UserRewards;
        _fixture.TourGuideService.Tracker.StopTracking();
        Assert.True(userRewards.Count == 1);
    }

    [Fact]
    public async Task IsWithinAttractionProximity()
    {
        var attractions = await _fixture.GpsUtil.GetAttractions();
        var attraction = attractions.First();
        Assert.True(_fixture.RewardsService.IsWithinAttractionProximity(attraction, attraction));
    }

    [Fact]
    public async Task NearAllAttractions()
    {
        _fixture.Initialize(1);
        _fixture.RewardsService.SetProximityBuffer(int.MaxValue);

        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();

        var user = _fixture.TourGuideService.GetAllUsers().First();
        await _fixture.RewardsService.CalculateRewards(user);
        var userRewards = _fixture.TourGuideService.GetUserRewards(user);

        stopWatch.Stop();
        _fixture.TourGuideService.Tracker.StopTracking();
        _output.WriteLine($"NearAllAttractions: Time Elapsed: {stopWatch.Elapsed.TotalSeconds} seconds.");

        var attractions = await _fixture.GpsUtil.GetAttractions();
        Assert.Equal(attractions.Count, userRewards.Count);
    }

}
