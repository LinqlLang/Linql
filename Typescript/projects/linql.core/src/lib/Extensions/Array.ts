import { IGrouping } from "./IGrouping";
import { AnyExpression, BooleanExpression, OneToManyExpression, TransformExpression } from "./Types";

declare global
{
    export interface Array<T>
    {
        All(Expression: BooleanExpression<T>): boolean;
        Any(Expression?: BooleanExpression<T>): boolean;
        Average(Expression?: TransformExpression<T, number>): number;
        Contains(ItemToCompare: T): boolean;
        Count: number;
        Distinct(): Array<T>;
        FirstOrDefault(Expression?: BooleanExpression<T>): T;
        GroupBy<S>(Expression: AnyExpression<S>): Array<IGrouping<S, T>>;
        LastOrDefault(Expression?: BooleanExpression<T>): T;
        Max(Expression: TransformExpression<T, number>): number;
        MaxBy<S>(Expression: TransformExpression<T, S>): T;
        Min(Expression: TransformExpression<T, number>): number;
        MinBy<S>(Expression: TransformExpression<T, S>): T;
        Select<S>(Expression: TransformExpression<T, S>): Array<S>;
        SelectMany<S>(Expression: OneToManyExpression<T, S>): Array<S>;
        Skip(Count: number): Array<T>;
        Sum(Expression?: TransformExpression<T, number>): number;
        Take(Count: number): Array<T>;
        Where(Expression: BooleanExpression<T>): Array<T>;

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

Array.prototype.Any = function <T>(Expression?: BooleanExpression<T>)
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

Array.prototype.Average = function <T>(Expression?: TransformExpression<T, number>)
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

Array.prototype.FirstOrDefault = function <T>(Expression?: BooleanExpression<T>)
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

Array.prototype.LastOrDefault = function <T>(Expression?: BooleanExpression<T>)
{
    const reverse = Array.from(this).reverse();
    if (Expression)
    {
        return reverse.find(Expression);
    }
    else
    {
        return reverse.find(r => true);
    }
}
Array.prototype.Min = function <T>(Expression?: TransformExpression<T, number>)
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

Array.prototype.Max = function <T>(Expression?: TransformExpression<T, number>)
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


Array.prototype.MinBy = function <T, S>(Expression: TransformExpression<T, S>)
{
    let map = this;

    if (Expression)
    {
        map = this.map(Expression);
    }

    let zip = map.map((r, index) => [r, this[index]]);

    const order = zip.sort((left, right) => left[0] - right[0]);
    const first = order.FirstOrDefault();

    if (first)
    {
        return first[1];
    }
    else
    {
        return first;
    }
}

Array.prototype.MaxBy = function <T, S>(Expression: TransformExpression<T, S>)
{
    let map = this;

    if (Expression)
    {
        map = this.map(Expression);
    }

    let zip = map.map((r, index) => [r, this[index]]);

    const order = zip.sort((left, right) => left[0] - right[0]);
    const last = order.LastOrDefault();

    if (last)
    {
        return last[1];
    }
    else
    {
        return last;
    }
}

Array.prototype.Select = function <T, S>(Expression: TransformExpression<T, S>)
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

Array.prototype.Sum = function <T>(Expression?: TransformExpression<T, number>)
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


export default {};