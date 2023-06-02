import { ALinqlContext, ALinqlSearch, LinqlSearch } from "linql.client";
import { AnyExpression, BooleanExpression, TransformExpression } from "linql.core";
import { Observable } from "rxjs";
import { IObservableContext } from "./IObservableContext";

export class LinqlObservable<T> extends LinqlSearch<T>
{

  private isObservableContext(Context: ALinqlContext | IObservableContext): Context is IObservableContext
  {
    return (Context as IObservableContext).GetObservableResult !== undefined;
  }

  executeCustomLinqlFunctionObservable<TResult>(FunctionName: string, Predicate: AnyExpression<any> | string | undefined = undefined): Observable<TResult>
  {
    const search: ALinqlSearch<T> = this.CustomLinqlFunction(FunctionName, Predicate);

    if (this.isObservableContext(this.Context))
    {
      return this.Context.GetObservableResult(search);
    }
    throw 'Context was not an IObservableContext.  Unable to useObservableMethods';
  }

  ToListObservable(): Observable<Array<T>>
  {
    return this.executeCustomLinqlFunctionObservable("ToListAsync");
  }

  FirstOrDefaultObservable(Predicate: BooleanExpression<T> | string | undefined = undefined): Observable<T | undefined>
  {
    return this.executeCustomLinqlFunctionObservable("FirstOrDefaultAsync", Predicate);
  }

  LastOrDefaultObservable(Predicate: BooleanExpression<T> | string | undefined = undefined): Observable<T | undefined>
  {
    return this.executeCustomLinqlFunctionObservable("FirstOrDefaultAsync", Predicate);
  }

  public AnyObservable(Predicate?: BooleanExpression<T> | string): Observable<boolean>
  {
    return this.executeCustomLinqlFunctionObservable("AnyAsync", Predicate);
  }

  public AllObservable(Predicate?: BooleanExpression<T> | string): Observable<boolean>
  {
    return this.executeCustomLinqlFunctionObservable("AllAsync", Predicate);
  }

  public MinObservable(Predicate?: TransformExpression<T, number> | string): Observable<number>
  {
    return this.executeCustomLinqlFunctionObservable("MinAsync", Predicate);
  }

  public MaxObservable(Predicate?: TransformExpression<T, number> | string): Observable<number>
  {
    return this.executeCustomLinqlFunctionObservable("MaxAsync", Predicate);
  }

  public MinByObservable(Predicate: TransformExpression<T, number> | string): Observable<T>
  {
    return this.executeCustomLinqlFunctionObservable("MinByAsync", Predicate);
  }

  public MaxByObservable(Predicate: TransformExpression<T, number> | string): Observable<T>
  {
    return this.executeCustomLinqlFunctionObservable("MaxByAsync", Predicate);
  }

  public SumObservable(Predicate?: TransformExpression<T, number> | string): Observable<number>
  {
    return this.executeCustomLinqlFunctionObservable("SumAsync", Predicate);
  }

  public AverageObservable(Predicate?: TransformExpression<T, number> | string): Observable<number>
  {
    return this.executeCustomLinqlFunctionObservable("AverageAsync", Predicate);
  }
}