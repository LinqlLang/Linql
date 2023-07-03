from typing_extensions import Self
from .LinqlExpression import LinqlExpression
from .LinqlType import LinqlType
from typing import Any, TypeVar, Generic

class LinqlFunction(LinqlExpression):
    
    type: str = "LinqlFunction"
    FunctionName: str
    Arguments: list[LinqlExpression] | None

    def __init__(self, FunctionName: str, Arguments: list[LinqlExpression] | None):
        self.FunctionName = FunctionName
        self.Arguments = Arguments
    
    def Clone(self) -> Self:
        fun = LinqlFunction(self.FunctionName, list(map(lambda x: x.Clone(), self.Arguments)))
        if self.Next != None:
            fun.Next = self.Next.Clone()

        return fun
    
    def toSerializable(self) -> dict:
        jsonObject = self._CreateSerializableType()
        
        jsonObject["FunctionName"] = self.FunctionName

        if hasattr(self, "Arguments") and len(self.Arguments) > 0:
            jsonObject["Arguments"] = list(map(lambda x: x.toSerializable(), self.Arguments))

        self._SerializeNext(jsonObject)
        return jsonObject