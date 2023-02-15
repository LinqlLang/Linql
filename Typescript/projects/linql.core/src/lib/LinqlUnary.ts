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

}