import { LinqlExpression } from "linql.core";

export abstract class ALinqlSearch<T>
{
    public Expressions: Array<LinqlExpression> | undefined;

    constructor(public Type: (string | (new () => T)), public ArgumentContext: {} | undefined = {}, public Context: ALinqlContext)
    {

    }
}

export abstract class ALinqlContext
{
    constructor(public LinqlSearchType: new (Type: (string | (new () => any)), ArgumentContext: {} | undefined, Context: ALinqlContext) => ALinqlSearch<any>)
    {

    }

    public abstract GetResult<TResult>(Search: ALinqlSearch<any>): Promise<TResult>;

    protected abstract SendHttpRequest<TResult>(Endpoint: string, Search: ALinqlSearch<any>): Promise<TResult>;

    protected abstract SendRequest<TResult>(Search: ALinqlSearch<any>): Promise<TResult>;

    ToJson(Search: ALinqlSearch<any>)
    {
        return JSON.stringify(Search);
    }

    protected GetEndpoint(Search: ALinqlSearch<any>)
    {
        let endPoint: string;
        if (typeof Search.Type === "string")
        {
            endPoint = Search.Type;
        }
        else
        {
            endPoint = Search.Type.name;
        }
        return `linql/${ endPoint }`
    }

    public abstract Parse(Expression: any): LinqlExpression;

    public Set<T>(Type: new () => T, ArgumentContext: {} | undefined): ALinqlSearch<T>
    {
        return new this.LinqlSearchType(Type, ArgumentContext, this);
    }
}