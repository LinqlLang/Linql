import { LinqlSearch } from "./LinqlSearch";
import { ALinqlContext, LinqlSearchConstructor } from "./ALinqlSearch";
import { LinqlContext } from "./LinqlContext";
import { TestFileLoader } from "./test/TestfileLoader";

class DataModel
{
    Number: number = 1;
}


describe('LinqlSearch', () =>
{
    const testFiles = new TestFileLoader("Smoke");
    const contextArgs: any = {} as any;
    const test = { this: contextArgs };
    let context: ALinqlContext = new LinqlContext(LinqlSearch as any as LinqlSearchConstructor<any>, { this: contextArgs });

    const simpleBoolean = 'SimpleBooleanFalse';
    it(simpleBoolean, async () =>
    {
        const search = context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => false);
        const json = newSearch.toJson();
        const compare = await testFiles.GetFile(simpleBoolean);
        debugger;
        expect(json).toEqual(compare);
    });
});