using GpsUtil.Location;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourGuide.LibrairiesWrappers.Interfaces;
using TourGuide.Services;
using TourGuide.Services.Interfaces;
using TourGuide.Users;
using TourGuide.Utilities;
using Xunit.Abstractions;

namespace TourGuideTest
{
    public class PerformanceTest : IClassFixture<DependencyFixture>
    {
        private readonly DependencyFixture _fixture;

        private readonly ITestOutputHelper _output;

        public PerformanceTest(DependencyFixture fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        ParallelOptions parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = 1000,
        };

        [Fact]
        public async Task HighVolumeTrackLocation()
        {
            _fixture.Initialize(100000);

            List<User> allUsers = _fixture.TourGuideService.GetAllUsers();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // SOLUTION B
            /*await _fixture.TourGuideService.TrackAllUserLocation(allUsers);*/

            await Parallel.ForEachAsync(allUsers, parallelOptions, async (User user, CancellationToken token) =>
            {
                await _fixture.TourGuideService.TrackUserLocation(user);
            });

            stopWatch.Stop();
            _fixture.TourGuideService.Tracker.StopTracking();

            _output.WriteLine($"highVolumeTrackLocation: Time Elapsed: {stopWatch.Elapsed.TotalSeconds} seconds.");

            Assert.True(TimeSpan.FromMinutes(15).TotalSeconds >= stopWatch.Elapsed.TotalSeconds);
        }

        [Fact]
        public async Task HighVolumeGetRewards()
        {
            _fixture.Initialize(100000);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            List<Attraction> attractions = await _fixture.GpsUtil.GetAttractions();
            Attraction attraction = attractions[0];
            List<User> allUsers = _fixture.TourGuideService.GetAllUsers();
            allUsers.ForEach(u => u.AddToVisitedLocations(new VisitedLocation(u.UserId, attraction, DateTime.Now)));

            // SOLUTION B
            /*await _fixture.RewardsService.CalculateAllUserRewards(allUsers);*/

            await Parallel.ForEachAsync(allUsers, parallelOptions, async (User user, CancellationToken token) =>
            {
                await _fixture.RewardsService.CalculateRewards(user);
            });

            foreach (var user in allUsers)
            {
                Assert.True(user.UserRewards.Count > 0);
            }

            stopWatch.Stop();
            _fixture.TourGuideService.Tracker.StopTracking();

            _output.WriteLine($"highVolumeGetRewards: Time Elapsed: {stopWatch.Elapsed.TotalSeconds} seconds.");
            Assert.True(TimeSpan.FromMinutes(20).TotalSeconds >= stopWatch.Elapsed.TotalSeconds);
        }
    }
}
