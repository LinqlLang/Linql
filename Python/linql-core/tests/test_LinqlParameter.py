from src.linql_core.LinqlType import LinqlType
from src.linql_core.LinqlParameter import LinqlParameter
from .CustomTypeNameProvider import CustomTypeNameProvider

class TestLinqlParameter:

   def test_Constructor(self):
      param = LinqlParameter("r")
      assert param.ParameterName == "r"


   def test_Clone(self):
      param = LinqlParameter("r")
      clone = param.Clone()
      assert param.ParameterName == clone.ParameterName

   def test_Serialization(self):
      param = LinqlParameter("r")
      serial = param.toSerializable()
      assert "$type" in serial
      assert "ParameterName" in serial
      assert "Next" not in serial
