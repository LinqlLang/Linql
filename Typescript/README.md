# Linql

`Linql` is a next generation graph api library.  It's primary goal is to provide language-native graph api integration to achieve consistency and scale across the web. 

These repos focus on Typescript/Javascript implementations.  

[Linql Overview](../README.md)

## Example Usage

```typescript
import { LinqlSearch, LinqlContext } from 'linql.client';
...
const context = new LinqlContext(LinqlSearch, "https://localhost:7113", { this: this });
const search = this.customContext.Set<State>(State, { this: this });
search.Where(r => r.State_Code!.Contains("A")).ToListAsyncSearch();
```


## Current Support

| Environment | Client                                      | Server      | Notes                                                                                                                   | Example                     |
| ----------- | ------------------------------------------- | ----------- | ----------------------------------------------------------------------------------------------------------------------- | --------------------------- |
| Node        | [Full](./projects/linql.client-node-fetch/) | Not Started | [linql.client-fetch](./projects/linql.client-fetch/) and [linql.client-node-fetch](./projects/linql.client-node-fetch/) | [link](./examples/node/)    |
| Angular     | [Full](./projects/linql.client-angular/)    | n/a         | Has native framework wrapper                                                                                            | [link](./examples/angular/) |
| React       | [Full](./projects/linql.client-fetch/)      | n/a         | Needs native framework wr apper                                                                                         |
| Vanilla     | [Full](./projects/linql.client-fetch/)      | n/a         |                                                                                                                         |

## Network Implementation

| Package                 | Implementation       | Import                                                           |
| ----------------------- | -------------------- | ---------------------------------------------------------------- |
| linql.client            | Native fetch         | `import { LinqlContext } from linql.client;`                     |
| linql.client-angular    | @angular/common/http | `import { LinqlContext } from linql.client-angular;`             |
| linql.client-fetch      | Native Fetch         | `import { FetchLinqlContext } from linql.client-fetch;`          |
| linql.client-node-fetch | node-fetch           | `import { NodeFetchLinqlContext } from linql.client-node-fetch;` |


# Concepts
## TypeName Resolution 

When an `object` is used within a LinqlSearch, the LinqlContext's `GetTypeName` function is used to resolve the name of that object.  By default, `GetTypeName` implementation is as follows: 

- Check for the existance of a static `Type` member on the constructor.  
- If static `Type` does not exist, choose `constructor.name`.

The static `Type` member, or some custom implementation, **is required to support minified code**, as **class names are generally clobbered** during minification.

#### **`ModelExample.ts`**
```typescript
export class State
{
  //Default way to handle code miniification 
  static Type = "State";
  StateID!: number;
  StateCode!: string;
  State_Name!: string;
}
```

## EndPoint Resolution

By default, the generated route is `linql/{GetTypeName(ObjectConstructor)}`.

The `State` object used above would generate `linql/State`, as the static `Type` member is equal to `State`.

## Idempotent

`Linql` is **idempotent**, allowing for LinqlSearches to be built on top of one another.

```typescript
const search = this.LinqlContext.Set<State>(State, { this: this });
const baseQuery = search.Where(r => r.StateCode.Contains("a"));
//branch 2 queries off the base query
const search2 = baseQuery.Where(r => r.State_Name!.ToLower().Contains(this.StateSearch));
const search3 = baseQuery.Where(r => r.State_Name.Contains("b"));
```

## Linq Emulation

`Linql` comes bundled with a naiive implementation of linq, which adds additional methods to `Array` and `String`.

