import { ITypeNameProvider } from "./ITypeNameProvider";
import { LinqlExpression } from "./LinqlExpression";
import { LinqlType } from "./LinqlType";

export class LinqlObject<T> extends LinqlExpression
{
    "$type": string = "LinqlObject";
    Type: LinqlType;
    Value: T;

    constructor(Value: T, Type: LinqlType)
    {
        super();
        this.Type = Type;
        this.Value = Value;
    }

    public Clone(): this
    {
        const obj = new LinqlObject(this.Value, this.Type);
        obj.Next = this.Next?.Clone();
        return obj as this;
    }

}