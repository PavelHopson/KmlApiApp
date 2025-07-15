using System.Collections.Generic;
namespace KmlApiApp.Models
{
    public class Field
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public double Size { get; set; }
        public required List<(double Lat, double Lng)> PolygonCoordinates { get; set; }
        public (double Lat, double Lng) Centroid { get; set; }
    }
}