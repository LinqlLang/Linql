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

    public Clone(): this
    {
        const clone = new LinqlProperty(this.PropertyName);
        clone.Next = this.Next?.Clone();
        return clone as this;
    }

}