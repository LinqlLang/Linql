# Linql WebApi Example

An example WebApi that provides a single linql endpoint, and a batching endpoint.  

## Getting started 

```powershell
dotnet run
```

Then navigate to our [client example](../ClientExample/) and run that example as well. 

## Notes

This example creates a [CustomLinqlCompiler](./CustomLinqlCompiler.cs) in order to encapsulate loading extension dlls as well as handle GeoJson converter support.  To read more about custom configuration, see [Linql.Server](../../Linql.Server/).