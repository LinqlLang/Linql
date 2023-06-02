# Linql.Client-Node-Fetch

A linql client implementation that uses node-fetch. 

[Linql Typescript Overview]("../../../../README.md)

[Node Example](../../examples/node/)

## Getting Started 

### Installation

```bash
npm i linql.client-node-fetch
```

Create a context, and then start using.

```typescript
import { NodeFetchLinqlContext } from "linql.client-fetch";
import { LinqlSearch } from "linql.client";

const context = new NodeFetchLinqlContext(LinqlSearch, "https://localhost:7113", { this: this });
const states = this.context.Set<State>(State);
const firstState = await states.FirstOrDefaultAsync();
```
