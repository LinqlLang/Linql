using System.Text.Json.Serialization;
using Linql.Client;
using NetTopologySuite.IO.Converters;

public class CustomLinqlContext : LinqlContext
{
    public CustomLinqlContext(string BaseUrl) : base(BaseUrl)
    {
        var geoJsonConverterFactory = new GeoJsonConverterFactory();
        this.JsonOptions.Converters.Add(geoJsonConverterFactory);
        this.JsonOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;

    }

    protected override string GetEndpoint(Type QueryableType)
    {
        return $"{QueryableType.Name}";
    }
}