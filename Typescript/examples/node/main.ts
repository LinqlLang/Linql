
import { LinqlContext, LinqlSearch } from "linql.client";
import { State } from "../DataModel";

class LinqlNodeExample
{
    context = new LinqlContext(LinqlSearch, "https://localhost:7113", { this: this });


    async Run()
    {
        const states = this.context.Set<State>(State)
        debugger;
        const firstState = await states.FirstOrDefaultAsync();
        console.log(firstState.State_Name);
        debugger;
    }

}

const example = new LinqlNodeExample();
await example.Run();