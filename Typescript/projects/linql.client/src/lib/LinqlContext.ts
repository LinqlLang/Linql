import { ALinqlContext, ALinqlSearch } from "./ALinqlSearch";

export class LinqlContext extends ALinqlContext
{

    public GetResult<TResult>(Search: ALinqlSearch<any>): Promise<TResult>
    {

        throw new Error("Method not implemented.");
    }
    protected SendHttpRequest<TResult>(Endpoint: string, Search: ALinqlSearch<any>): Promise<TResult>
    {
        throw new Error("Method not implemented.");
    }

    async SendRequest<TResult>(Search: ALinqlSearch<any>): Promise<TResult>
    {
        return null as TResult;
    }
}