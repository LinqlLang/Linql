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

    public Clone(): this
    {
        const binary = new LinqlBinary(this.BinaryName, this.Left?.Clone(), this.Right?.Clone());
        binary.Next = this.Next?.Clone();
        return binary as this;
    }
}