import { ALinqlContext, ALinqlSearch } from "./ALinqlSearch";
import { LinqlContext } from "./LinqlContext"
export class LinqlSearch<T> extends ALinqlSearch<T>
{
    constructor(Type: (string | (new () => T)), ArgumentContext: {} = {}, Context: ALinqlContext = new LinqlContext(LinqlSearch<any>))
    {
        super(Type, ArgumentContext, Context);
    }
}