import { ALinqlContext, ALinqlSearch } from "./ALinqlSearch";

export class LinqlContext extends ALinqlContext
{

    public async GetResult<T, TResult>(Search: ALinqlSearch<any>): Promise<TResult>
    {
        const endpoint = this.GetEndpoint(Search);
        return await this.SendHttpRequest(endpoint, Search);
    }
    protected async SendHttpRequest<TResult>(Endpoint: string, Search: ALinqlSearch<any>): Promise<TResult>
    {

        const json = this.ToJson(Search);
        const requestOptions: RequestInit = {
            method: "POST",
            mode: "cors",
            body: json
        }
        return await fetch(Endpoint, requestOptions).then(r => <TResult>r.json());
    }

}