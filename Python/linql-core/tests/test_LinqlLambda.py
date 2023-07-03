from src.linql_core.LinqlType import LinqlType
from src.linql_core.LinqlLambda import LinqlLambda
from src.linql_core.LinqlParameter import LinqlParameter
from src.linql_core.LinqlConstant import LinqlConstant
from .CustomTypeNameProvider import CustomTypeNameProvider

class TestLinqlLambda:

   typeNameProvider = CustomTypeNameProvider()

   def test_Constructor(self):
      lam = LinqlLambda()
      lam.Body = LinqlConstant(LinqlType.GetLinqlType(False, self.typeNameProvider), False)
      lam.Parameters = [LinqlParameter("r")]
      assert lam.Body.Value == False and lam.Parameters[0].ParameterName == "r"


   def test_Clone(self):
      lam = LinqlLambda()
      lam.Body = LinqlConstant(LinqlType.GetLinqlType(False, self.typeNameProvider), False)
      lam.Parameters = [LinqlParameter("r")]      
      clone = lam.Clone()
      assert lam.Body.Value == clone.Body.Value and len(lam.Parameters) == len(clone.Parameters)

   def test_Serialization(self):
      lam = LinqlLambda()
      lam.Body = LinqlConstant(LinqlType.GetLinqlType(False, self.typeNameProvider), False)
      lam.Parameters = [LinqlParameter("r")]      
      serial = lam.toSerializable()
      assert "$type" in serial
      assert "Body" in serial
      assert "Parameters" in serial 
      assert "Next" not in serial
