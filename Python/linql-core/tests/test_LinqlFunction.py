from src.linql_core.LinqlType import LinqlType
from src.linql_core.LinqlFunction import LinqlFunction
from src.linql_core.LinqlParameter import LinqlParameter
from .CustomTypeNameProvider import CustomTypeNameProvider

class TestLinqlFunction:

   def test_Constructor(self):
      fun = LinqlFunction("Where", [LinqlParameter("r")])
      assert fun.FunctionName == "Where" and len(fun.Arguments) == 1


   def test_Clone(self):
      fun = LinqlFunction("Where", [LinqlParameter("r")])
      clone = fun.Clone()
      assert fun.FunctionName == clone.FunctionName and len(fun.Arguments) == len(clone.Arguments)

   def test_Serialization(self):
      fun = LinqlFunction("Where", [LinqlParameter("r")])
      serial = fun.toSerializable()
      assert "$type" in serial
      assert "FunctionName" in serial
      assert "Arguments" in serial 
      assert "Next" not in serial
