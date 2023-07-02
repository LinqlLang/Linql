from linql_core.LinqlParameter import LinqlParameter
from src.linql_client.ALinqlSearch import ALinqlSearch
from src.linql_client.LinqlParser import LinqlParser
from src.linql_client.LinqlContext import LinqlContext
from src.linql_client.LinqlSearch import LinqlSearch

class DataModel: 
   pass

context = LinqlContext(LinqlSearch, "")

class TestLinqlParameter:

   def test_SimpleConstant(self):
      search: ALinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: True)
      assert True
