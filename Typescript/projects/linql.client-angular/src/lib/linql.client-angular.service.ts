import { HttpClient } from '@angular/common/http';
import { ALinqlContext, ALinqlSearch, LinqlSearchConstructor } from 'linql.client';
import { Observable, lastValueFrom } from "rxjs";
import { IObservableContext } from './IObservableContext';
import { LinqlObservable } from './LinqlObservable';
import { GenericConstructor } from 'linql.core';
export class LinqlContext extends ALinqlContext implements IObservableContext
{
  constructor(BaseUrl: string, public Client: HttpClient, LinqlSearchType: LinqlSearchConstructor<any> = LinqlObservable)
  {
    super(LinqlSearchType, BaseUrl, {});
  }

  async GetResult<T, TResult>(Search: ALinqlSearch<T>): Promise<TResult>
  {
    const endpoint = this.GetEndpoint(Search);
    return await this.SendHttpRequest(endpoint, Search);
  }

  protected SendHttpRequest<TResult>(Endpoint: string, Search: ALinqlSearch<any>): Promise<TResult>
  {
    const optimizedSearch = this.GetOptimizedSearch(Search);
    return lastValueFrom(this.Client.post<TResult>(Endpoint, optimizedSearch));
  }

  GetObservableResult<T, TResult>(Search: ALinqlSearch<T>): Observable<TResult>
  {
    const endpoint = this.GetEndpoint(Search);
    const optimizedSearch = this.GetOptimizedSearch(Search);
    return this.Client.post<TResult>(endpoint, optimizedSearch);
  }


  SetObservable<T>(Type: string | GenericConstructor<T>, ArgumentContext?: {} | undefined): LinqlObservable<T>
  {
    return this.Set<T>(Type, ArgumentContext) as LinqlObservable<T>;
  }
}
