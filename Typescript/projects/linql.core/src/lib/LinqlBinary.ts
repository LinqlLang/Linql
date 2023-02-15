import { LinqlExpression } from "./LinqlExpression";

export class LinqlBinary extends LinqlExpression
{
    "$type": string = "LinqlBinary";
    BinaryName: string;
    Left: LinqlExpression | undefined;
    Right: LinqlExpression | undefined;

    constructor(BinaryName: string, Left: LinqlExpression | undefined = undefined, Right: LinqlExpression | undefined = undefined)
    {
        super();
        this.BinaryName = BinaryName;
        this.Left = Left;
        this.Right = Right;

    }
}