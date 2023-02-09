using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linql.Core.Test
{
    public class DataModel
    {
        #region static Anchors
        
        public static Guid GuidAnchor = Guid.Parse("40869a51-67d7-4c25-ba01-9cb62464c26a");

        #endregion

        #region Built-In Types

        public bool Boolean { get; set; } = false;

        public byte Byte { get; set; }

        public sbyte SByte { get; set; }

        public char Char { get; set; }

        public decimal Decimal { get; set; }

        public double Double { get; set; }

        public float Float { get; set; }

        public int Integer { get; set; }

        public uint UInt { get; set; }

        public long Long { get; set; }

        public ulong ULong { get; set; }

        public short Short { get; set; }

        public ushort UShort { get; set; }

        #endregion

        #region Reference Types
        public string String { get; set; } = "";

        #endregion

        #region CommonClasses
        public DateTime DateTime { get; set; }

        public DateTimeOffset DateTimeOffset { get; set;}

        public Guid Guid { get; set; }

        public List<int> ListInteger { get; set; } = new List<int>();

        public List<string> ListString { get; set; } = new List<string>();

        public List<DataModel> ListRecusrive { get; set; } = new List<DataModel>();

        public List<NullableModel> ListNullableModel { get; set; } = new List<NullableModel>();

        public DataModel OneToOne { get; set; }

        public NullableModel OneToOneNullable { get; set; }

        #endregion
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
