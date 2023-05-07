import { IGrouping } from "../IGrouping";
import { BooleanExpression, AnyExpression, OneToManyExpression } from "../Types";

declare global
{
    export interface Array<T>
    {
        Contains<T>(ItemToCompare: T): boolean;
        Any<T>(Expression: BooleanExpression<T> | undefined): boolean;
        All<T>(Expression: BooleanExpression<T> | undefined): boolean;
        Average<T, S extends number>(Expression: AnyExpression<S> | undefined): number;
        Distinct(): Array<T>;
        GroupBy<T, S>(Expression: AnyExpression<S>): Array<IGrouping<S, T>>;
        FirstOrDefault<T>(Expression: BooleanExpression<T> | undefined): boolean;
        LastOrDefault<T>(Expression: BooleanExpression<T> | undefined): boolean;
        Min<T, S extends number>(Expression: AnyExpression<S> | undefined): number;
        Max<T, S extends number>(Expression: AnyExpression<S> | undefined): number;
        MinBy<T, S extends number>(Expression: AnyExpression<S> | undefined): T;
        MaxBy<T, S extends number>(Expression: AnyExpression<S> | undefined): T;
        Select<T, S>(Expression: AnyExpression<S>): Array<S>;
        SelectMany<T, S>(Expression: OneToManyExpression<T, S>): Array<S>;
        Skip<T>(Count: number): Array<T>;
        Sum<T, S extends number>(Expression: AnyExpression<S> | undefined): number;
        Take<T>(Count: number): Array<T>;
        Where<T>(Expression: BooleanExpression<T>): Array<T>;


    }
}
Array.prototype.Contains = function <T>(ItemToCompare: T | undefined)
{
    if (!ItemToCompare)
    {
        return false;
    }
    else
    {
        return this.indexOf(ItemToCompare) > -1;
    }
};

Array.prototype.Any = function <T>(Expression: BooleanExpression<T> | undefined)
{

    if (!Expression)
    {
        return this.length > 0;
    }
    else
    {
        return this.some(Expression);
    }
};

Array.prototype.All = function <T>(Expression: BooleanExpression<T>)
{
    return this.every(Expression);
};

Array.prototype.Average = function <T, S extends number>(Expression: AnyExpression<S> | undefined)
{
    return this.Sum(Expression) / this.length;
};

Array.prototype.Distinct = function ()
{
    const hashSet = new Set(this);
    return Array.from(hashSet);
};

Array.prototype.GroupBy = function <T, S>(Expression: AnyExpression<S>)
{
    const map = new Map<S, IGrouping<S, T>>();

    this.map(Expression).forEach((r, index) => 
    {
        const item = this[index];
        let group = map.get(r);

        if (!group)
        {
            group = new IGrouping<S, T>();
            group.Key = r;
            map.set(r, group);
        }

        group.push(item);
    });

    return Array.from(map.values());

};

Array.prototype.FirstOrDefault = function <T>(Expression: BooleanExpression<T> | undefined)
{
    if (Expression)
    {
        return this.find(Expression);
    }
    else
    {
        return this.find(r => true);
    }
}

Array.prototype.LastOrDefault = function <T>(Expression: BooleanExpression<T> | undefined)
{
    const reverse = this.reverse();
    if (Expression)
    {
        return reverse.find(Expression);
    }
    else
    {
        return reverse.find(r => true);
    }
}
Array.prototype.Min = function <T, S extends number>(Expression: AnyExpression<S> | undefined)
{
    let numbers: Array<number>;

    if (Expression)
    {
        numbers = this.map(Expression);
    }
    else
    {
        numbers = this;
    }

    return Math.min(...numbers);
}

Array.prototype.Max = function <T, S extends number>(Expression: AnyExpression<S> | undefined)
{
    let numbers: Array<number>;

    if (Expression)
    {
        numbers = this.map(Expression);
    }
    else
    {
        numbers = this;
    }

    return Math.max(...numbers);
}


Array.prototype.MinBy = function <T, S extends number>(Expression: AnyExpression<S> | undefined)
{
    const minBy = this.Min(Expression);
    let map = this;

    if (Expression)
    {
        map = this.map(Expression);
    }

    const index = map.indexOf(minBy);

    if (index < 0)
    {
        return null;
    }
    else
    {
        return this[index];
    }
}

Array.prototype.MaxBy = function <T, S extends number>(Expression: AnyExpression<S> | undefined)
{
    const minBy = this.Max(Expression);
    let map = this;

    if (Expression)
    {
        map = this.map(Expression);
    }

    const index = map.indexOf(minBy);

    if (index < 0)
    {
        return null;
    }
    else
    {
        return this[index];
    }
}

Array.prototype.Select = function <T, S>(Expression: AnyExpression<S>)
{
    return this.map(Expression);
}
Array.prototype.SelectMany = function <T, S>(Expression: OneToManyExpression<T, S>)
{
    const mapOfMaps: Array<Array<S>> = this.map(Expression);
    return mapOfMaps.reduce((left, right) => left.concat(...right));
}

Array.prototype.Skip = function <T, S>(Count: number)
{
    return this.slice(Count);
}

Array.prototype.Take = function <T, S>(Count: number)
{
    return this.slice(0, Count);
}

Array.prototype.Sum = function <T, S extends number>(Expression: AnyExpression<S> | undefined)
{
    let map = this;

    if (Expression)
    {
        map = this.map(Expression);
    }

    return map.reduce((left, right) => left + right);
};

Array.prototype.Where = function <T>(Expression: BooleanExpression<T>)
{
    return this.filter(Expression);
}


export { };