import { LinqlSearch, ALinqlSearch } from "linql.client";
import { LinqlContext } from "linql.client-angular";
import { GenericConstructor } from "linql.core";
import { lastValueFrom } from "rxjs";
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