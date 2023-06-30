from src.linql_core.LinqlType import LinqlType
from src.linql_core.LinqlSearch import LinqlSearch
from .TestClass import TestClass
from .CustomTypeNameProvider import CustomTypeNameProvider

class TestLinqlProperty:

   typeNameProvider = CustomTypeNameProvider()

   def test_Constructor(self):
      testClass = TestClass()
      search = LinqlSearch(LinqlType.GetLinqlType(testClass, self.typeNameProvider))
      assert search.Type.TypeName == "TestClass"
