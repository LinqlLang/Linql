import { LinqlSearch } from "./LinqlSearch";
import { ALinqlContext, LinqlSearchConstructor } from "./ALinqlSearch";
import { LinqlContext } from "./LinqlContext";
import { TestFileLoader } from "./test/TestfileLoader";

class DataModel
{
    Number: number = 1;
    Boolean: boolean = true;
    OneToOne!: DataModel;
}

class TestClass
{
    test: boolean = false;
    context: ALinqlContext = new LinqlContext(LinqlSearch as any as LinqlSearchConstructor<any>, { this: this });
    testFiles = new TestFileLoader("Smoke");

    async EmptySearch()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search;
        await this._ExecuteTest("EmptySearch", newSearch);
    }

    async SimpleConstant()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => true);
        await this._ExecuteTest("SimpleConstant", newSearch);
    }
    async SimpleBooleanProperty()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => r.Boolean);
        await this._ExecuteTest("SimpleBooleanProperty", newSearch);
    }

    async BooleanNegate()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => !r.Boolean);
        await this._ExecuteTest("BooleanNegate", newSearch);
    }

    async SimpleBooleanPropertyChaining()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => r.OneToOne.Boolean);
        await this._ExecuteTest("SimpleBooleanPropertyChaining", newSearch);
    }

    async SimpleBooleanPropertyEqualsSwap()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => false === r.Boolean);
        await this._ExecuteTest("SimpleBooleanPropertyEqualsSwap", newSearch);
    }

    async BooleanVar()
    {
        this.test = false;
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => false === this.test);
        await this._ExecuteTest("BooleanVar", newSearch);
    }

    private async _ExecuteTest(TestName: string, newSearch: LinqlSearch<any>)
    {
        const json = newSearch.toJson();
        const compare = await this.testFiles.GetFile(TestName);
        expect(json).toEqual(compare);
    }

}


describe('LinqlSearch', () =>
{
    const contextArgs: any = {} as any;
    const test = { this: contextArgs };
    let context: ALinqlContext = new LinqlContext(LinqlSearch as any as LinqlSearchConstructor<any>, { this: contextArgs });
    const testClass = new TestClass();
    let functions = Object.getOwnPropertyNames(testClass.constructor.prototype)
        .filter(r => r !== "constructor" && r.indexOf("_") == -1);

    //functions = functions.filter(r => r === "BooleanVar");

    for (let key of functions)
    {
        const any = testClass as any;
        const value = any[key];
        if (typeof value === "function")
        {
            it(key, async () =>
            {
                await any[key]();
            });
        }
    }
});