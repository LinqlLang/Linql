from linql_core.LinqlType import LinqlType
from linql_core.LinqlObject import LinqlObject
from src.linql_client.ALinqlSearch import ALinqlSearch
from src.linql_client.LinqlParser import LinqlParser
from src.linql_client.LinqlContext import LinqlContext
from src.linql_client.LinqlSearch import LinqlSearch
from .FileLoader import FileLoader
from typing import Self
from src.linql_client import selectMany

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

   def test_LinqlObject_NonZero(self):
      value = _CreateDataModel(1)
      type = LinqlType.GetLinqlType(value, context)
      objectTest = LinqlObject(value, type)
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: objectTest.Value.Integer == r.Integer)
      testLoader.ExecuteTest(newSearch)

   def test_List_Int_Contains(self):
      integers = [1, 2, 3]
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: r.Integer in integers)
      testLoader.ExecuteTest(newSearch)

   # Doesn't port exactly to c# since we can't tell the type of the list at compile time.  Fails in Typescript too I think
   # def test_EmptyList(self):
   #    integers = []
   #    search: LinqlSearch[DataModel] = context.Set(DataModel)
   #    newSearch = search.Where(lambda r: r.Integer in integers)
   #    testLoader.ExecuteTest(newSearch)

   def test_Inner_Lambda(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Where(lambda r: any(lambda s: 1 in s.ListInteger, r.ListRecusrive))
      testLoader.ExecuteTest(newSearch)

   def test_Select_Test(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Select(lambda r: r.Integer)
      testLoader.ExecuteTest(newSearch)

   def test_SelectMany(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.SelectMany(lambda r: r.ListInteger)
      testLoader.ExecuteTest(newSearch)

   def test_SelectManyDouble(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.SelectMany(lambda r: selectMany(lambda s: s.ListInteger, r.ListRecusrive))
      testLoader.ExecuteTest(newSearch)

   def test_SkipTake(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Skip(1).Take(2).ToListAsyncSearch()
      testLoader.ExecuteTest(newSearch)
   
   def test_FirstOrDefault(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.FirstOrDefaultSearch()
      testLoader.ExecuteTest(newSearch)

   def test_FirstOrDefaultWithPredicate(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.FirstOrDefaultSearch(lambda r: r.Integer == 1)
      testLoader.ExecuteTest(newSearch)

   def test_LastOrDefault(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.LastOrDefaultSearch()
      testLoader.ExecuteTest(newSearch)

   def test_LastOrDefaultWithPredicate(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.LastOrDefaultSearch(lambda r: r.Integer == 1)
      testLoader.ExecuteTest(newSearch)

   def test_Lower(self):
      search: LinqlSearch[DataModel] = context.Set(DataModel)
      newSearch = search.Select(lambda r: r.String.lower())
      json = newSearch.ToListAsyncSearch().toJson()
      assert "ToLower" in json
