# Linql

`Linql` is a next generation graph api library.  It's primary goal is to provide language-native graph api integration to achieve consistency and scale across the web. 

These repos focus on Python implementations.  

[Linql Overview](../README.md)

## Example Usage

```python
from linql_client.LinqlContext import LinqlContext
from linql_client.LinqlSearch import LinqlSearch
...
context = CustomLinqlContext(LinqlSearch, "localhost:7113")
..
search: LinqlSearch[State] = context.Set(State)
statesICareAbout = ["al", "ma"]
await search.Where(lambda r => r in map(lambda t: t.upper(), statesICareAbout)).ToListAsync();
```

## Current Support

| Environment               | ORM        | Notes                                                           | Example                     |
| ------------------------- | ---------- | --------------------------------------------------------------- | --------------------------- |
| [Client](./linql-client/) | N/A        | Need to test all str and list functions.  Base support working. | [link](./example/)          |
| Server                    |            |                                                                 | [link](./examples/angular/) |
|                           | SqlAlchemy | Planned                                                         |
|                           | Django     | Planned                                                         |


dev stuff: 

[packaging docs](https://packaging.python.org/en/latest/tutorials/packaging-projects/)
[testing docs](https://docs.python.org/3/library/test.html)

Many thanks to [aprimc](https://github.com/aprimc) for showing me how to parse the bytecode with his project [from_lambda](https://github.com/aprimc/from_lambda/blob/master/README.md)

hatch has no mono repo support.  Really? 