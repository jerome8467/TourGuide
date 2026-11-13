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


        // RÉSULTATS HighVolumeTrackLocation (secondes)
        // Sémaphore d'origine (1000)
        // MODIF = Async + Task.WhenAll 

        // Utilisateurs | Avant optimisation | Modif
        // 100          | -                  | 0,12
        // 1 000        | 87,23              | 0,40
        // 10 000       | -                  | 3,80
        // 100 000      | -                  | 32,07

        // Seuil exigé pour 100 000 utilisateurs : 15 min (900 s)
        [Fact]
        public async Task HighVolumeTrackLocation()
        {
            _fixture.Initialize(100);

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


        // RÉSULTATS HighVolumeGetRewards (secondes)
        // MODIF = Async + Task.WhenAll  

        // Utilisateurs | Avant optimisation | Modif sans sémaphore | Modif + sémaphore (1000)
        // 10           | 4,36               | 0,96                 | 0,94
        // 100          | -                  | 1,03                 | 1,05
        // 1 000        | -                  | 1,10                 | 1,30
        // 10 000       | -                  | 2,16                 | 10,71
        // 100 000      | -                  | 11,58                | 104,54

        // Seuil exigé pour 100 000 utilisateurs : 20 min (1 200 s)
        [Fact]
        public async Task HighVolumeGetRewards()
        {
            _fixture.Initialize(100);

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
