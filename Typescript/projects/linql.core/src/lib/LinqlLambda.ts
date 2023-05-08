import { LinqlExpression } from "./LinqlExpression";
import { LinqlType } from "./LinqlType";

export class LinqlLambda extends LinqlExpression
{

    "$type": string = "LinqlLambda";

    Body: LinqlExpression | undefined;

    Parameters: Array<LinqlExpression> | undefined;

    public Clone(): this
    {
        const clone = new LinqlLambda();
        clone.Next = this.Next?.Clone();
        clone.Parameters = this.Parameters?.map(r => r.Clone());
        clone.Body = this.Body?.Clone();
        return clone as this;
    }

}