export abstract class LinqlExpression
{
    abstract "$type": string;

    public Next: LinqlExpression | undefined;

    public GetLastExpressionInNextChain(): LinqlExpression
    {
        if (!this.Next)
        {
            return this;
        }
        return this.Next.GetLastExpressionInNextChain();
    }

    public abstract Clone(): this;
}