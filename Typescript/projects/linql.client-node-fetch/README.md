# Linql.Client-Node-Fetch

A linql client implementation that uses node-fetch. 

[Linql Overview](../../../../README.md)

[Node Example](../../examples/node/)

## Getting Started 

### Installation

```bash
npm i linql.client-fetch
```

Create a context, and then start using.

```typescript
const context = new CustomLinqlContext(LinqlSearch, "https://localhost:7113", { this: this });
const states = this.context.Set<State>(State);
const firstState = await states.FirstOrDefaultAsync();
```
