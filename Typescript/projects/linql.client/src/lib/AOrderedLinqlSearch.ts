import { TransformExpression } from "linql.core";
import { ALinqlSearch } from "./ALinqlSearch";

export abstract class AOrderedLinqlSearch<T> extends ALinqlSearch<T>
{
    public thenByDescending<S>(Expression: TransformExpression<T, S>)
    {
        return this.CustomLinqlFunction<T>("ThenByDescending", Expression);
    }

    public thenBy<S>(Expression: TransformExpression<T, S>)
    {
        return this.CustomLinqlFunction<T>("ThenBy", Expression);
    }
}