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

    public Clone(): this
    {
        const param = new LinqlParameter(this.ParameterName);
        param.Next = this.Next?.Clone();
        return param as this;
    }

}