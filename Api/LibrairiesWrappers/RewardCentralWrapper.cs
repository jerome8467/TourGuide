using TourGuide.LibrairiesWrappers.Interfaces;

namespace TourGuide.LibrairiesWrappers
{
    public class RewardCentralWrapper : IRewardCentral
    {
        private static readonly SemaphoreSlim rateLimiter = new(1000, 1000);
        private readonly RewardCentral.RewardCentral _rewardCentral;

        public RewardCentralWrapper()
        {
            _rewardCentral = new ();
        }

        public async Task<int> GetAttractionRewardPoints(Guid attractionId, Guid userId)
        {
            await rateLimiter.WaitAsync();
            try
            {
                return await _rewardCentral.GetAttractionRewardPoints(attractionId, userId);
            }
            finally { rateLimiter.Release(); }
        }
    }
}
