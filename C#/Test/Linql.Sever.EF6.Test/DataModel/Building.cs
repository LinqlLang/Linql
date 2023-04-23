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

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public DbGeography Geography { get; set; }

        public DbGeometry Geometry { get; set; }


    }
}
