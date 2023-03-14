import { LinqlExpression } from "./LinqlExpression";

export class LinqlUnary extends LinqlExpression
{
    "$type": string = "LinqlUnary";
    UnaryName: string;
    Arguments: Array<LinqlExpression> | undefined;

    constructor(UnaryName: string, Arguments: Array<LinqlExpression> | undefined = undefined)
    {
        super();
        this.UnaryName = UnaryName;
        this.Arguments = Arguments;
    }

    public Clone(): this
    {
        const unary = new LinqlUnary(this.UnaryName, this.Arguments?.map(r => r.Clone()));
        unary.Next = this.Next?.Clone();
        return unary as this;
    }

}