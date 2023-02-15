import { LinqlExpression } from "./LinqlExpression";

export class LinqlProperty extends LinqlExpression
{
    "$type": string = "LinqlProperty";
    PropertyName: string;
    constructor(PropertyName: string)
    {
        super();
        this.PropertyName = PropertyName;
    }

}