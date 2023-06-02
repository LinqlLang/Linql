import { LinqlSearch } from "./LinqlSearch";
import { ALinqlContext, LinqlSearchConstructor } from "./ALinqlSearch";
import { LinqlContext } from "./LinqlContext";
import { TestFileLoader } from "./test/TestfileLoader";
import { INullable } from "../../../linql.core/src/lib/Extensions/INullable";
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
    ListRecusrive!: Array<DataModel>;
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
    context: ALinqlContext = new LinqlContext(LinqlSearch as any as LinqlSearchConstructor<any>, "", { this: this });
    testFiles = new TestFileLoader("Smoke");

    //#region TestVariables
    test: boolean = false;
    complex: DataModel = new DataModel();
    integers = [1, 2, 3];
    objectTest: LinqlObject<DataModel>;
    //#endregion

    constructor()
    {
        const testData = this._CreateDataModel();
        const type = LinqlType.GetLinqlType(testData, this.context);
        this.objectTest = new LinqlObject<DataModel>(testData, type);
    }

    private _CreateDataModel(Integer?: number)
    {
        const testData = new DataModel();
        const anyCast = testData as any;

        if (Integer)
        {
            anyCast.Integer = Integer;
        }
        anyCast.Boolean = undefined;
        anyCast.String = "";
        anyCast.ListInteger = new Array<any>();
        anyCast.ListString = new Array<any>();
        anyCast.ListRecusrive = new Array<any>();
        anyCast.ListNullableModel = new Array<any>();
        return testData;

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
        const newSearch = search.Where(r => true);
        await this._ExecuteTest("SimpleConstant", newSearch);
    }

    async SimpleBooleanProperty()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => r.Boolean);
        await this._ExecuteTest("SimpleBooleanProperty", newSearch);
    }

    async BooleanNegate()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => !r.Boolean);
        await this._ExecuteTest("BooleanNegate", newSearch);
    }

    async SimpleBooleanPropertyChaining()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => r.OneToOne.Boolean);
        await this._ExecuteTest("SimpleBooleanPropertyChaining", newSearch);
    }

    async SimpleBooleanPropertyEqualsSwap()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => false === r.Boolean);
        await this._ExecuteTest("SimpleBooleanPropertyEqualsSwap", newSearch);
    }

    async BooleanVar()
    {
        this.test = false;
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => false === this.test);
        await this._ExecuteTest("BooleanVar", newSearch);
    }

    async ComplexBoolean()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => this.complex.Boolean);
        await this._ExecuteTest("ComplexBoolean", newSearch);
    }

    async ThreeBooleans()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => r.Boolean && r.Boolean && r.Boolean);
        await this._ExecuteTest("ThreeBooleans", newSearch);
    }

    async ListInt()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => this.integers.Contains(r.Integer));
        await this._ExecuteTest("ListInt", newSearch);
    }

    async ListIntFromProperty()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => r.ListInteger.Contains(1));
        await this._ExecuteTest("ListIntFromProperty", newSearch);
    }

    async InnerLambda()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => r.ListInteger.Any(s => s === 1));
        await this._ExecuteTest("InnerLambda", newSearch);
    }

    async NullableHasValue()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => r.OneToOneNullable.Integer !== undefined);
        await this._ExecuteTest("NullableHasValue", newSearch);
    }

    async NullableHasValueReversed()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => undefined !== r.OneToOneNullable.Integer);
        await this._ExecuteTest("NullableHasValue", newSearch);
    }

    async NullableValue()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => r.OneToOneNullable.Integer !== undefined && (r.OneToOneNullable.Integer as any as INullable<number>).Value === 1);
        await this._ExecuteTest("NullableValue", newSearch);
    }

    async LinqlObject()
    {
        const value = this._CreateDataModel();
        const type = LinqlType.GetLinqlType(value, this.context);
        this.objectTest = new LinqlObject(value, type);

        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => this.objectTest.Value.Integer == r.Integer);
        await this._ExecuteTest("LinqlObject", newSearch);
    }

    async LinqlObject_NonZero()
    {
        const value = this._CreateDataModel(1);
        const type = LinqlType.GetLinqlType(value, this.context);
        this.objectTest = new LinqlObject(value, type);
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => this.objectTest.Value.Integer == r.Integer);
        await this._ExecuteTest("LinqlObject_NonZero", newSearch);
    }

    async List_Int_Contains()
    {
        this.integers = [1, 2, 3];
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => this.integers.Contains(r.Integer));
        await this._ExecuteTest("List_Int_Contains", newSearch);
    }

    async EmptyList()
    {
        this.integers = [];
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => this.integers.Contains(r.Integer));
        await this._ExecuteTest("EmptyList", newSearch);
    }


    async String_Contains()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => "3".ToLowerInvariant().Contains(r.String));
        await this._ExecuteTest("String_Contains", newSearch);
    }


    async List_Int_Count()
    {
        this.integers = [1, 2, 3];
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => this.integers.Count === 1);
        await this._ExecuteTest("List_Int_Count", newSearch);
    }

    async Inner_Lambda()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Where(r => r.ListRecusrive.Any(s => s.ListInteger.Contains(1)));
        await this._ExecuteTest("Inner_Lambda", newSearch);
    }

    async Select_Test()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Select(r => r.Integer);
        await this._ExecuteTest("Select_Test", newSearch);
    }

    async SelectMany()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.SelectMany(r => r.ListInteger);
        await this._ExecuteTest("SelectMany", newSearch);
    }

    async SelectManyDouble()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.SelectMany(r => r.ListRecusrive.SelectMany(s => s.ListInteger));
        await this._ExecuteTest("SelectManyDouble", newSearch);
    }

    async SkipTake()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.Skip(1).Take(2).ToListAsyncSearch();
        await this._ExecuteTest("SkipTake", newSearch);
    }

    async ToListAsync()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.SelectMany(r => r.ListRecusrive.SelectMany(s => s.ListInteger)).ToListAsyncSearch();
        await this._ExecuteTest("ToListAsync", newSearch);
    }

    async FirstOrDefault()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.FirstOrDefaultSearch();
        await this._ExecuteTest("FirstOrDefault", newSearch);
    }

    async FirstOrDefaultWithPredicate()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.FirstOrDefaultSearch(r => r.Integer === 1);
        await this._ExecuteTest("FirstOrDefaultWithPredicate", newSearch);
    }

    async LastOrDefault()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.LastOrDefaultSearch();
        await this._ExecuteTest("LastOrDefault", newSearch);
    }


    async LastOrDefaultWithPredicate()
    {
        const search = this.context.Set<DataModel>(DataModel);
        const newSearch = search.LastOrDefaultSearch(r => r.Integer === 1);
        await this._ExecuteTest("LastOrDefaultWithPredicate", newSearch);
    }


    private async _ExecuteTest(TestName: string, newSearch: LinqlSearch<any>)
    {
        const json = newSearch.toJson();
        const compare = await this.testFiles.GetFile(TestName);

        if (this.debugMode)
        {
            console.log(json);
            console.log(compare);
        }
        expect(json).toEqual(compare);
    }

}


describe('LinqlSearch', () =>
{
    const contextArgs: any = {} as any;
    const test = { this: contextArgs };
    let context: ALinqlContext = new LinqlContext(LinqlSearch as any as LinqlSearchConstructor<any>, "", { this: contextArgs });
    const testClass = new TestClass();
    let functions = Object.getOwnPropertyNames(testClass.constructor.prototype)
        .filter(r => r !== "constructor" && !r.startsWith("_"));

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