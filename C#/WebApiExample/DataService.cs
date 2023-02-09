using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace WebApiExample
{
    public class DataService
    {
        public List<State> StateData { get; set; } = new List<State>();

        public DataService() 
        {
            Random random = new Random();
            string data = System.IO.File.ReadAllText("Files/States_shapefile.geojson");
            var reader = new NetTopologySuite.IO.GeoJsonReader();
            var featureCollection = reader.Read<FeatureCollection>(data);

            foreach(Feature feature in featureCollection ) 
            {
                State state = new State();
                state.Geometry = feature.Geometry;

                feature.Attributes.GetNames().ToList().ForEach(r =>
                {
                    PropertyInfo property = typeof(State).GetProperty(r);
                    object value = feature.Attributes.GetOptionalValue(r);
                    property.SetValue(state, Convert.ChangeType(value, property.PropertyType));

                });

                this.StateData.Add(state);

                Enumerable.Range(DateTime.Now.Year - 5, 10).ToList().ForEach(r =>
                {
                    StateData data = new StateData();
                    data.Year = r;
                    DateTime observation = DateTime.Parse($"1/{random.Next(1, 12)}/{r}");
                    data.DateOfRecording = observation;
                    data.Variable = "Population";
                    data.Value = random.Next(500000, 1000000) * random.Next(1, 12);
                    state.Data.Add(data);
                    
                });
            }

            Point point = new Point(new Coordinate(-77.036530, 38.897675));
            Point point2 = new Point(new Coordinate(38.897675, -77.036530));
            var testpoint = this.StateData.AsQueryable().Where(r => r.Geometry.Contains(point));
            var testpoint2 = this.StateData.FirstOrDefault(r => r.Geometry.Contains(point2));


        }
    }
}
