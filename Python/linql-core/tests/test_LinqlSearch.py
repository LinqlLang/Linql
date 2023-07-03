from src.linql_core.LinqlType import LinqlType
from src.linql_core.LinqlSearch import LinqlSearch
from .TestClass import TestClass
from .CustomTypeNameProvider import CustomTypeNameProvider

class TestLinqlSearch:

   typeNameProvider = CustomTypeNameProvider()

   def test_Constructor(self):
      testClass = TestClass()
      search = LinqlSearch(LinqlType.GetLinqlType(testClass, self.typeNameProvider))
      assert search.Type.TypeName == "TestClass"

   def test_Serialization(self):
      testClass = TestClass()
      search = LinqlSearch(LinqlType.GetLinqlType(testClass, self.typeNameProvider))
      serial = search.toSerializable()
      assert "Type" in serial
      assert "Value" not in serial
