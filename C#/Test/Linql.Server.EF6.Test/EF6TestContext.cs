using Linql.Server.EF6.Test.DataModel;
using System;
using System.Collections;
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

        public DbSet<Variable> Variables { get; set; }

        public DbSet<BuildingData> BuildingData { get; set; }

        public EF6TestContext(string ConnectionString = DataModelConstants.ConnectionString) : base(ConnectionString)
        {
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SqlClient.SqlClientFactory.Instance);
            ////for Connection
            var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            DbConnection connection = factory.CreateConnection();

        }

        public async Task Init(bool Reset = true)
        {
            if (Reset == true)
            {
                try
                {
                    await this.RunScript("DbSetup.sql", true);
                }
                catch(Exception ex)
                {

                }

            }

            await this.Buildings.ToListAsync();

            if (Reset == true)
            {
                await this.RunScript("TableSetup.sql");
                //await this.SeedDataInMemory();
                await this.SeedDataFromFiles();

            }
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

        private async Task SeedDataFromFiles()
        {
            string variablesText = await File.ReadAllTextAsync("./TestData/Variable.sql");
            string buildingsText = await File.ReadAllTextAsync("./TestData/Building.sql");
            string dataText = await File.ReadAllTextAsync("./TestData/BuildingData.sql");

            await this.Database.ExecuteSqlCommandAsync(variablesText);
            await this.Database.ExecuteSqlCommandAsync(buildingsText);
            await this.Database.ExecuteSqlCommandAsync(dataText);
        }

        private async Task SeedDataInMemory()
        {
            List<string> variables = new List<string>() { "Rent per Square Foot", "Vacant Percent", "Occupied Percent" };
            int startYear = 2000;
            int endYear = DateTime.Now.Year;
            Random random = new Random();
            string latLongFile = await File.ReadAllTextAsync("./TestData/LatLongs.json");

            Dictionary<string, Variable> variableMap = variables.Select(r => new Variable() { VariableName = r }).ToDictionary(r => r.VariableName);

            List<LatLong> latLongs = JsonSerializer.Deserialize<List<LatLong>>(latLongFile);

            List<Building> buildings = latLongs.Select(r =>
            {
                Building building = new Building();
                building.Latitude = r.Latitude;
                building.Longitude = r.Longitude;
                building.BuildingName = $"Test Building {latLongs.IndexOf(r) + 1}";
                building.Data = new List<BuildingData>();

                Enumerable.Range(startYear, endYear - startYear).ToList().ForEach(r =>
                {
                    int rentValue = random.Next(15, 100);
                    double vacantPercent = random.NextDouble();
                    double occupied = 1 - vacantPercent;

                    BuildingData rent = new BuildingData(r, variableMap["Rent per Square Foot"], rentValue);
                    BuildingData vacant = new BuildingData(r, variableMap["Vacant Percent"], vacantPercent);
                    BuildingData occupiedPer = new BuildingData(r, variableMap["Occupied Percent"], occupied);
                    building.Data.Add(rent);
                    building.Data.Add(vacant);
                    building.Data.Add(occupiedPer);

                });

                return building;
            }).ToList();

            this.Buildings.AddRange(buildings);
            await this.SaveChangesAsync();
        }
    }
}
