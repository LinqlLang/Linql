
export declare type BooleanExpression<T> = (r: T) => boolean;
export declare type AnyExpression<T> = (r: T) => any;
export declare type TransformExpression<T, S> = (r: T) => S;
export declare type GenericConstructor<T> = new () => T;