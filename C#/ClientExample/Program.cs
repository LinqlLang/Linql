// See https://aka.ms/new-console-template for more information
using Linql.Client;
using NetTopologySuite.Geometries;

Console.WriteLine("Hello, World!");

LinqlContext context = new CustomLinqlContext("https://localhost:7113");

LinqlSearch<State> state = context.Set<State>();

State firstState = await state.FirstOrDefaultAsync();

Console.WriteLine(firstState.State_Name);

List<State> foundStates = await state.Where(r => r.State_Name.ToLower().Contains("ma")).ToListAsync();

foundStates.ForEach(r =>
{
    Console.WriteLine($"{r.State_Name}");
});

Point point = new Point(new Coordinate(-77.036530, 38.897675));
LinqlObject<Point> linqlPoint = new LinqlObject<Point>(point);

State stateContainsCordinate = await state.FirstOrDefaultAsync(r => r.Geometry.Contains(linqlPoint.TypedValue));

Console.WriteLine($"{stateContainsCordinate.State_Name}");