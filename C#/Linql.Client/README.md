# Linql.Client

A C# Client for the Linql Language.  Allows you to use your api as if it were an IQueryable. 

```cs
LinqlSearch<DataModel> search = Context.Set<DataModel>();
string output = await search.Where(r => r.Boolean && r.OneToOne.Boolean).ToListAsync();
```

## Installation

```powershell
dotnet add package Linql.Client
```

## Basic Usage

Create a LinqlContext: 

```cs
LinqlContext Context = new LinqlContext("https://localhost:8080");
```

Start a query: 

```cs
LinqlSearch<MyModel> basicSearch = Context.Set<MyModel>();
LinqlSearch<MyModel> searchOne = basicSearch.Where(r => r.Integers.Contains(1));
LinqlSearch<MyModel> searchTwo = basicSearch.Where(r => r.Integers.Contains(2));

var results = await Task.WhenAll(
    searchOne.ToListAsync(),
    searchTwo.TwoListASync()
)
```