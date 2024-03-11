namespace News.DbModels
{
    public class Waypoint
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public virtual DbImageFile? ImageFile { get; set; }
        public string? Description { get; set; }
    }
}
