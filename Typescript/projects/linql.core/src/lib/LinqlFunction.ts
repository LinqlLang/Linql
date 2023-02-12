import { LinqlExpression } from "./LinqlExpression";

export class LinqlFunction extends LinqlExpression
{
    "$type": string = "LinqlFunction";

    constructor(public FunctionName: string, public Arguments: Array<LinqlExpression> | undefined)
    {
        super();
    }

}