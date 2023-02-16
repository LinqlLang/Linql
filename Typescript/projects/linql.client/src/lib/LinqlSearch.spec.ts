import { LinqlSearch } from "./LinqlSearch";
import { ALinqlContext, LinqlSearchConstructor } from "./ALinqlSearch";
import { LinqlContext } from "./LinqlContext";
import { TestFileLoader } from "./test/TestfileLoader";
import "./Extensions/Array";
import { INullable } from "./INullable";
import { LinqlObject, LinqlType } from "linql.core";

class DataModel
{
    Integer!: number;
    Boolean: boolean = false;
    String!: string;
    Decimal!: number;
    Long!: bigint;
    DateTime!: Date;
    Guid!: string;
    ListInteger!: Array<number>;
    ListString!: Array<string>;
    ListRecursive!: Array<DataModel>;
    OneToOne!: DataModel;
    OneToOneNullable!: NullableModel;
    ListNullableModel!: Array<NullableModel>;
}

class NullableModel
{
    Integer?: number;
    Boolean?: boolean;
    String?: string;
    Decimal?: number;
    Long?: bigint;
    DateTime?: Date;
    Guid?: string;
    ListInteger?: Array<number>;
    ListString?: Array<string>;
    ListRecursive?: Array<DataModel>;
    OneToOne?: DataModel;
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
    objectTest: LinqlObject<DataModel>;
    //#endregion

    constructor()
    {
        const testData = new DataModel();
        const anyCast = testData as any;
        anyCast.Boolean = undefined;
        anyCast.String = "";
        anyCast.ListInteger = new Array<any>();
        anyCast.ListString = new Array<any>();
        anyCast.ListRecusrive = new Array<any>();
        anyCast.ListNullableModel = new Array<any>();

        this.objectTest = new LinqlObject<DataModel>(testData);
    }

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

    async ListIntFromProperty()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => r.ListInteger.Contains(1));
        await this._ExecuteTest("ListIntFromProperty", newSearch);
    }

    async InnerLambda()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => r.ListInteger.Any(s => s === 1));
        await this._ExecuteTest("InnerLambda", newSearch);
    }

    async NullableHasValue()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => r.OneToOneNullable.Integer !== undefined);
        await this._ExecuteTest("NullableHasValue", newSearch);
    }

    async NullableHasValueReversed()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => undefined !== r.OneToOneNullable.Integer);
        await this._ExecuteTest("NullableHasValue", newSearch);
    }

    async NullableValue()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => r.OneToOneNullable.Integer !== undefined && (r.OneToOneNullable.Integer as any as INullable<number>).Value === 1);
        await this._ExecuteTest("NullableValue", newSearch);
    }

    async LinqlObject()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.filter(r => this.objectTest.Value.Integer == r.Integer);
        await this._ExecuteTest("LinqlObject", newSearch);
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