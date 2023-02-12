export class LinqlType
{
    static readonly ListType: string = "List";

    public TypeName: string | undefined;

    public GenericParameters: Array<LinqlType> | undefined;

    public IsList(): boolean
    {
        return this.TypeName == LinqlType.ListType;
    }
}