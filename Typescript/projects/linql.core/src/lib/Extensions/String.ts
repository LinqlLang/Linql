import { IGrouping } from "./IGrouping";
import { BooleanExpression, AnyExpression, OneToManyExpression } from "./Types";

declare global
{
    export interface String
    {
        CompareTo(Value: string): number;
        Concat(Value: string): string;
        Contains(Value: string): boolean;
        EndsWith(Value: string): boolean;
        Equals(Value: string): boolean;
        IndexOf(Value: string): number;
        Insert(startIndex: number, Value: string): string;
        LastIndexOf(Value: string): number;
        Replace(oldValue: string, newValue: string): string;
        Split(separator: string): Array<string>;
        StartsWith(Value: string): boolean;
        Substring(startIndex: number, endIndex?: number): string;
        ToLower(): string;
        ToLowerInvariant(): string;
        ToUpper(): string;
        ToUpperInvariant(): string;
        Trim(): string;
        TrimStart(): string;
        TrimEnd(): string;

    }
}

String.prototype.CompareTo = function (Value: string)
{
    return this.indexOf(Value);
};

String.prototype.Concat = function (Value: string)
{
    return this.concat(Value);
};

String.prototype.Contains = function (Value: string)
{
    return this.includes(Value);
};

String.prototype.EndsWith = function (Value: string)
{
    return this.endsWith(Value);
};

String.prototype.Equals = function (Value: string)
{
    return this === Value;
};

String.prototype.IndexOf = function (Value: string)
{
    return this.indexOf(Value);
};

String.prototype.Insert = function (startIndex: number, Value: string)
{
    return `${ this.substring(0, startIndex) }${ Value }${ this.substring(startIndex) }`;
};

String.prototype.LastIndexOf = function (Value: string)
{
    return this.lastIndexOf(Value);
};

String.prototype.Replace = function (oldValue: string, newValue: string)
{
    return this.replace(oldValue, newValue);
};

String.prototype.Split = function (separator: string)
{
    return this.split(separator);
};

String.prototype.StartsWith = function (Value: string)
{
    return this.startsWith(Value);
};

String.prototype.Substring = function (startIndex: number, endIndex?: number)
{
    return this.substring(startIndex, endIndex);
};

String.prototype.ToLower = function ()
{
    return this.toLowerCase();
};

String.prototype.ToLowerInvariant = function ()
{
    return this.toLocaleLowerCase();
};

String.prototype.ToUpper = function ()
{
    return this.toUpperCase();
};

String.prototype.ToUpperInvariant = function ()
{
    return this.toLocaleUpperCase();
};

String.prototype.Trim = function ()
{
    return this.trim();
};

String.prototype.TrimStart = function ()
{
    return this.trimStart();
};

String.prototype.TrimEnd = function ()
{
    return this.trimEnd();
};

export default {};