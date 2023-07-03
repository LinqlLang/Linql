from typing_extensions import Self
from .LinqlExpression import LinqlExpression
from .LinqlType import LinqlType
from typing import Any

class LinqlProperty(LinqlExpression):
    
    type: str = "LinqlProperty"
    PropertyName: str

    def __init__(self, PropertyName: str):
        self.PropertyName = PropertyName
    
    def Clone(self) -> Self:
        prop = LinqlProperty(self.PropertyName)

        if self.Next != None:
            prop.Next = self.Next.Clone()
        return prop
    
    def toSerializable(self) -> dict:
        jsonObject = self._CreateSerializableType()
        
        jsonObject["PropertyName"] = self.PropertyName

        self._SerializeNext(jsonObject)
        return jsonObject