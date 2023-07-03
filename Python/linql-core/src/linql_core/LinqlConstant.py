from typing_extensions import Self
from .LinqlExpression import LinqlExpression
from .LinqlType import LinqlType
from typing import Any

class LinqlConstant(LinqlExpression):
    
    type: str = "LinqlConstant"
    ConstantType: LinqlType
    Value: Any

    def __init__(self, ConstantType: LinqlType, Value: Any):
        self.ConstantType = ConstantType
        self.Value = Value
    
    def Clone(self) -> Self:
        constant = LinqlConstant(self.ConstantType, self.Value)

        if self.Next != None:
            constant.Next = self.Next.Clone()
        return constant
    
    def toSerializable(self) -> dict:
        jsonObject = self._CreateSerializableType()
        
        jsonObject["ConstantType"] = self.ConstantType.toSerializable()

        if hasattr(self, "Value") and self.Value != None:
            jsonObject["Value"] = self.Value

        self._SerializeNext(jsonObject)
        return jsonObject