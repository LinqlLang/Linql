from linql_core.LinqlParameter import LinqlParameter
from src.linql_client.ALinqlSearch import ALinqlSearch
from src.linql_client.LinqlParser import LinqlParser
from src.linql_client.LinqlContext import LinqlContext
from src.linql_client.LinqlSearch import LinqlSearch
from .FileLoader import FileLoader

class DataModel: 
   pass

context = LinqlContext(LinqlSearch, "")
testLoader = FileLoader("../../../C#/Test/Linql.Test.Files/TestFiles")

class TestLinqlParser:

   def test_SimpleConstant(self):
      search: ALinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: True)
      testLoader.ExecuteTest(newSearch)
      assert True
