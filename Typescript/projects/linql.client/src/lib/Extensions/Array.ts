import { BooleanExpression } from "../Types";

declare global
{
    export interface Array<T>
    {
        Contains<T>(ItemToCompare: T): boolean;
        Any<T>(Expression: BooleanExpression<T> | undefined): boolean;
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
        return this.find(Expression).length > 0;
    }
};

export { };