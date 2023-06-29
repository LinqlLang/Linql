from src.linql_core.LinqlType import LinqlType
from src.linql_core.LinqlProperty import LinqlProperty
from .CustomTypeNameProvider import CustomTypeNameProvider

class TestLinqlProperty:

   def test_Constructor(self):
      prop = LinqlProperty("r")
      assert prop.PropertyName == "r"


   def test_Clone(self):
      prop = LinqlProperty("r")
      clone = prop.Clone()
      assert prop.PropertyName == clone.PropertyName
