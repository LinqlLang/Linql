import { LinqlExpression } from "./LinqlExpression";

export class LinqlProperty extends LinqlExpression
{
    "$type": string = "LinqlParameter";

    constructor(public PropertyName: string)
    {
        super();
    }

}