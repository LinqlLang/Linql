import { LinqlExpression } from "./LinqlExpression";
import { LinqlType } from "./LinqlType";

export class LinqlConstant extends LinqlExpression
{
    "$type": string = "LinqlConstant";

    ConstantType: LinqlType | undefined;

    Value: any;

    constructor(ConstantType: LinqlType, Value: any)
    {
        super();
        this.ConstantType = ConstantType;
        this.Value = Value;
    }
}