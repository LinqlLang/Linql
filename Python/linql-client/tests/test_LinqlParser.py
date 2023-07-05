from linql_core.LinqlType import LinqlType
from linql_core.LinqlObject import LinqlObject
from src.linql_client.ALinqlSearch import ALinqlSearch
from src.linql_client.LinqlParser import LinqlParser
from src.linql_client.LinqlContext import LinqlContext
from src.linql_client.LinqlSearch import LinqlSearch
from .FileLoader import FileLoader
import inspect
from typing import Self

class NullableModel:
   Integer: int | None

   def toSerializable(self) -> dict:
      pass


class DataModel:
   Boolean: bool
   String: str
   OneToOne: Self
   Integer: int 
   ListInteger: list[int]
   ListString: list[str]
   ListRecusrive: list[Self]
   ListNullableModel: list[NullableModel]
   OneToOneNullable: NullableModel

   def toSerializable(self) -> dict:
      return self.__dict__



context = LinqlContext(LinqlSearch, "")
testLoader = FileLoader("../C#/Test/Linql.Test.Files/TestFiles/Smoke")
complex = DataModel()
complex.Boolean = False

def _CreateDataModel(Integer: int | None = None):
   testData = DataModel()

   if Integer != None:
      testData.Integer = Integer

   testData.String = ""
   testData.ListInteger = []
   testData.ListString = []
   testData.ListRecusrive = []
   testData.ListNullableModel = []
   return testData

class TestLinqlParser:

   def test_EmptySearch(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      testLoader.ExecuteTest(search)

   def test_SimpleConstant(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: True)
      testLoader.ExecuteTest(newSearch)

   def test_SimpleBooleanProperty(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: r.Boolean)
      testLoader.ExecuteTest(newSearch)

   def test_BooleanNegate(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: not r.Boolean)
      testLoader.ExecuteTest(newSearch)

   def test_SimpleBooleanPropertyChaining(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: r.OneToOne.Boolean)
      testLoader.ExecuteTest(newSearch)

   def test_SimpleBooleanPropertyEqualsSwap(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: False == r.Boolean)
      testLoader.ExecuteTest(newSearch)

   def test_BooleanVar(self):
      test = False
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: False == test)
      testLoader.ExecuteTest(newSearch)

   def test_ComplexBoolean(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: complex.Boolean)
      testLoader.ExecuteTest(newSearch)

   def test_ThreeBooleans(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: r.Boolean and r.Boolean and r.Boolean)
      testLoader.ExecuteTest(newSearch)

   def test_ListInt(self):
      integers = [1, 2, 3]
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: r.Integer in integers)
      testLoader.ExecuteTest(newSearch)

   def test_ListIntFromProperty(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: 1 in r.ListInteger)
      testLoader.ExecuteTest(newSearch)

   def test_InnerLambda(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: any(lambda s: s == 1, r.ListInteger))
      testLoader.ExecuteTest(newSearch)

   def test_NullableHasValue(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: r.OneToOneNullable.Integer != None)
      testLoader.ExecuteTest(newSearch)

   def test_NullableHasValueReversed(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: None != r.OneToOneNullable.Integer)
      testLoader.ExecuteTest(newSearch, "test_NullableHasValue")

   def test_NullableValue(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: r.OneToOneNullable.Integer != None and r.OneToOneNullable.Integer.Value == 1)
      testLoader.ExecuteTest(newSearch)

   def test_LinqlObject(self):
      value = _CreateDataModel()
      type = LinqlType.GetLinqlType(value, context)
      objectTest = LinqlObject(value, type)
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: objectTest.Value.Integer == r.Integer)
      testLoader.ExecuteTest(newSearch)
