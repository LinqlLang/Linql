# Linql Client Example

An example Linql Client that interfaces with our [webapi example](../WebApiExample/).

## Getting started 

Start the webapi example.  Then run: 

```powershell
dotnet run
```

Add debuggers as you please to see the data returned and mess around with the api!

## Notes

This example creates a [CustomLinqlContext](./CustomLinqlContext.cs) in order to override the default endpoint.  To read more about endpoint defaults, see [Linql.Client](../../Linql.Client/).