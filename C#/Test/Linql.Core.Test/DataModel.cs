using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linql.Core.Test
{
    public class DataModel
    {
        public int Integer { get; set; }

        public string String { get; set; } = "";

        public bool Boolean { get; set; } = false;

        public decimal Decimal { get; set; }

        public long Long { get; set; }

        public DateTime DateTime { get; set; }

        public DateTimeOffset DateTimeOffset { get; set;}

        public Guid Guid { get; set; }

        public List<int> ListInteger { get; set; } = new List<int>();

        public List<string> ListString { get; set; } = new List<string>();

        public List<DataModel> ListRecusrive { get; set; } = new List<DataModel>();

        public List<NullableModel> ListNullableModel { get; set; } = new List<NullableModel>();

        public DataModel OneToOne { get; set; }

        public NullableModel OneToOneNullable { get; set; }
    }

    public class NullableModel
    {
        public int? Integer { get; set; }

        public string? String { get; set; }

        public decimal? Decimal { get; set; }

        public long? Long { get; set; }

        public DateTime? DateTime { get; set; }

        public DateTimeOffset? DateTimeOffset { get; set; }

        public Guid? Guid { get; set; }

        public List<int>? ListInteger { get; set; }

        public List<string>? ListString { get; set; }

        public List<DataModel>? ListRecusrive { get; set; }

        public DataModel? OneToOne { get; set; }
    }
}
