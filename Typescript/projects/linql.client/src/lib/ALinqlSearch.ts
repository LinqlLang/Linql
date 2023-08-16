import { AnyExpression, BooleanExpression, GenericConstructor, IGrouping, LinqlConstant, LinqlExpression, LinqlFunction, LinqlSearch, LinqlType, TransformExpression, ITypeNameProvider } from "linql.core";
// import { OrderedLinqlSearch } from "./AOrderedLinqlSearch";
import { LinqlParser } from "./LinqlParser";
import { constructorType } from "./LinqlConstructorType";


export declare type LinqlSearchConstructor<T> =
    new (
        Type: string | GenericConstructor<T>,
        ArgumentContext: {} | undefined,
        Context: ALinqlContext
    ) => ALinqlSearch<any>;


export abstract class ALinqlSearch<T> extends LinqlSearch
{
    public override Expressions: Array<LinqlExpression> | undefined;

    constructor(public ModelType: string | GenericConstructor<T>, public ArgumentContext: {} | undefined = {}, public Context: ALinqlContext)
    {
        super(new LinqlType());
        this.Type.TypeName = this.Context.GetTypeName(ModelType);
        this.Expressions = new Array<LinqlExpression>();
        const searchExpression = this.BuildLinqlSearchExpression();
        this.Expressions.push(searchExpression);
    }


    private BuildLinqlSearchExpression()
    {
        const searchType = new LinqlType();
        searchType.TypeName = "LinqlSearch";
        searchType.GenericParameters = new Array<LinqlType>();
        searchType.GenericParameters.push(this.Type);
        const searchExpression = new LinqlConstant(searchType, undefined);
        return searchExpression;

    }

    abstract Copy(): this;

    public CustomLinqlFunction<S>(FunctionName: string, Expression: AnyExpression<T> | string | undefined = undefined): ALinqlSearch<S>
    {
        const customFunction = new LinqlFunction(FunctionName);

        const functionArguments = this.Context.Parse(Expression, this.ArgumentContext);

        if (functionArguments)
        {
            customFunction.Arguments = new Array<LinqlExpression>();
            customFunction.Arguments.push(functionArguments);
        }

        const newSearch = this.Copy();
        this.AttachTopLevelFunction(customFunction, newSearch);
        return newSearch as any as ALinqlSearch<S>;
    }

    public AttachTopLevelFunction(customFunction: LinqlFunction, search: LinqlSearch)
    {
        const firstExpression = search.Expressions?.find(r => true);

        if (firstExpression)
        {
            const lastExpression = firstExpression.GetLastExpressionInNextChain();
            lastExpression.Next = customFunction;
        }
        else if (customFunction)
        {
            search.Expressions?.push(customFunction);
        }
    }

    //#region Functions

    public Where(Expression: BooleanExpression<T> | string): this
    {
        return this.CustomLinqlFunction<T>("Where", Expression) as this;
    }

    public Distinct()
    {
        return this.CustomLinqlFunction<T>("Distinct", undefined);
    }

    public Select<S>(Expression: TransformExpression<T, S> | string)
    {
        return this.CustomLinqlFunction<S>("Select", Expression);
    }

    public Include<S>(Expression: TransformExpression<T, S> | string): this
    {
        return this.CustomLinqlFunction<T>("Include", Expression) as this;
    }

    public SelectMany<S>(Expression: TransformExpression<T, S> | string)
    {
        return this.CustomLinqlFunction<S>("SelectMany", Expression);
    }

    public GroupBy<S>(Expression: TransformExpression<T, S> | string)
    {
        return this.CustomLinqlFunction<IGrouping<S, T>>("GroupBy", Expression);
    }

    public OrderBy<S>(Expression: TransformExpression<T, S> | string): OrderedLinqlSearch<T>
    {
        const custom = this.CustomLinqlFunction<T>("OrderBy", Expression);
        const convert = new OrderedLinqlSearch<T>(this.ModelType, this.ArgumentContext, this.Context);
        convert.Expressions = custom.Expressions;
        return convert;

    }

    public OrderByDescending<S>(Expression: TransformExpression<T, S> | string): OrderedLinqlSearch<T>
    {
        const custom = this.CustomLinqlFunction<T>("OrderByDescending", Expression);
        const convert = new OrderedLinqlSearch<T>(this.ModelType, this.ArgumentContext, this.Context);
        convert.Expressions = custom.Expressions;
        return convert;
    }

