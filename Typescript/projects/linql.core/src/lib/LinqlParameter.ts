import { LinqlExpression } from "./LinqlExpression";

export class LinqlParameter extends LinqlExpression
{
    "$type": string = "LinqlParameter";

    constructor(public ParameterName: string)
    {
        super();
    }

}