import { LinqlExpression } from "./LinqlExpression";

export class LinqlUnary extends LinqlExpression
{
    "$type": string = "LinqlUnary";

    constructor(public UnaryName: string, public Arguments: Array<LinqlExpression> | undefined = undefined)
    {
        super();
    }

}