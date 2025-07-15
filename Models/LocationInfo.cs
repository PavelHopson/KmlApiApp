namespace KmlApiApp.Models
{
    public class LocationInfo
    {
        public required double[] Center { get; set; }
        public required double[][] Polygon { get; set; }
    }
}