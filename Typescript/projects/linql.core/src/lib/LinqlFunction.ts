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

    public Clone(): this
    {
        const linqlFunction = new LinqlFunction(this.FunctionName, this.Arguments?.map(r => r.Clone()));
        linqlFunction.Next = this.Next?.Clone();
        return linqlFunction as this;
    }

}