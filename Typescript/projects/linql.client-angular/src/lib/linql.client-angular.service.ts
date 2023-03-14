import { HttpClient } from '@angular/common/http';
import { Inject, Injectable, Optional } from '@angular/core';
import { ALinqlContext, ALinqlSearch, LinqlSearch, LinqlSearchConstructor } from 'linql.client';
import { LinqlSearchType, BaseUrl as BaseUrlToken } from './Tokens';
import { lastValueFrom } from "rxjs";
export class LinqlContext extends ALinqlContext
{
  async GetResult<T, TResult>(Search: ALinqlSearch<T>): Promise<TResult>
  {
    const endpoint = this.GetEndpoint(Search);
    return await this.SendHttpRequest(endpoint, Search);
  }
  protected SendHttpRequest<TResult>(Endpoint: string, Search: ALinqlSearch<any>): Promise<TResult>
  {
    const optimizedSearch = Search.Copy();
    const anycast = optimizedSearch as any;
    optimizedSearch.ArgumentContext = undefined;
    anycast.Context = undefined;
    anycast.ModelType = undefined;
    return lastValueFrom(this.Client.post<TResult>(Endpoint, optimizedSearch));
  }

  constructor(BaseUrl: string, public Client: HttpClient, LinqlSearchType: LinqlSearchConstructor<any> = LinqlSearch)
  {
    super(LinqlSearchType, BaseUrl, {});

  }
}
