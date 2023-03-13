import { HttpClient } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { ALinqlContext, ALinqlSearch, LinqlSearch, LinqlSearchConstructor } from 'linql.client';
import { LinqlSearchType, BaseUrl as BaseUrlToken } from './Tokens';
import { lastValueFrom } from "rxjs";
@Injectable({
  providedIn: 'root'
})
export class LinqlContext extends ALinqlContext
{
  async GetResult<T, TResult>(Search: ALinqlSearch<T>): Promise<TResult>
  {
    const endpoint = this.GetEndpoint(Search);
    return await this.SendHttpRequest(endpoint, Search);
  }
  protected SendHttpRequest<TResult>(Endpoint: string, Search: ALinqlSearch<any>): Promise<TResult>
  {
    return lastValueFrom(this.Client.post<TResult>(Endpoint, Search));
  }

  constructor(@Inject(LinqlSearchType) LinqlSearchType: LinqlSearchConstructor<any> = LinqlSearch, @Inject(BaseUrlToken) BaseUrl: string, public Client: HttpClient)
  {
    super(LinqlSearchType, BaseUrl, {});

  }
}
