import { GenericConstructor } from "linql.core";
import { ALinqlContext } from "./ALinqlSearch";

export type constructorType<T, LinqlType> = new (ModelType: string | GenericConstructor<T>, ArgumentContext: any, Context: ALinqlContext) => LinqlType
