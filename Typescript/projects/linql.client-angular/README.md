# Linql.Client-Angular

Provides native linql client support for angular.  
## Example Usage

```typescript
const search = this.customContext.Set<State>(State, { this: this });
search.Where(r => r.State_Code!.Contains("A")).ToListAsyncSearch();
```
## Getting Started
### Requirements
- Angular 14+

> If using npm < 8, dependant packages may need to be installed manually.
#### Installation

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

### Advanced Concepts

#### TypeName resolution 

When an `object` is used within a LinqlSearch, the LinqlContext's `GetTypeName` function is used to resolve the name of that object.  By default, `GetTypeName` implentation checks for the existance of a static `Type` member on the constructor.  If it does not exist, it chooses the `name` of the constructor.   

The static `Type` member, or some custom implementation, **is required to support minified code**, as **class names are generally clobbered** during the 

#### **`ModelExample.ts`**
```typescript
export class State
{
  //Default way to handle code miniification 
  static Type = "State";
  FID!: number;
  Program!: string;
}
```

#### Custom LinqlContext

Overriding the default LinqlContext can provide developers to customize endpoint generation, authentication, and type name resolution.  To do so, simply create your custom implementation, and provide it in your app module.

#### **`CustomLinqlContext.ts`**
```typescript
export class CustomLinqlContext extends LinqlContext
{
    override GetRoute(Search: LinqlSearch<any>)
    {
        if (Search.Type.TypeName)
        {
            return Search.Type.TypeName;
        }
        return "";
    }
    async Batch<T>(...values: Array<T>): Promise<Array<T>>
    {
        const searches = values.Where(r => (r as Object).constructor === this.LinqlSearchType) as Array<ALinqlSearch<any>>;
        const sanitizedSearches = searches.map(r => this.GetOptimizedSearch(r));
        const results = await lastValueFrom(this.Client.post(`${ this.BaseUrl }Batch`, sanitizedSearches));
        return results as Array<T>;
    }

    override GetTypeName(Type: string | GenericConstructor<any>): string
    {
        if (typeof Type !== "string")
        {
            const anyCast = Type as any;
            const type = anyCast["Type"];

            if (type)
            {
                return type;
            }
        }
        return super.GetTypeName(Type);
    }

}
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

