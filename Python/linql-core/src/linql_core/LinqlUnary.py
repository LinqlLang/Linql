from typing_extensions import Self
from .LinqlExpression import LinqlExpression
from .LinqlType import LinqlType
from typing import Any, TypeVar, Generic

class LinqlUnary(LinqlExpression):
    
    type: str = "LinqlUnary"
    UnaryName: str
    Arguments: list[LinqlExpression] | None

    def __init__(self, UnaryName: str, Arguments: list[LinqlExpression] | None):
        self.UnaryName = UnaryName
        self.Arguments = Arguments
    
    def Clone(self) -> Self:
        fun = LinqlUnary(self.UnaryName, list(map(lambda x: x.Clone(), self.Arguments)))
        if self.Next != None:
            fun.Next = self.Next.Clone()

        return fun
    
    def toSerializable(self) -> dict:
        jsonObject = self._CreateSerializableType()
        
        jsonObject["UnaryName"] = self.UnaryName

        if hasattr(self, "Arguments") and self.Arguments != None and len(self.Arguments) > 0:
            jsonObject["Arguments"] = list(map(lambda x: x.toSerializable(), self.Arguments))

        self._SerializeNext(jsonObject)
        return jsonObject