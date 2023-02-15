declare global
{
    export interface Array<T>
    {
        Contains<T>(ItemToCompare: T): boolean;
    }
}
Array.prototype.Contains = function <T>(ItemToCompare: T | undefined)
{
    if (!ItemToCompare)
    {
        return false;
    }
    else
    {
        return this.indexOf(ItemToCompare) > -1;
    }
};

export { };