    public Skip(Skip: number): this
    {
        const type = new LinqlType();
        type.TypeName = "Int32";
        const constant = new LinqlConstant(type, Skip);
        const fun = new LinqlFunction("Skip", [constant]);
        const newSearch = this.Copy();
        this.AttachTopLevelFunction(fun, newSearch);
        return newSearch;
    }

    public Take(Take: number): this
    {
        const type = new LinqlType();
        type.TypeName = "Int32";
        const constant = new LinqlConstant(type, Take);
        const fun = new LinqlFunction("Take", [constant]);
        const newSearch = this.Copy();
        this.AttachTopLevelFunction(fun, newSearch);
        return newSearch;
    }

    toJson(): string
    {
        return this.Context.ToJson(this);
    }

    ToListAsyncSearch()
    {
        const newSearch = this.CustomLinqlFunction<Array<T>>("ToListAsync");
        return newSearch;
    }

    FirstOrDefaultSearch(Predicate: BooleanExpression<T> | string | undefined = undefined)
    {
        return this.CustomLinqlFunction<T | null>("FirstOrDefaultAsync", Predicate);
    }

    LastOrDefaultSearch(Predicate: BooleanExpression<T> | string | undefined = undefined)
    {
        return this.CustomLinqlFunction<T | null>("LastOrDefaultAsync", Predicate);
    }

    public AnySearch(Predicate?: BooleanExpression<T> | string)
    {
        return this.CustomLinqlFunction<boolean>("AnyAsync", Predicate);
    }

    public AllSearch(Predicate?: BooleanExpression<T> | string)
    {
        return this.CustomLinqlFunction<boolean>("AllAsync", Predicate);
    }

    public MinSearch(Predicate?: TransformExpression<T, number> | string)
    {
        return this.CustomLinqlFunction<number>("MinAsync", Predicate);
    }

    public MaxSearch(Predicate?: TransformExpression<T, number> | string)
    {
        return this.CustomLinqlFunction<number>("MaxAsync", Predicate);
    }

    public MinBySearch(Predicate: TransformExpression<T, number> | string)
    {
        return this.CustomLinqlFunction<T>("MinByAsync", Predicate);
    }

    public MaxBySearch(Predicate: TransformExpression<T, number> | string)
    {
        return this.CustomLinqlFunction<T>("MaxByAsync", Predicate);
    }

    public SumSearch(Predicate?: TransformExpression<T, number> | string)
    {
        return this.CustomLinqlFunction<number>("SumAsync", Predicate);
    }

    public AverageSearch(Predicate?: TransformExpression<T, number> | string)
    {
        return this.CustomLinqlFunction<number>("AverageAsync", Predicate);
    }

    public CountSearch()
    {
        return this.CustomLinqlFunction<number>("CountAsync");
    }

    //#endregion

    //#region ExecuteFunctions

    async executeCustomLinqlFunction<TResult>(FunctionName: string, Predicate: AnyExpression<any> | string | undefined = undefined): Promise<TResult>
    {
        const search: ALinqlSearch<T> = this.CustomLinqlFunction(FunctionName, Predicate);
        return await this.Context.GetResult<T, TResult>(search);
    }

    async ToListAsync(): Promise<Array<T>>
    {
        return this.executeCustomLinqlFunction("ToListAsync");
    }

    async FirstOrDefaultAsync(Predicate: BooleanExpression<T> | string | undefined = undefined): Promise<T | undefined>
    {
        return this.executeCustomLinqlFunction("FirstOrDefaultAsync", Predicate);
    }

    async LastOrDefaultAsync(Predicate: BooleanExpression<T> | string | undefined = undefined): Promise<T | undefined>
    {
        return this.executeCustomLinqlFunction("FirstOrDefaultAsync", Predicate);
    }

    public AnyAsync(Predicate?: BooleanExpression<T> | string): Promise<boolean>
    {
        return this.executeCustomLinqlFunction("AnyAsync", Predicate);
    }

    public AllAsync(Predicate?: BooleanExpression<T> | string): Promise<boolean>
    {
        return this.executeCustomLinqlFunction("AllAsync", Predicate);
    }

    public MinAsync(Predicate?: TransformExpression<T, number> | string): Promise<number>
    {
        return this.executeCustomLinqlFunction("MinAsync", Predicate);
    }

    public MaxAsync(Predicate?: TransformExpression<T, number> | string): Promise<number>
    {
        return this.executeCustomLinqlFunction("MaxAsync", Predicate);
    }

