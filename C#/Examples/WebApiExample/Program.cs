using Linql.Server;
using Microsoft.AspNetCore.Http.Json;
using NetTopologySuite;
using NetTopologySuite.IO.Converters;
using System.Text.Json;
using WebApiExample;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.AllowAnyHeader();
                          policy.AllowAnyMethod();
                          policy.AllowAnyOrigin();
                      });
});

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        
        // this constructor is overloaded.  see other overloads for options.
        var geoJsonConverterFactory = new GeoJsonConverterFactory();
        options.JsonSerializerOptions.Converters.Add(geoJsonConverterFactory);
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        
    });

builder.Services.AddSingleton(NtsGeometryServices.Instance);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<DataService, DataService>();

builder.Services.AddSingleton<LinqlCompiler, CustomLinqlCompiler>();


var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.UseRouting();

app.MapControllers();

app.Run();
