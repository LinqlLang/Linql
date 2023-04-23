using Linql.Server.EF6.Test.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Linql.Server.EF6.Test
{
    public class EF6TestContext : DbContext
    {
        public DbSet<Building> Buildings { get; set; }

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
                await this.RunScript("DbSetup.sql", true);
                
            }

            await this.Buildings.ToListAsync();
            await this.RunScript("TableSetup.sql");
            await this.SeedData();
        }

        private async Task RunScript(string ScriptName, bool UseMaster = false)
        {
            string connection = UseMaster ? DataModelConstants.MasterConnectionString : DataModelConstants.ConnectionString;
            string script = Path.Join("Scripts", ScriptName);

            string text = await File.ReadAllTextAsync(script);
            using (SqlConnection con = new SqlConnection(connection))
            {
                con.Open();
                SqlCommand command = new SqlCommand(text, con);
                await command.ExecuteNonQueryAsync();
            }
        }

        private async Task SeedData()
        {
            string latLongFile = await File.ReadAllTextAsync("./TestData/LatLongs.json");

            List<LatLong> latLongs = JsonSerializer.Deserialize<List<LatLong>>(latLongFile);

            List<Building> buildings = latLongs.Select(r =>
            {
                Building building = new Building();
                building.Latitude = r.Latitude;
                building.Longitude = r.Longitude;
                building.BuildingName = $"Test Building {latLongs.IndexOf(r) + 1}";
                return building;
            }).ToList();

            this.Buildings.AddRange(buildings);
            await this.SaveChangesAsync();
        }
    }
}
