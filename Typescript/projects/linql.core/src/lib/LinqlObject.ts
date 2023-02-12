import { LinqlExpression } from "./LinqlExpression";
import { LinqlType } from "./LinqlType";

export class LinqlObject extends LinqlExpression
{
    "$type": string = "LinqlObject";

    constructor(public Type: LinqlType, public Value: any)
    {
        super();
    }

}