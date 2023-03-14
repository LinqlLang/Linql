import { LinqlExpression } from "./LinqlExpression";
import { LinqlType } from "./LinqlType";

export class LinqlObject<T> extends LinqlExpression
{
    "$type": string = "LinqlObject";
    Type: LinqlType;
    Value: T;

    constructor(Value: T)
    {
        super();
        this.Type = LinqlType.GetLinqlType(Value);
        this.Value = Value;
    }

    public Clone(): this
    {
        const obj = new LinqlObject(this.Value);
        obj.Next = this.Next?.Clone();
        return obj as this;
    }

}