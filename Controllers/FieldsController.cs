using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;
using KmlApiApp.Models;
using KmlApiApp.Services;

namespace KmlApiApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FieldsController : ControllerBase
    {
        private readonly KmlService _kmlService;

        public FieldsController(KmlService kmlService) => _kmlService = kmlService;

        [HttpGet]
        public ActionResult<List<FieldDto>> GetAllFields()
        {
            var fields = _kmlService.GetAllFields().Select(f => new FieldDto
            {
                Id = f.Id,
                Name = f.Name,
                Size = f.Size,
                Locations = new LocationInfo
                {
                    Center = new[] { f.Centroid.Lat, f.Centroid.Lng },
                    Polygon = f.PolygonCoordinates.Select(p => new[] { p.Lat, p.Lng }).ToArray()
                }
            }).ToList();
            return Ok(fields);
        }

        [HttpGet("{id}")]
        public ActionResult<double> GetSize(int id)
        {
            var field = _kmlService.GetFieldById(id);
            if (field == null) return NotFound();
            return Ok(field.Size);
        }

        [HttpGet("distance")]
        public ActionResult<double> GetDistance(int fieldId, double lat, double lng)
        {
            try
            {
                var distance = _kmlService.GetDistanceToCentroid(fieldId, lat, lng);
                return Ok(distance);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("point-inside")]
        public ActionResult<object> CheckPointInside(double lat, double lng)
        {
            var result = _kmlService.CheckPointInFields(lat, lng);
            if (result.Id.HasValue)
            {
                return Ok(new { id = result.Id.Value, name = result.Name });
            }
            return Ok(false);
        }
    }
}