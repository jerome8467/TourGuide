using GpsUtil.Location;
using TripPricer;

namespace TourGuide.Users;

public class User
{
    public Guid UserId { get; }
    public string UserName { get; }
    public string PhoneNumber { get; set; }
    public string EmailAddress { get; set; }
    public DateTime LatestLocationTimestamp { get; set; }
    public List<VisitedLocation> VisitedLocations { get; } = new List<VisitedLocation>();
    public List<UserReward> UserRewards { get; } = new List<UserReward>();
    public UserPreferences UserPreferences { get; set; } = new UserPreferences();
    public List<Provider> TripDeals { get; set; } = new List<Provider>();
    private readonly object _lock = new object();


    public User(Guid userId, string userName, string phoneNumber, string emailAddress)
    {
        UserId = userId;
        UserName = userName;
        PhoneNumber = phoneNumber;
        EmailAddress = emailAddress;
    }

    public void AddToVisitedLocations(VisitedLocation visitedLocation)
    {
        lock (_lock)
        {
            VisitedLocations.Add(visitedLocation);
        }
    }

    public void ClearVisitedLocations()
    {
        lock (_lock)
        {
            VisitedLocations.Clear();
        }
    }

    public void AddUserReward(UserReward userReward)
    {
        lock (_lock)
        {
            if (!UserRewards.Exists(r => r.Attraction.AttractionName == userReward.Attraction.AttractionName))
            {
                UserRewards.Add(userReward);
            }
        }
    }

    public VisitedLocation GetLastVisitedLocation()
    {
        lock (_lock)
        {
            return VisitedLocations[^1];
        }
    }

    public List<VisitedLocation> GetAllVisitedLocations()
    {
        lock (_lock)
        {
            return VisitedLocations.ToList();
        }
    }

    public List<UserReward> GetAllUserRewards()
    {
        lock (_lock)
        {
            return UserRewards.ToList();
        }
    }
}
