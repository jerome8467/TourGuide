namespace TourGuide.Dto.NearbyAttraction
{
    public class NearbyAttractionDto
    {
        public Guid AttractionId { get; set; }
        public required string AttractionName { get; set; }
        public double LatitudeAttraction { get; set; }
        public double LongitudeAttraction { get; set; }
        public double LatitudeUser { get; set; }
        public double LongitudeUser { get; set; }
        public double MilesDistance { get; set; }
        public int RewardsPoints { get; set; }
    }
}