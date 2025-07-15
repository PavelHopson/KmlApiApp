namespace KmlApiApp.Models
{
    public class FieldDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public double Size { get; set; }
        public required LocationInfo Locations { get; set; }
    }
}