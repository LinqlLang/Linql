using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linql.Server.EF6.Test.DataModel
{
    public static class DataModelConstants
    {
        public const string Schema = "test.";

        public const string ConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=LinqlTest;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;";
        public const string MasterConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;";

    }
}
