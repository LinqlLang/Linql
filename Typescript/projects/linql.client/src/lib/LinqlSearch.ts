import { GenericConstructor } from "linql.core";
import { ALinqlContext, ALinqlSearch, LinqlSearchConstructor } from "./ALinqlSearch";
import { constructorType } from "./LinqlConstructorType";

export class LinqlSearch<T> extends ALinqlSearch<T>
{
    constructor(
        ModelType: string | GenericConstructor<T>,
        ArgumentContext: any = {},
        Context: ALinqlContext
    )
    {
        super(ModelType, ArgumentContext, Context);
    }

    public Copy(): this
    {
        const objCast = this as Object;
        const constructor: constructorType<T, this> = objCast.constructor as constructorType<T, this>;
        const search = new constructor(this.ModelType, this.ArgumentContext, this.Context)

        if (this.Expressions)
        {
            search.Expressions = this.Expressions.map(r => r.Clone());
        }

        return search as this;
    }


}

