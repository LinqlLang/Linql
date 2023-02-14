import { LinqlSearch } from "./LinqlSearch";
import { ALinqlContext, LinqlSearchConstructor } from "./ALinqlSearch";
import { LinqlContext } from "./LinqlContext";

class DataModel
{
    Number: number = 1;
}


describe('LinqlSearch', () =>
{
    const contextArgs: any = {} as any;
    const test = { this: contextArgs };
    let context: ALinqlContext = new LinqlContext(LinqlSearch as any as LinqlSearchConstructor<any>, { this: contextArgs });

    it('LinqlConstant', () =>
    {
        const search = context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => true);
    });
});