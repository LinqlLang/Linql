import { LinqlExpression } from "./LinqlExpression";
import { LinqlType } from "./LinqlType";

export class LinqlConstant extends LinqlExpression
{
    "$type": string = "LinqlConstant";

    ConstantType: LinqlType;

    Value: any;

    constructor(ConstantType: LinqlType, Value: any)
    {
        super();
        this.ConstantType = ConstantType;
        this.Value = Value;
    }

    public Clone(): this
    {
        const constant = new LinqlConstant(this.ConstantType, this.Value);
        constant.Next = this.Next?.Clone();
        return constant as this;
    }
}