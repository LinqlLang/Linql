import { InjectionToken } from "@angular/core";
import { LinqlSearchConstructor } from "linql.client";

export const LinqlSearchType = new InjectionToken<LinqlSearchConstructor<any>>('Default LinqlSearch type');
export const BaseUrl = new InjectionToken<string>('BaseUrl of the server');
