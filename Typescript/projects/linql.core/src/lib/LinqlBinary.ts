import { LinqlExpression } from "./LinqlExpression";

export class LinqlBinary extends LinqlExpression
{
    "$type": string = "LinqlBinary";

    constructor(public BinaryName: string, public Left: LinqlExpression | undefined = undefined, public Right: LinqlExpression | undefined = undefined)
    {
        super();

    }
}