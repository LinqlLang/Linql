import { LinqlSearch } from "linql.client";
import { LinqlContext } from "linql.client-angular";

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

}