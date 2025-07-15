using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System;
using KmlApiApp.Models;
using System.Globalization;

namespace KmlApiApp.Services
{
    public class KmlService
    {
        private readonly List<Field> _fields = new();
        private readonly Dictionary<int, (double Lat, double Lng)> _centroids = new();

        public KmlService(string fieldsPath, string centroidsPath)
        {
            ParseFieldsKml(fieldsPath);
            ParseCentroidsKml(centroidsPath);
            LinkFieldsWithCentroids();
        }

        private void ParseFieldsKml(string filePath)
        {
            var doc = XDocument.Load(filePath);
            var ns = XNamespace.Get("http://www.opengis.net/kml/2.2");
            var placemarks = doc.Descendants(ns + "Placemark");
            foreach (var pm in placemarks)
            {
                // Исправление: Используем ?? "" для избежания null
                var name = pm.Element(ns + "name")?.Value ?? string.Empty;
                
                var extendedData = pm.Descendants(ns + "ExtendedData").FirstOrDefault();
                var fid = int.Parse(extendedData?.Descendants(ns + "SimpleData")
                    .FirstOrDefault(e => e.Attribute("name")?.Value == "fid")?.Value ?? "0");
                var size = double.Parse(extendedData?.Descendants(ns + "SimpleData")
                    .FirstOrDefault(e => e.Attribute("name")?.Value == "size")?.Value ?? "0");

                var coordinatesStr = pm.Descendants(ns + "coordinates").FirstOrDefault()?.Value ?? string.Empty;
                var polygonCoords = ParseCoordinates(coordinatesStr);

                _fields.Add(new Field
                {
                    Id = fid,
                    Name = name, // Теперь Name точно не null
                    Size = size,
                    PolygonCoordinates = polygonCoords // Теперь точно не null
                });
            }
        }

        private void ParseCentroidsKml(string filePath)
        {
            var doc = XDocument.Load(filePath);
            var ns = XNamespace.Get("http://www.opengis.net/kml/2.2");
            var placemarks = doc.Descendants(ns + "Placemark");
            foreach (var pm in placemarks)
            {
                var extendedData = pm.Descendants(ns + "ExtendedData").FirstOrDefault();
                var fid = int.Parse(extendedData?.Descendants(ns + "SimpleData")
                    .FirstOrDefault(e => e.Attribute("name")?.Value == "fid")?.Value ?? "0");
                
                // Исправление: Используем ?? "" для избежания null
                var pointCoords = pm.Descendants(ns + "coordinates").FirstOrDefault()?.Value ?? string.Empty;
                var coords = ParseCoordinates(pointCoords).FirstOrDefault();
                _centroids[fid] = coords;
            }
        }

        private List<(double Lat, double Lng)> ParseCoordinates(string? coordinatesStr)
        {
            var coords = new List<(double Lat, double Lng)>();
            
            // Проверка на null или пустую строку
            if (string.IsNullOrEmpty(coordinatesStr)) return coords;

            var points = coordinatesStr.Trim().Split(' ');
            foreach (var point in points)
            {
                var parts = point.Split(',');
                if (parts.Length >= 2)
                {
                    // Парсим координаты
                    double lng = double.Parse(parts[0], CultureInfo.InvariantCulture);
                    double lat = double.Parse(parts[1], CultureInfo.InvariantCulture);
                    coords.Add((lat, lng));
                }
            }
            return coords;
        }

        private void LinkFieldsWithCentroids()
        {
            foreach (var field in _fields)
            {
                if (_centroids.TryGetValue(field.Id, out var centroid))
                {
                    field.Centroid = centroid;
                }
            }
        }

        // Остальные методы без изменений
        public List<Field> GetAllFields() => _fields;

        public Field? GetFieldById(int id) => _fields.FirstOrDefault(f => f.Id == id);

        public double GetDistanceToCentroid(int fieldId, double pointLat, double pointLng)
        {
            var field = GetFieldById(fieldId);
            if (field == null) throw new ArgumentException("Field not found");
            return CalculateDistance(field.Centroid.Lat, field.Centroid.Lng, pointLat, pointLng);
        }

        public (int? Id, string? Name) CheckPointInFields(double lat, double lng)
{
    foreach (var field in _fields)
    {
        if (IsPointInPolygon(lat, lng, field.PolygonCoordinates))
        {
            return (field.Id, field.Name);
        }
    }
    return (null, null);
}

        private bool IsPointInPolygon(double lat, double lng, List<(double Lat, double Lng)> polygon)
        {
            bool inside = false;
            for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
            {
                var xi = polygon[i].Lat;
                var yi = polygon[i].Lng;
                var xj = polygon[j].Lat;
                var yj = polygon[j].Lng;

                bool intersect = ((yi > lng) != (yj > lng)) &&
                    (lat < (xj - xi) * (lng - yi) / (yj - yi) + xi);
                if (intersect) inside = !inside;
            }
            return inside;
        }

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // Earth radius in meters
            var φ1 = lat1 * Math.PI / 180;
            var φ2 = lat2 * Math.PI / 180;
            var Δφ = (lat2 - lat1) * Math.PI / 180;
            var Δλ = (lon2 - lon1) * Math.PI / 180;

            var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                    Math.Cos(φ1) * Math.Cos(φ2) * Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }
}