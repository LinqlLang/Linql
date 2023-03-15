import { LinqlSearch, ALinqlSearch } from "linql.client";
import { LinqlContext } from "linql.client-angular";
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
    async Batch<T extends unknown[] | []>(values: T): Promise<{ -readonly [P in keyof T]: Awaited<T[P]> }>
    {
        debugger;
        const searches = values.Where(r => (r as Object).constructor === this.LinqlSearchType) as Array<ALinqlSearch<any>>;
        const sanitizedSearches = searches.map(r => this.GetOptimizedSearch(r));
        const results = await lastValueFrom(this.Client.post(`${ this.BaseUrl }Batch`, sanitizedSearches));
        return await Promise.all(values);
    }

}