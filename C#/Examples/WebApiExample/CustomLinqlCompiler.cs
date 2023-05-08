using Linql.Server;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using System.Reflection;
using System.Text.Json;

namespace WebApiExample
{
    public class CustomLinqlCompiler : LinqlCompiler
    {
        public CustomLinqlCompiler() : base() 
        { 
            this.ValidAssemblies = new HashSet<Assembly>()
            {
                typeof(Boolean).Assembly,
                typeof(Enumerable).Assembly,
                typeof(Queryable).Assembly,
                typeof(Geometry).Assembly,
                typeof(State).Assembly,
            };

            var geoJsonConverterFactory = new GeoJsonConverterFactory();
            this.JsonOptions.Converters.Add(geoJsonConverterFactory);
        }
    }
}
