import { TransformExpression } from "linql.core";
import { ALinqlSearch } from "./ALinqlSearch";

export abstract class AOrderedLinqlSearch<T> extends ALinqlSearch<T>
{
    public ThenByDescending<S>(Expression: TransformExpression<T, S> | string)
    {
        return this.CustomLinqlFunction<T>("ThenByDescending", Expression);
    }

    public ThenBy<S>(Expression: TransformExpression<T, S> | string)
    {
        return this.CustomLinqlFunction<T>("ThenBy", Expression);
    }
}