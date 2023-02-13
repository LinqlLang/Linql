import { LinqlConstant, LinqlExpression, LinqlFunction, LinqlType } from "linql.core";
import { ALinqlContext, ALinqlSearch } from "./ALinqlSearch";
import { LinqlContext } from "./LinqlContext"

declare type BooleanExpression<T> = (r: T) => boolean;
declare type AnyExpression<T> = (r: T) => any;
declare type TransformExpression<T, S> = (r: T) => S;


export class LinqlSearch<T> extends ALinqlSearch<T>
{
    constructor(Type: (string | (new () => T)), ArgumentContext: {} = {}, Context: ALinqlContext = new LinqlContext(LinqlSearch<any>))
    {
        super(Type, ArgumentContext, Context);
    }

    protected Copy(): LinqlSearch<T>
    {
        const search = new LinqlSearch<T>(this.Type, this.ArgumentContext, this.Context);

        if (this.Expressions)
        {
            search.Expressions = Array.from(this.Expressions);
        }

        return search;
    }

    private Parse(Expression: AnyExpression<T> | string | undefined): LinqlExpression
    {
        return {} as LinqlExpression;
    }

    //#region Functions

    public CustomLinqlFunction<S>(FunctionName: string, Expression: AnyExpression<T> | string | undefined): LinqlSearch<S>
    {

        let expressions = new Array<LinqlExpression>();
        if (this.Expressions)
        {
            expressions = Array.from(this.Expressions);
        }

        const newExpression = this.Parse(Expression);
        const newSearch = this.Copy();
        newSearch.Expressions?.push(newExpression);
        return newSearch as any as LinqlSearch<S>;
    }

    public filter(Expression: BooleanExpression<T> | string)
    {
        return this.CustomLinqlFunction<T>("Where", Expression);
    }

    public any(Expression: BooleanExpression<T> | string)
    {
        return this.CustomLinqlFunction<T>("Any", Expression);
    }

    public all(Expression: BooleanExpression<T> | string)
    {
        return this.CustomLinqlFunction<T>("All", Expression);
    }

    public distinct()
    {
        return this.CustomLinqlFunction<T>("Distinct", undefined);
    }

    public select<S>(Expression: TransformExpression<T, S>)
    {
        return this.CustomLinqlFunction<S>("Select", Expression);
    }

    public selectMany<S>(Expression: TransformExpression<T, S>)
    {
        return this.CustomLinqlFunction<S>("SelectMany", Expression);
    }

    public groupBy<S>(Expression: TransformExpression<T, S>)
    {
        return this.CustomLinqlFunction<IGrouping<S, T>>("GroupBy", Expression);
    }

    public orderBy<S>(Expression: TransformExpression<T, S>): IOrderedLinqlSearch<T>
    {
        return this.CustomLinqlFunction<T>("OrderBy", Expression) as IOrderedLinqlSearch<T>;
    }

    public orderByDescending<S>(Expression: TransformExpression<T, S>): IOrderedLinqlSearch<T>
    {
        return this.CustomLinqlFunction<T>("OrderByDescending", Expression) as IOrderedLinqlSearch<T>;
    }

    public skip(Skip: number): LinqlSearch<T>
    {
        const type = new LinqlType();
        type.TypeName = "int32";
        const constant = new LinqlConstant(type, Skip);
        const fun = new LinqlFunction("Skip", [constant]);
        const newExpression = this.Copy();
        newExpression.Expressions?.push(fun);
        return newExpression;
    }

    public take(Take: number): LinqlSearch<T>
    {
        const type = new LinqlType();
        type.TypeName = "int32";
        const constant = new LinqlConstant(type, Take);
        const fun = new LinqlFunction("Take", [constant]);
        const newExpression = this.Copy();
        newExpression.Expressions?.push(fun);
        return newExpression;
    }


    //#endregion

}

export class IOrderedLinqlSearch<T> extends LinqlSearch<T>
{
    public thenByDescending<S>(Expression: TransformExpression<T, S>)
    {
        return this.CustomLinqlFunction<T>("ThenByDescending", Expression);
    }

    public thenBy<S>(Expression: TransformExpression<T, S>)
    {
        return this.CustomLinqlFunction<T>("ThenBy", Expression);
    }
}

export class IGrouping<Key, Value> extends Array<Value>
{
    public Key: Key | undefined;
}
