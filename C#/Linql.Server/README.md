# Linql.Server

A C# Server for the Linql Language.  Allows your api to be used as if it were an IQueryable. 

```cs
//Some LinqlSearch string
string json = "SomeLinqlSearchJson";

//Set the assemblies that are valid inside the LinqlContext
HashSet<Assembly> assemblies = new HashSet<Assembly>()
{
    typeof(Boolean).Assembly,
    typeof(Enumerable).Assembly,
    typeof(Queryable).Assembly
};

//Create a LinqlCompiler with the assemblies we care about
LinqlCompiler Compiler = new LinqlCompiler(assemblies);

//Turn the json into a generic LinqlSearch
LinqlSearch? search = JsonSerializer.Deserialize<LinqlSearch>(json);


//Execute the LinqlSearch either with a concrete type, or generically
IEnumerable<DataModel> typedData = this.Compiler.Execute<IEnumerable<DataModel>>(search, this.Data);

object genericData = this.Compiler.Execute(search, this.Data);

```

## Installation

```powershell
dotnet add package Linql.Server
```

## WebApi Usage

#### **`Program.cs`**
```cs
var builder = WebApplication.CreateBuilder(args);
...
builder.Services.AddSingleton<LinqlCompiler, CustomLinqlCompiler>();

```


#### **`CustomLinqlCompiler.cs`**
```cs
 public class CustomLinqlCompiler : LinqlCompiler
{
    public CustomLinqlCompiler() : base() 
    { 
        //Loads .Net System types, as well as NetTopologySuite and Linq Assemblies
        this.ValidAssemblies = new HashSet<Assembly>()
        {
            typeof(Boolean).Assembly,
            typeof(Enumerable).Assembly,
            typeof(Queryable).Assembly,
            typeof(Geometry).Assembly,
            typeof(State).Assembly,
        };

        //Add deserializer to Linql deserializer chain
        var geoJsonConverterFactory = new GeoJsonConverterFactory();
        this.JsonOptions.Converters.Add(geoJsonConverterFactory);
    }
}

```


#### **`StateController.cs`**
```cs
//Example Controller that returns StateData
[ApiController]
[Route("[controller]")]
public class StateController : ControllerBase
{
    private readonly ILogger<StateController> _logger;

    protected DataService DataService { get; set; }

    protected LinqlCompiler Compiler { get; set; }

    public StateController(ILogger<StateController> logger, DataService DataService, LinqlCompiler Compiler)
    {
        _logger = logger;
        this.DataService = DataService;
        this.Compiler = Compiler;

    }

    [HttpPost]
    public object Linql(LinqlSearch Search)
    {
        object result = this.Compiler.Execute(Search, this.DataService.StateData.AsQueryable());
        return result;
    }

    //A Batching Method Example.
    [HttpPost("/Batch")]
    public List<object> Batch(List<LinqlSearch> Searches)
    {
        ConcurrentDictionary<long, object> results = new ConcurrentDictionary<long, object>();
        Parallel.ForEach(Searches, async (search, options, index) =>
        {
            LinqlCompiler compiler = new CustomLinqlCompiler();
            object result = await compiler.ExecuteAsync(search, this.DataService.StateData.AsQueryable());
            results.GetOrAdd(index, (key) => result);
        });

        return results.OrderBy(r => r.Key).Select(r => r.Value).ToList();
    }
}

```
### Full Example

Checkout our full example [here](../Examples/WebApiExample/).

### EntityFramework 6 Support

Linql is compatible with EntityFramework 6.  There are tests [here](../Test/Linql.Sever.EF6.Test/).

### EntityFramework Core Support

Linql should be compatible with EntityFramework Core as well.  I do not have tests for this yet, but will be working on them soon.  Conceptually, there should be no issue.  

## Development 

- Visual Studio 2022 
- .Net 7 (requires Visual Studio Preview at the time of this writing.  Can change this to .Net 6 without issue)

## Testing 

Unit tests are located [here](../Test/Linql.Server.Test/).