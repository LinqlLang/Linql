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
        constant = Self(self.ConstantType, self.Value)
        constant.Next = self.Next.Clone()
        return constant