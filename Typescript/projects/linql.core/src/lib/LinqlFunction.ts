import { LinqlExpression } from "./LinqlExpression";

export class LinqlFunction extends LinqlExpression
{
    "$type": string = "LinqlFunction";
    FunctionName: string;
    Arguments: Array<LinqlExpression> | undefined;

    constructor(FunctionName: string, Arguments: Array<LinqlExpression> | undefined = undefined)
    {
        super();
        this.FunctionName = FunctionName;
        this.Arguments = Arguments;
    }

}