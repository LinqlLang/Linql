import { ALinqlSearch } from "linql.client";
import { Observable } from "rxjs";

export interface IObservableContext
{
  GetObservableResult<T, TResult>(Search: ALinqlSearch<T>): Observable<TResult>;
}