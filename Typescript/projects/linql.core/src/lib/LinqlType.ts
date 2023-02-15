export class LinqlType
{
    static readonly ListType: string = "List";

    public TypeName: string | undefined;

    public GenericParameters: Array<LinqlType> | undefined;

    public IsList(): boolean
    {
        return this.TypeName == LinqlType.ListType;
    }

    static GetLinqlType(Value: any)
    {
        const type = new LinqlType();

        switch (typeof Value)
        {
            case "string":
                type.TypeName = "String";
                break;
            case "bigint":
                type.TypeName = "Long";
                break;
            case "boolean":
                type.TypeName = "Boolean"
                break;
            case "number":
                type.TypeName = "Int32";
                break;
            case "object":
            case "function":
            case "symbol":
            case "undefined":
                console.error("Unsupported type passed to VisitContant.");
                break;
        }

        return type;
    }
}