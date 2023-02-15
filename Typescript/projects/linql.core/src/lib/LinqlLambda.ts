import { LinqlExpression } from "./LinqlExpression";
import { LinqlType } from "./LinqlType";

export class LinqlLambda extends LinqlExpression
{
    "$type": string = "LinqlLambda";

    Body: LinqlExpression | undefined;

    Parameters: Array<LinqlExpression> | undefined;

}