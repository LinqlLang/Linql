import { LinqlExpression } from "./LinqlExpression";
import { LinqlType } from "./LinqlType";

export class LinqlSearch 
{

    public Expressions: Array<LinqlExpression> | undefined;

    constructor(public Type: LinqlType)
    {
    }

    Merge(SearchToMerge: LinqlSearch)
    {
        this.Expressions = this.Expressions?.map(r => r.Clone());
        const lastExpression = this.Expressions?.FirstOrDefault()?.GetLastExpressionInNextChain();

        if (lastExpression)
        {
            lastExpression.Next = SearchToMerge.Expressions?.FirstOrDefault()?.Next?.Clone();
        }
    }
}