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


    }
}
