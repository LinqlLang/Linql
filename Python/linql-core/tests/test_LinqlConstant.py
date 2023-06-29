from src.linql_core.LinqlType import LinqlType
from src.linql_core.LinqlConstant import LinqlConstant
from .CustomTypeNameProvider import CustomTypeNameProvider

class TestLinqlConstant:

   typeNameProvider = CustomTypeNameProvider()

   def test_Constructor(self):
      value = 'test'
      type = LinqlType.GetLinqlType(value, self.typeNameProvider)
      constant = LinqlConstant(type, value)
      assert constant.ConstantType.TypeName == "String"


   def test_Clone(self):
      value = 'test'
      type = LinqlType.GetLinqlType(value, self.typeNameProvider)
      constant = LinqlConstant(type, value)
      clone = constant.Clone()
      assert clone.ConstantType.TypeName == constant.ConstantType.TypeName and clone.Value == constant.Value
