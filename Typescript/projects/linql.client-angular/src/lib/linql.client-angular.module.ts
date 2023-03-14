import { HttpClient, HttpClientModule } from "@angular/common/http";
import { ModuleWithProviders, NgModule } from '@angular/core';
import { LinqlSearch, LinqlSearchConstructor } from "linql.client";
import { LinqlContext } from "./linql.client-angular.service";
import { LinqlSearchType as LinqlTypeToken, BaseUrl as BaseUrlToken } from "./Tokens";


@NgModule({
  declarations: [

  ],
  imports: [
    HttpClientModule
  ],
  exports: [

  ],
  providers: [

  ]
})
export class LinqlClientAngularModule
{
  static forRoot(BaseUrl: string, LinqlSearchType: LinqlSearchConstructor<any> = LinqlSearch): ModuleWithProviders<LinqlClientAngularModule>
  {
    return {
      ngModule: LinqlClientAngularModule,
      providers: [
        { provide: LinqlContext, deps: [HttpClient], useFactory: (client: HttpClient) => new LinqlContext(BaseUrl, client, LinqlSearchType) },
      ]
    };
  }
}
