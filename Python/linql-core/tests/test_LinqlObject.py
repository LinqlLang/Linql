from src.linql_core.LinqlType import LinqlType
from src.linql_core.LinqlConstant import LinqlConstant
from src.linql_core.LinqlObject import LinqlObject
from .CustomTypeNameProvider import CustomTypeNameProvider

class TestLinqlObject:

   typeNameProvider = CustomTypeNameProvider()

   def createObject(self, Value) -> LinqlObject:
      return LinqlObject(Value, LinqlType.GetLinqlType(Value, self.typeNameProvider))

   def test_Constructor(self):
      obj: LinqlObject[int] = self.createObject(5)
      assert obj.Type.TypeName == "Int32"


   def test_Clone(self):
      obj = self.createObject(4)
      clone = obj.Clone()
      assert obj.Value == clone.Value

   def test_Serialization(self):
      obj: LinqlObject[int] = self.createObject(5)
      serial = obj.toSerializable()
      assert "$type" in serial
      assert "Type" in serial
      assert "Value" in serial 
      assert "Next" not in serial
