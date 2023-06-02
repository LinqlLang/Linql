# Linql.Client-Fetch

A linql client implementation that uses native fetch. 

[Linql Typescript Overview]("../../../../README.md)

## Getting Started 

### Installation

```bash
npm i linql.client-fetch
```

Create a context, and then start using.

```typescript
import { FetchLinqlContext } from "linql.client-fetch";
import { LinqlSearch } from "linql.client";

const context = new FetchLinqlContext(LinqlSearch, "https://localhost:7113", { this: this });
const states = this.context.Set<State>(State);
const firstState = await states.FirstOrDefaultAsync();
```
