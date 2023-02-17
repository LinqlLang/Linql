"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const linql_client_1 = require("linql.client");
class LinqlNodeExample {
    constructor() {
        this.context = new linql_client_1.LinqlContext(linql_client_1.LinqlSearch, "https://localhost:7113", { this: this });
        // async Run()
        // {
        //     const states = this.context.Set<State>(State)
        //     debugger;
        //     const firstState = await states.FirstOrDefaultAsync();
        //     console.log(firstState.State_Name);
        //     debugger;
        // }
    }
}
// const example = new LinqlNodeExample();
// example.Run();
//# sourceMappingURL=main.js.map