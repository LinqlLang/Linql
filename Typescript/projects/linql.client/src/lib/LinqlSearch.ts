import { LinqlConstant, LinqlExpression, LinqlFunction, LinqlType } from "linql.core";
import { ALinqlContext, ALinqlSearch, LinqlSearchConstructor } from "./ALinqlSearch";
import { IGrouping } from "./IGrouping";
import { LinqlContext } from "./LinqlContext"
import { AnyExpression, BooleanExpression, GenericConstructor, TransformExpression } from "./Types";

export class LinqlSearch<T> extends ALinqlSearch<T>
{
    constructor(
        ModelType: string | GenericConstructor<T>,
        ArgumentContext: any = {},
        Context: ALinqlContext = new LinqlContext(LinqlSearch as any as LinqlSearchConstructor<T>, ArgumentContext)
    )
    {
        super(ModelType, ArgumentContext, Context);
    }

    public Copy(): this
    {
        const search = new LinqlSearch<T>(this.ModelType, this.ArgumentContext, this.Context);

        if (this.Expressions)
        {
            search.Expressions = Array.from(this.Expressions);
        }

        return search as this;
    }


}

