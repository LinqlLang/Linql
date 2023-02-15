import { LinqlExpression } from "./LinqlExpression";

export class LinqlParameter extends LinqlExpression
{
    "$type": string = "LinqlParameter";
    ParameterName: string;
    constructor(ParameterName: string)
    {
        super();
        this.ParameterName = ParameterName;
    }

}