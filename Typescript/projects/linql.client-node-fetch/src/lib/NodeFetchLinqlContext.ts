import { ALinqlContext, ALinqlSearch } from "linql.client";
import fetch, { RequestInit } from 'node-fetch';
export class NodeFetchLinqlContext extends ALinqlContext
{

    public async GetResult<T, TResult>(Search: ALinqlSearch<any>): Promise<TResult>
    {
        const endpoint = this.GetEndpoint(Search);
        return await this.SendHttpRequest(endpoint, Search);
    }

    protected GetHeaders()
    {
        return { 'Content-Type': 'application/json' };
    }

    protected async SendHttpRequest<TResult>(Endpoint: string, Search: ALinqlSearch<any>): Promise<TResult>
    {
        const json = this.ToJson(Search);
        const headers = this.GetHeaders();
        const requestOptions: RequestInit = {
            method: "POST",
            body: json,
            headers: headers
        }
        const result = await fetch(Endpoint, requestOptions);
        const parsedResult = await result.json();
        return <TResult>parsedResult;

    }

}