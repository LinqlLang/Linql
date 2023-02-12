import { LinqlExpression } from "./LinqlExpression";
import { LinqlType } from "./LinqlType";

export class LinqlSearch 
{

    public Expressions: Array<LinqlExpression> | undefined;

    constructor(public Type: LinqlType)
    {
    }

}