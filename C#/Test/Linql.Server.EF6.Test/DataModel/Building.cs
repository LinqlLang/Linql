using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linql.Server.EF6.Test.DataModel
{
    [Table(DataModelConstants.Schema + nameof(Building))]
    public class Building
    {
        [Key]
        public int BuildingID { get; set; }

        [MaxLength(50)]
        [Required]
        public string BuildingName { get; set; } = "";

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DbGeography Geography { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DbGeometry Geometry { get; set;  }

        public virtual List<BuildingData> Data { get; set; } 


    }

    [Table(DataModelConstants.Schema + nameof(BuildingData))]
    public class BuildingData
    {
        [Key, Column(Order = 1)]
        public int BuildingID { get; set; }

        [ForeignKey(nameof(BuildingData.BuildingID))]
        public virtual Building Building { get; set; }

        [Key, Column(Order = 2)]
        public int Year { get; set; }

        [Key, Column(Order = 3)]
        public int VariableID { get; set; }

        [ForeignKey(nameof(BuildingData.VariableID))]
        public virtual Variable Variable { get; set; }

        public double Value { get; set; }

        public BuildingData() { }

        public BuildingData(int year, Variable Variable, double value)
        {
            this.Year = year;
            this.Variable = Variable;
            this.Value = value;
        }
    }

    [Table(DataModelConstants.Schema + nameof(Variable))]
    public class Variable
    {
        [Key]
        public int VariableID { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [MaxLength(50)]
        public string VariableName { get; set; }
    }
}
