import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { LinqlClientAngularModule, LinqlContext } from 'linql.client-angular';
import { HttpClient, HttpClientModule } from "@angular/common/http";
import { LinqlSearch } from "linql.client";

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { CustomLinqlContext } from './CustomLinqlContext';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    LinqlClientAngularModule.forRoot("https://localhost:7113/")
  ],
  providers: [
    HttpClient,
    {
      provide: CustomLinqlContext,
      deps: [HttpClient],
      useFactory: (client: HttpClient) => new CustomLinqlContext("https://localhost:7113/", client)
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
