using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using KmlApiApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Регистрация сервисов
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Требуется Swashbuckle.AspNetCore

string fieldsPath = Path.Combine(builder.Environment.ContentRootPath, "kml", "fields.kml");
string centroidsPath = Path.Combine(builder.Environment.ContentRootPath, "kml", "centroids.kml");

var kmlService = new KmlService(fieldsPath, centroidsPath);
builder.Services.AddSingleton(kmlService);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();      // Требуется Swashbuckle.AspNetCore
    app.UseSwaggerUI();    // Требуется Swashbuckle.AspNetCore
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();