For a full list of available functions: [LinqlSearch](./projects/linql.client/src/lib/ALinqlSearch.ts), [Array](./projects//linql.core/src/lib/Extensions/Array.ts), [String](./projects/linql.core/src/lib/Extensions/String.ts).

```typescript
const inMemoryArray: Array<State> = [...];
const inMemoryMax = intArray.Where(r => r.StateID > 2).Max();

const apiArray = this.LinqlContext.Set<State>(State, { this: this });
const apiMax = await apiArray.Where(r => r.StateID > 2).MaxAsync();
```

## Using Variables

Javascript has no access to the stack at runtime, and therefore it is **impossible to use locally scoped variables inside of a linql search**.  

To circumnavigate this, `Linql` emulates the stack with the *incantation* `{ this: this }`. 

Other variables can be stuck into the stack, but due to code minification, this is not advised. 

```typescript
export class SomeClass
{
    StatesICareAbout: Array<string> = ["ma", "al"];
    ...
    async GetData()
    {
        //Define this inside of the stack with the incantation 
        const search = this.LinqlContext.Set<State>(State, { this: this });
        //Use the variable by accessing it through 'this'
        const results = await search.Where(r => this.StatesICareAbout.Contains(r.StateCode)).ToListAsync();
    }
}
```
## Nullable Types

`Nullable` members are operated on using the `non-null asertion operator (!)`  on `strings` and with `INullable` casting for non-string types.

```typescript
//Ignore nullable string
search.Where(r => r.StringProperty!.Contains("A")).ToListAsync();
//Check for not null and then cast to INullable
const notNull = search.Where(r => r.NullableInteger !== undefined && (r.NullableInteger as any as INullable<number>).Value === 1);
//Get where Is Null
const isNull = search.Where(r => r.NullableInteger === undefined);
```
## Dynamic Queries

`Linql` methods accept both `arrow functions` and `strings` as predicates.  This can allow you to do dynamic generation of queries. 

```typescript
const value = 1;
const method = "<";
search.Where(`r => r.Integer ${method} ${value}`).ToListAsync();
```

## Custom LinqlContext 

Overriding the default `LinqlContext` allows customized endpoint generation, authentication, and `TypeName` resolution.  To do so, simply inherit from the default `LinqlContext` from the linql package that matches your usecase. 

In the below example, we show a `CustomLinqlContext` which overrides the route generation, overrides the `TypeName Resolution`, and adds a `Batch` functionality.

#### **`CustomLinqlContext.ts`**
```typescript
export class CustomLinqlContext extends LinqlContext
{
    override GetRoute(Search: LinqlSearch<any>)
    {
        if (Search.Type.TypeName)
        {
            return Search.Type.TypeName;
        }
        return "";
    }
    
    async Batch<T>(...values: Array<T>): Promise<Array<T>>
    {
        const searches = values.Where(r => (r as Object).constructor === this.LinqlSearchType) as Array<ALinqlSearch<any>>;
        const sanitizedSearches = searches.map(r => this.GetOptimizedSearch(r));
        const results = await lastValueFrom(this.Client.post(`${ this.BaseUrl }Batch`, sanitizedSearches));
        return results as Array<T>;
    }

    override GetTypeName(Type: string | GenericConstructor<any>): string
    {
        if (typeof Type !== "string")
        {
            const anyCast = Type as any;
            const type = anyCast["Type"];

            if (type)
            {
                return type;
            }
        }
        return super.GetTypeName(Type);
    }

}
```

## Batching

The modern web is filled with many small requests.  If you check your devtools at anytime, it's filled with network requests.

To alleviate this issue, `Batch` functionality is strongly recommended, and must be enabled on your [server](../C%23/Linql.Server/README.md).

If your server supports it, you can batch many requests together into a single network request, and the `Linql` server will return all the results of a Batch in a single request. 

The above example shows a naiive implementation of how to implement `Batch` that hits the `/Batch` endpoint of the backend. 

This functionality may be migrated into the core of `Linql` in the future, and include functionality to also accept non-linql requests and zip the results together, as well as include better type definitions that can infer .

```typescript
const searches = [
    search.ToListAsyncSearch(),
    search.Where(r => r.State_Code!.Contains("A")).ToListAsyncSearch(),
    search.Where(r => r.State_Name!.ToLower().Contains(this.StateSearch)),
    search.FirstOrDefaultSearch()
];
const batchResults = await this.customContext.Batch(searches);
```
