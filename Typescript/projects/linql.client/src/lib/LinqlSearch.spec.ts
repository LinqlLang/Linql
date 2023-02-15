import { LinqlSearch } from "./LinqlSearch";
import { ALinqlContext, LinqlSearchConstructor } from "./ALinqlSearch";
import { LinqlContext } from "./LinqlContext";
import { TestFileLoader } from "./test/TestfileLoader";

class DataModel
{
    Number: number = 1;
    Boolean: boolean = true;
}


describe('LinqlSearch', () =>
{
    const testFiles = new TestFileLoader("Smoke");
    const contextArgs: any = {} as any;
    const test = { this: contextArgs };
    let context: ALinqlContext = new LinqlContext(LinqlSearch as any as LinqlSearchConstructor<any>, { this: contextArgs });

    const emptySearch = 'EmptySearch';
    it(emptySearch, async () =>
    {
        const search = context.Set<DataModel>(DataModel);
        const newSearch = search;
        const json = newSearch.toJson();
        const compare = await testFiles.GetFile(emptySearch);
        expect(json).toEqual(compare);
    });

    const simpleConstant = 'SimpleConstant';
    it(simpleConstant, async () =>
    {
        const search = context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => true);
        const json = newSearch.toJson();
        const compare = await testFiles.GetFile(simpleConstant);
        expect(json).toEqual(compare);
    });

    const simpleBooleanProperty = 'SimpleBooleanProperty';
    it(simpleBooleanProperty, async () =>
    {
        const search = context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => r.Boolean);
        const json = newSearch.toJson();
        const compare = await testFiles.GetFile(simpleBooleanProperty);
        expect(json).toEqual(compare);
    });

    const booleanNegate = 'BooleanNegate';
    it(booleanNegate, async () =>
    {
        const search = context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => !r.Boolean);
        const json = newSearch.toJson();
        const compare = await testFiles.GetFile(booleanNegate);
        console.log(json);
        console.log(compare);
        expect(json).toEqual(compare);
    });
});