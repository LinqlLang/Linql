import { LinqlSearch } from "./LinqlSearch";
import { ALinqlContext, LinqlSearchConstructor } from "./ALinqlSearch";
import { LinqlContext } from "./LinqlContext";
import { TestFileLoader } from "./test/TestfileLoader";
import "./Extensions/Array";

class DataModel
{
    Integer: number = 1;
    Boolean: boolean = false;
    OneToOne!: DataModel;
}

class TestClass
{
    debugMode = false;
    context: ALinqlContext = new LinqlContext(LinqlSearch as any as LinqlSearchConstructor<any>, { this: this });
    testFiles = new TestFileLoader("Smoke");

    //#region TestVariables
    test: boolean = false;
    complex: DataModel = new DataModel();
    integers = [1, 2, 3];
    //#endregion

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

    async ComplexBoolean()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => this.complex.Boolean);
        await this._ExecuteTest("ComplexBoolean", newSearch);
    }

    async ThreeBooleans()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => r.Boolean && r.Boolean && r.Boolean);
        await this._ExecuteTest("ThreeBooleans", newSearch);
    }

    async ListInt()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => this.integers.Contains(r.Integer));
        await this._ExecuteTest("ListInt", newSearch);
    }


    private async _ExecuteTest(TestName: string, newSearch: LinqlSearch<any>)
    {
        const json = newSearch.toJson();
        const compare = await this.testFiles.GetFile(TestName);

        if (this.debugMode)
        {
            console.log(json);
            console.log(compare);
            debugger;
        }
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

    const functionFilter: string = "";

    if (functionFilter !== "")
    {
        functions = functions.filter(r => r === functionFilter);
        testClass.debugMode = true;
    }

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