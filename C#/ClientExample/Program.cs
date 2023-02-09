// See https://aka.ms/new-console-template for more information
using Linql.Client;
using NetTopologySuite.Geometries;

Console.WriteLine("Hello, World!");

LinqlContext context = new CustomLinqlContext("https://localhost:7113");

LinqlSearch<State> states = context.Set<State>();

State firstState = await states.FirstOrDefaultAsync();

Console.WriteLine(firstState.State_Name);

List<State> foundStates = await states.Where(r => r.State_Name.ToLower().Contains("ma")).ToListAsync();

foundStates.ForEach(r =>
{
    Console.WriteLine($"{r.State_Name}");
});

Point point = new Point(new Coordinate(-77.036530, 38.897675));

State stateContainsCordinate = await states.FirstOrDefaultAsync(r => r.Geometry.Contains(point));

Console.WriteLine($"The white house is in: {stateContainsCordinate.State_Name}");

List<string> codesICareAbout = new List<string>() { "al", "ma" };

List<State> statesICareAbout = await states.Where(r => codesICareAbout.Select(t => t.ToUpper()).Contains(r.State_Code)).ToListAsync();

statesICareAbout.ForEach(r =>
{
    Console.WriteLine($"{r.State_Code}");
});

List<State> stateWithIdsBetween = await states.Where(r => r.FID > 2 && r.FID <= 30).ToListAsync();
stateWithIdsBetween.ForEach(r =>
{
    Console.WriteLine($"{r.State_Code}");
});

List<State> statePopulationQuery = await states.Where(r => r.Data.Any(s => s.Variable == "Population" && s.Value < 3234812 && s.DateOfRecording.Year == 2022)).ToListAsync();

List<StateData> stateData = await states.SelectMany(r => r.Data).Where(s => s.Variable == "Population" && s.Value < 3234812 && s.DateOfRecording.Year == 2022).ToListAsync();


statePopulationQuery.ForEach(r =>
{
    Console.WriteLine($"{r.State_Code}");
});

stateData.ForEach(r =>
{
    Console.WriteLine($"{r.Value}");
});