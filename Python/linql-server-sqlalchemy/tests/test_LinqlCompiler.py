from .FileLoader import FileLoader
from typing import Self
from src.linql_server_sqlalchemy.LinqlCompiler import LinqlCompiler
from linql_client.LinqlContext import LinqlContext
from linql_client.LinqlSearch import LinqlSearch
import tests.Building as Building
import json
import pytest

compiler = LinqlCompiler({
   Building
})

context = LinqlContext(LinqlSearch, "")

class TestLinqlCompiler:

   @pytest.mark.asyncio
   async def test_EmptySearch(self):
     set: LinqlSearch[Building.Building] = context.Set(Building.Building)
     emptySearch = set.ToListAsyncSearch().toJson()
     reloadSearch: LinqlSearch = json.loads(emptySearch)
     await compiler.ExecuteAsync(reloadSearch, None)
     assert True