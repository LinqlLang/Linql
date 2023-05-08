using NetTopologySuite.Geometries;
using System.Text.Json.Serialization;

public class State
{
    public int FID { get; set; }

    public string Program { get; set; }

    public string State_Code { get; set; }

    public string State_Name { get; set; }

    public char Flowing_St { get; set; }

    public int FID_1 { get; set; }

    [JsonIgnore]
    public Geometry Geometry { get; set; }

    public List<StateData> Data { get; set; } = new List<StateData>();

}

public class StateData
{
    public int Year { get; set; }

    public decimal Value { get; set; }

    public string Variable { get; set; }

    public DateTime DateOfRecording { get; set; }

}

public class LatLong
{

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public Point ToPoint()
    {
        return new Point(new Coordinate(this.Latitude, this.Longitude));
    }
}