    public MinByAsync(Predicate: TransformExpression<T, number> | string): Promise<T>
    {
        return this.executeCustomLinqlFunction("MinByAsync", Predicate);
    }

    public MaxByAsync(Predicate: TransformExpression<T, number> | string): Promise<T>
    {
        return this.executeCustomLinqlFunction("MaxByAsync", Predicate);
    }

    public SumAsync(Predicate?: TransformExpression<T, number> | string): Promise<number>
    {
        return this.executeCustomLinqlFunction("SumAsync", Predicate);
    }

    public AverageAsync(Predicate?: TransformExpression<T, number> | string): Promise<number>
    {
        return this.executeCustomLinqlFunction("AverageAsync", Predicate);
    }

    public CountAsync(): Promise<number>
    {
        return this.executeCustomLinqlFunction("CountAsync");
    }

    //#endregion
}

export abstract class ALinqlContext implements ITypeNameProvider
{
    constructor(public LinqlSearchType: LinqlSearchConstructor<any>, public BaseUrl: string, public ArgumentContext: {} = {})
    {

    }

    public abstract GetResult<T, TResult>(Search: ALinqlSearch<T>): Promise<TResult>;

    protected abstract SendHttpRequest<TResult>(Endpoint: string, Search: ALinqlSearch<any>): Promise<TResult>;

    Parse(Expression: string | AnyExpression<any> | undefined, ArgumentContext: {} = this.ArgumentContext): LinqlExpression | undefined
    {
        if (!ArgumentContext)
        {
            ArgumentContext = this.ArgumentContext;
        }
        const parser = new LinqlParser(Expression, ArgumentContext, this);
        return parser.Root;
    }

    ToJson(Search: ALinqlSearch<any>)
    {
        const copy: any = this.GetOptimizedSearch(Search);
        return JSON.stringify(copy);
    }

    GetTypeName(Type: string | GenericConstructor<any>): string
    {
        const anyCast = Type as any;
        if (typeof Type === "string")
        {
            return Type;
        }
        else if (anyCast.constructor && anyCast.constructor.name !== "Function")
        {
            return this.GetTypeName(anyCast.constructor);
        }
        else if (anyCast["Type"])
        {
            return anyCast["Type"];
        }
        else
        {
            return Type.name;
        }
    }

    private GetSearchTypeString<T>(Search: ALinqlSearch<T>)
    {
        let searchTypeString: string = this.GetTypeName(Search.ModelType);
        return searchTypeString;
    }

    protected GetRoute<T>(Search: ALinqlSearch<T>)
    {
        const endpoint = this.GetSearchTypeString(Search);
        return `linql/${ endpoint }`

    }

    protected GetEndpoint<T>(Search: ALinqlSearch<T>)
    {
        let basePath = this.BaseUrl;
        const route = this.GetRoute(Search);

        if (!basePath.endsWith("/"))
        {
            basePath += "/";
        }

        return `${ basePath }${ route }`;
    }

    public Set<T>(Type: string | GenericConstructor<T>, ArgumentContext: {} | undefined = this.ArgumentContext): ALinqlSearch<T>
    {
        return new this.LinqlSearchType(Type, ArgumentContext, this);
    }

    protected GetOptimizedSearch<T>(Search: ALinqlSearch<T>)
    {
        const optimizedSearch = Search.Copy();
        const anycast = optimizedSearch as any;
        optimizedSearch.ArgumentContext = undefined;
        anycast.Context = undefined;
        anycast.ModelType = undefined;
        return optimizedSearch;
    }
}

export class OrderedLinqlSearch<T> extends ALinqlSearch<T>
{
    public Copy(): this
    {
        const objCast = this as Object;
        const constructor: constructorType<T, this> = objCast.constructor as constructorType<T, this>;
        const search = new constructor(this.ModelType, this.ArgumentContext, this.Context)

        if (this.Expressions)
        {
            search.Expressions = this.Expressions.map(r => r.Clone());
        }

        return search as this;
    }

    public ThenByDescending<S>(Expression: TransformExpression<T, S> | string): this
    {
        return this.CustomLinqlFunction<T>("ThenByDescending", Expression) as this;
    }

    public ThenBy<S>(Expression: TransformExpression<T, S> | string): this
    {
        return this.CustomLinqlFunction<T>("ThenBy", Expression) as this;
    }
}