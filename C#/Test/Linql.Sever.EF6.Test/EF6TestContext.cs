using Linql.Server.EF6.Test.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linql.Server.EF6.Test
{
    public class EF6TestContext<T> : DbContext where T: class, new()
    {
        public DbSet<T> ResultSet { get; set; }

        public EF6TestContext(string ConnectionString = DataModelConstants.ConnectionString) : base(ConnectionString)
        {
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SqlClient.SqlClientFactory.Instance);
            ////for Connection
            var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            DbConnection connection = factory.CreateConnection();
            
        }

        public async Task Init(bool Reset = true)
        {
            if (Reset)
            {
                string text = await File.ReadAllTextAsync("./Scripts/DbSetup.sql");
                using (SqlConnection con = new SqlConnection(DataModelConstants.MasterConnectionString))
                {
                    con.Open();
                    SqlCommand command = new SqlCommand(text, con);
                    await command.ExecuteNonQueryAsync();
                }
            }

            await this.ResultSet.ToListAsync();
        }
    }
}
