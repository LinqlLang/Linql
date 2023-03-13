import { HttpClientModule } from "@angular/common/http";
import { ModuleWithProviders, NgModule } from '@angular/core';
import { LinqlSearchConstructor } from "linql.client";
import { LinqlContext } from "./linql.client-angular.service";
import { LinqlSearchType, BaseUrl } from "./Tokens";


@NgModule({
  declarations: [

  ],
  imports: [
    HttpClientModule
  ],
  exports: [

  ]
})
export class LinqlClientAngularModule
{
  static forRoot(LinqlSearchType: LinqlSearchConstructor<any>, BaseUrl: string): ModuleWithProviders<LinqlClientAngularModule>
  {
    return {
      ngModule: LinqlClientAngularModule,
      providers: [
        { provide: LinqlSearchType, useValue: LinqlSearchType },
        { provide: BaseUrl, useValue: BaseUrl },
        LinqlContext
      ]
    };
  }
}
