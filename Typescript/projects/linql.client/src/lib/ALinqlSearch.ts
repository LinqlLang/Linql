import { AnyExpression, BooleanExpression, GenericConstructor, IGrouping, LinqlConstant, LinqlExpression, LinqlFunction, LinqlSearch, LinqlType, TransformExpression } from "linql.core";
import { AOrderedLinqlSearch } from "./AOrderedLinqlSearch";
import { LinqlParser } from "./LinqlParser";


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

        const functionArguments = this.Context.Parse(Expression);

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

    public Where(Expression: BooleanExpression<T> | string)
    {
        return this.CustomLinqlFunction<T>("Where", Expression);
    }

    public Any(Expression: BooleanExpression<T> | string)
    {
        return this.CustomLinqlFunction<T>("Any", Expression);
    }

    public All(Expression: BooleanExpression<T> | string)
    {
        return this.CustomLinqlFunction<T>("All", Expression);
    }

    public Distinct()
    {
        return this.CustomLinqlFunction<T>("Distinct", undefined);
    }

    public Select<S>(Expression: TransformExpression<T, S>)
    {
        return this.CustomLinqlFunction<S>("Select", Expression);
    }

    public SelectMany<S>(Expression: TransformExpression<T, S>)
    {
        return this.CustomLinqlFunction<S>("SelectMany", Expression);
    }

    public GroupBy<S>(Expression: TransformExpression<T, S>)
    {
        return this.CustomLinqlFunction<IGrouping<S, T>>("GroupBy", Expression);
    }

    public OrderBy<S>(Expression: TransformExpression<T, S>): AOrderedLinqlSearch<T>
    {
        return this.CustomLinqlFunction<T>("OrderBy", Expression) as AOrderedLinqlSearch<T>;
    }

    public OrderByDescending<S>(Expression: TransformExpression<T, S>): AOrderedLinqlSearch<T>
    {
        return this.CustomLinqlFunction<T>("OrderByDescending", Expression) as AOrderedLinqlSearch<T>;
    }

    public Skip(Skip: number)
    {
        const type = new LinqlType();
        type.TypeName = "Int32";
        const constant = new LinqlConstant(type, Skip);
        const fun = new LinqlFunction("Skip", [constant]);
        const newSearch = this.Copy();
        this.AttachTopLevelFunction(fun, newSearch);
        return newSearch;
    }

    public Take(Take: number)
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
        const newSearch = this.CustomLinqlFunction("ToListAsync");
        return newSearch;
    }

    FirstOrDefaultSearch(Predicate: BooleanExpression<T> | string | undefined = undefined)
    {
        return this.CustomLinqlFunction("FirstOrDefaultAsync", Predicate);
    }

    LastOrDefaultSearch(Predicate: BooleanExpression<T> | string | undefined = undefined)
    {
        return this.CustomLinqlFunction("LastOrDefaultAsync", Predicate);
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

    //#endregion



}

export abstract class ALinqlContext
{
    constructor(public LinqlSearchType: LinqlSearchConstructor<any>, public BaseUrl: string, public ArgumentContext: {} = {})
    {

    }

    public abstract GetResult<T, TResult>(Search: ALinqlSearch<T>): Promise<TResult>;

    protected abstract SendHttpRequest<TResult>(Endpoint: string, Search: ALinqlSearch<any>): Promise<TResult>;

    Parse(Expression: string | AnyExpression<any> | undefined): LinqlExpression | undefined
    {
        const parser = new LinqlParser(Expression, this.ArgumentContext);
        return parser.Root;
    }

    ToJson(Search: ALinqlSearch<any>)
    {
        const copy: any = Search.Copy();
        copy.Context = undefined;
        copy.ArgumentContext = undefined;
        copy.ModelType = undefined;
        return JSON.stringify(copy);
    }

    GetTypeName(Type: string | GenericConstructor<any>)
    {
        if (typeof Type === "string")
        {
            return Type;
        }
        else
        {
            return Type.name;
        }
    }

    protected GetEndpoint<T>(Search: ALinqlSearch<T>)
    {
        let endPoint: string;
        if (typeof Search.ModelType === "string")
        {
            endPoint = Search.ModelType;
        }
        else
        {
            endPoint = Search.ModelType.name;
        }
        return `linql/${ endPoint }`
    }

    public Set<T>(Type: string | GenericConstructor<T>, ArgumentContext: {} | undefined = this.ArgumentContext): ALinqlSearch<T>
    {
        return new this.LinqlSearchType(Type, ArgumentContext, this);
    }
}