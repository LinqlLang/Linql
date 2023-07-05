from typing_extensions import Self
from .LinqlExpression import LinqlExpression
from .LinqlType import LinqlType
from typing import Any, TypeVar, Generic

T = TypeVar('T')

class LinqlObject(LinqlExpression, Generic[T]):
    
    type: str = "LinqlObject"
    Type: LinqlType
    Value: T

    def __init__(self, Value: T, Type: LinqlType):
        self.Value = Value
        self.Type = Type
    
    def Clone(self) -> Self:
        obj = LinqlObject(self.Value, self.Type)
        if self.Next != None:
            obj.Next = self.Next.Clone()

        return obj
    
    def toSerializable(self) -> dict:
        jsonObject = self._CreateSerializableType()
        
        jsonObject["Type"] = self.Type.toSerializable()

        if hasattr(self, "Value"):
            if hasattr(self.Value, "toSerializable"):
                jsonObject["Value"] = self.Value.toSerializable()
            else:
                jsonObject["Value"] = self.Value

        self._SerializeNext(jsonObject)
        return jsonObject