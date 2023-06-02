# Linql.Client-Angular

Provides native linql client support for angular.  

[Linql Overview]("../../../../README.md)

[Angular example](../../examples/angular/)
## Example Usage

```typescript
const search = this.customContext.Set<State>(State, { this: this });
search.Where(r => r.State_Code!.Contains("A")).ToListAsyncSearch();
```
## Getting Started
### Requirements
- Angular 14+

> If using npm < 8, dependant packages may need to be installed manually.
### Installation

```bash
npm i linql.client-angular
```

#### **`app.module.ts`**
```typescript
import { LinqlClientAngularModule } from 'linql.client-angular';
...
@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    LinqlClientAngularModule.forRoot("https://localhost:7113/")
  ],
  providers: [
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
```

#### **`app.component.ts`**
```typescript
@Component({
  selector: 'app-component',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  changeDetectionStrategy: ChangeDetectionStrategy.OnPush
})
export class AppComponent implements OnInit
{
  StateData: Array<State> | undefined;

  constructor(public context: LinqlContext, public CD: ChangeDetectorRef) {}

  async ngOnInit()
  {
    const search = this.customContext.Set<State>(State, { this: this });
    this.StateData = await search.Where(r => r.State_Code!.Contains("A")).ToListAsync();
    this.CD.markForCheck();
  }
}
```
## Custom LinqlContext

Create a `CustomLinqlContext` allows developers to customize endpoint generation, authentication, and type name resolution. 

Implement a [CustomLinqlContext](../../README.md#custom-linqlcontext).

Provide it in your module:

#### **`app.module.ts`**
```typescript
import { LinqlClientAngularModule } from 'linql.client-angular';
...
@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
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
```
