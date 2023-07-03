from src.linql_core.LinqlType import LinqlType
from src.linql_core.LinqlConstant import LinqlConstant
from src.linql_core.LinqlBinary import LinqlBinary
from .CustomTypeNameProvider import CustomTypeNameProvider

class TestLinqlBinary:

   typeNameProvider = CustomTypeNameProvider()

   def createBinary(self, binaryName, leftValue, rightValue) -> LinqlBinary:
      left = LinqlConstant(LinqlType.GetLinqlType(leftValue, self.typeNameProvider), leftValue)
      right = LinqlConstant(LinqlType.GetLinqlType(rightValue, self.typeNameProvider), rightValue)
      binary = LinqlBinary(binaryName, left, right)
      return binary


   def test_Constructor(self):
      binary = self.createBinary("Equals", 4, 5)
      assert binary.BinaryName == "Equals" and binary.Left.Value == 4


   def test_Clone(self):
      binary = self.createBinary("Equals", 4, 5)
      clone = binary.Clone()
      assert binary.BinaryName == clone.BinaryName and binary.Left.Value == clone.Left.Value

   def test_Serialization(self):
      binary = self.createBinary("Equals", 4, 5)
      serial = binary.toSerializable()
      assert "$type" in serial
      assert "Left" in serial
      assert "Right" in serial 
      assert "Next" not in serial
