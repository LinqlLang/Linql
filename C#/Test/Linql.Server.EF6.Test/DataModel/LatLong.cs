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
    public class LatLong
    {
        public double Latitude { get; set; }

        public double Longitude { get; set; }

    }
}
