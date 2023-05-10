using Linql.Core.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linql.Server.Test
{
    public class CompiledDataModel : DataModel
    {
        public CompiledDataModel(int Index, bool Recursive = false)
        {
            this.Boolean = Index % 2 == 0;
            this.DateTime = DateTime.Now.AddDays(Index);
            this.DateTimeOffset = new DateTimeOffset(this.DateTime);

            decimal indexDecimal = new decimal(Index);
            this.Decimal = indexDecimal * (decimal)Math.PI;
            this.Guid = Guid.NewGuid();

            this.ListInteger = Enumerable.Range(1, 100).ToList();
            this.ListNullableModel = Enumerable.Range(1, 100).Select(r => new NullableModel()).ToList();
            this.ListString = Enumerable.Range(1, 100).Select(r => $"StringValue{Index}{r}").ToList();
            this.Long = (long)this.Decimal;
            this.OneToOneNullable = new NullableModel();

            if(Index == 1)
            {
                this.Guid = DataModel.GuidAnchor;
            }

            if (this.Boolean == true)
            {
                this.OneToOneNullable.Integer = 1;
            }

            this.String = $"MainString{Index}";

            if (Recursive == true)
            {
                this.OneToOne = new CompiledDataModel(-Index);
                this.ListRecusrive = Enumerable.Range(1, 100).Select(r => new CompiledDataModel(r)).Cast<DataModel>().ToList();

            }
        }
    }

}
