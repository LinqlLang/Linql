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

}