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

[Linql Overview](../../README.md)

## Installation

```powershell
dotnet add package Linql.Server -v 1.0.0-alpha1
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
    public async Task<object> Linql(LinqlSearch Search)
    {
        object result = await this.Compiler.ExecuteAsync(Search, this.DataService.StateData.AsQueryable());
        return result;
    }

    //A Batching Method Example.
    [HttpPost("/Batch")]
    public async Task<List<object>> Batch(List<LinqlSearch> Searches)
    {
        List<Task<object>> tasks = Searches.Select(r =>
        {
            LinqlCompiler compiler = new CustomLinqlCompiler();
            Task<object> result = compiler.ExecuteAsync(r, this.DataService.StateData.AsQueryable());
            return result;
        }).ToList();

        var taskResults = await Task.WhenAll(tasks);
        List<object> results = taskResults.ToList();

        return results;
    }
}

```
### Full Example

Checkout our full example [here](../Examples/WebApiExample/).

## Library Support
### EntityFramework 6

Linql is compatible with EntityFramework 6.  There are tests [here](../Test/Linql.Server.EF6.Test/).

### EntityFramework Core

Linql should be compatible with EntityFramework Core as well.  I do not have tests for this yet, but will be working on them soon.  Conceptually, there should be no issue.  

## Advanced Concepts
### Batching

In our full example, as well as in the above sample, you can see the "Batching" technique.  Batching can significantly reduce the overhead of your application, by bundling requests together into one Http Request.  The server then multiplexes the results.

Batching is mostly always desired, but requires that the server implement a Generic Controller interface.

### Generic Controllers

It's obviously advantageous to use Linql over your entire data model.  To do so, a generic controller interface is preferrable.  An example of this will be provided at a later date.

### Information Extraction

Find functionality built into the library allows application developers to search for patterns within linql searches in order to implement more advanced logic.  

Find has two [options](../Linql.Core/LinqlFindOption.cs), Exact and Similar.  

Exact will only return results if the statements exactly match, while Similar will do its best to try and find things that relatively match.  Either method will recursively try to find the expression anywhere in the expression tree.  

Only the first expression in the comparison search is used for matching.  

A common usecase is to limit the max number of objects returned for types.  

#### **`EnforceLimit.cs`**
```cs 
LinqlSearch someLinqlSearch;

//Set some limit
int limit = 500;

//Create a search that will look for the Take method
IQueryable<DataModel> takeSearch = new LinqlSearch<DataModel>();
takeSearch = takeSearch.Take(500);
LinqlSearch takeCompiled = takeSearch.ToLinqlSearch();

//Look for an expression that is similar ot the take method
List<LinqlExpression> findResults = someLinqlSearch.Find(takeCompiled, LinqlFindOption.Similar);

//If not method is found, splice the take expression into the user supplied search
if(findResults.Count() == 0)
{
    takeSearch.Expression.LastOrDefault()?.Next = takeSearch.Expressions.FirstOrDefault();
}
else
{
    //Go through the results, compare the limits, and override if they exceeded the limit.  
    //Realisitcally, we'd only expect 1 item in findResults, but we loop through for consistency.
    findResults.ForEach(r =>
    {
        if (r is LinqlFunction fun)
        {
            fun.Arguments.ForEach(arg =>
            {
                if (arg is LinqlConstant constant && constant.Value > limit)
                {
                    constant.Value = limit;
                }
            });
        }
    });
}
```

### Compiler Hooks

The first level of Linql searches provide a before and after lifecycle hook method.  These can be used to provide advanced logic before and after expressions are executed.  

In example, suppose I want to disallow the selection of a particular property.  I can accomplish this like so: 

#### **`DisablePropertySelectionHook.cs`**
```cs 
//Create the linql hook
LinqlCompilerHook disableSelectHook = new LinqlBeforeExecutionHook((fun, input, inputType, method, args) =>
{
    MemberInfo prop = typeof(DataModel).GetMember(nameof(DataModel.Decimal)).FirstOrDefault();

    if(fun.FunctionName == nameof(Queryable.Select))
    {
        LambdaExpression lam = args.Where(r => r is LambdaExpression).Cast<LambdaExpression>().FirstOrDefault();

        if(lam != null && lam.Body is MemberExpression member && member.Member == prop)
        {
            throw new Exception($"Not allowed to select into property {nameof(DataModel.Decimal)} on type {nameof(DataModel)}");
        }
    }

    return Task.CompletedTask;
});

...

//Add the hook into the linql compiler.  There is a corresponding RemoveHook method as well. 
this.Compiler.AddHook(disableSelectHook);

```

#### **`AfterExecutionHookExample.cs`**
```cs 
//Example prototype of after execution hooks
LinqlCompilerHook afterExecutionHook = new LinqlAfterExecutionHook((fun, input, inputType, method, args, object) =>
{
   ...
});

...

//Add the hook into the linql compiler.  There is a corresponding RemoveHook method as well. 
this.Compiler.AddHook(afterExecutionHook);

```
### ORM Row Level Permissioning  

If using an ORM, and row level access control can be achieved by backing the data model generated for a user to point to views that enforce row level permissions.

In conjunction with Linql's Find functionality and lifecycle hooks, this strategy provides an "open" experience for consumers while allowing developers to enforce data access controls at scale.   

## Development 

- Visual Studio 2022 17.4 
- .Net 7 to run tests and example projects.

## Future Enhancements 

- Better Find/Hook support 
- Allow linql queries to continue after materialization 
- Support multi-line statements
- Support multiple LinqlSearches in the same context, and allow interaction between them (Join)
- Anonymous Types (which would then allow specific select statements)
- Performance Tests
- More test cases

## Testing 

Unit tests are located [here](../Test/Linql.Server.Test/).