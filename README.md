# Linql

Linql is a next generation graph api library.  It's primary goal is to provide language-native graph api integration to achieve consistency and scale across the web. 

[Read the white paper](./WhitePaper.md)  

## Client Examples

#### **`C#`**
```cs 
List<string> codesICareAbout = new List<string>() { "al", "ma" };

List<State> statesICareAbout = await states
.Where(r => codesICareAbout.Select(t => t.ToUpper()).Contains(r.State_Code))
.ToListAsync();
```

#### **`Typescript`**
```typescript
this.CodesICareAbout = ["al", "ma"];
...
const statesICareAbout: Array<State> = search
.Where(r => this.CodesICareAbout.Select(t => t.ToUpper()).Contains(r.State_Code!))
.ToListAsync();

```

#### **`Python`**
```python
codesICareAbout = ["al", "ma"]
result = list(
    filter(lambda r: 
    r in 
        [t.upper() for t in codesICareAbout], 
    states
    )
)
```

#### **`Java`**
```java
List<String> codesICareAbout = Arrays.asList("al", "ma");

List<State> statesICareAbout = states
.stream()
.filter(r => codesICareAbout.stream().map(t => t.toUpperCase()).collect(Collectors.toList()).contains(r))
.collect(Collectors.toList());
```

## Language Support

| Language                             | Environment | Client                                                 | Server                       | Notes                                                                                                                                         |
| ------------------------------------ | ----------- | ------------------------------------------------------ | ---------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------- |
| C#                                   |             | [Full](./C%23/Linql.Client/)                           | [Full](./C%23/Linql.Server/) |
| [Javascript](./Typescript/README.md) |             |                                                        |                              | Uses Linq emulator                                                                                                                            |
| -                                    | Node        | [Full](./Typescript/projects/linql.client-node-fetch/) | Not Started                  | [linql.client-fetch](./Typescript/projects/linql.client-fetch/) and [linql.client-node-fetch](./Typescript/projects/linql.client-node-fetch/) |
| -                                    | Angular     | [Full](./Typescript/projects/linql.client-angular/)    | n/a                          | Has native framework wrapper                                                                                                                  |
| -                                    | React       | [Full](./Typescript/projects/linql.client-fetch/)      | n/a                          | Needs native framework wr apper                                                                                                               |
| -                                    | Vanilla     | [Full](./Typescript/projects/linql.client-fetch/)      | n/a                          |
| Python                               |             | Not Started                                            | Not Started                  | May not be possible                                                                                                                           |
| Java                                 |             | Not Started                                            | Not Started                  |

## Developer Support

If Linql resonates with you, please consider supporting the project.  As of now, I am the only developer and tough times have come for me, so I could use whatever support anyone can give.  

### Ethereum

```
0x50373B51Cb601827CcC1Dc5472251031d2fdBF89
```

### Please Hire Me

[LinkedIn](https://www.linkedin.com/in/kris-sodroski-60001480/)

[sodroski@bu.edu](mailto:sodroski@bu.edu)

## Acknowledgements 

Big thanks to Jeff Bender, [Manjot Chahal](https://www.linkedin.com/in/manjot-chahal-96740198/) and [Sushmita Chaudhari](https://www.linkedin.com/in/sushmitachaudhari/) for exploring some of these concepts with me over the years.  
