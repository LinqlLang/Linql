from typing_extensions import Self
from .LinqlExpression import LinqlExpression
from .LinqlType import LinqlType
from typing import Any

class LinqlParameter(LinqlExpression):
    
    type: str = "LinqlParameter"
    ParameterName: str

    def __init__(self, ParameterName: str):
        self.ParameterName = ParameterName
    
    def Clone(self) -> Self:
        param = LinqlParameter(self.ParameterName)

        if self.Next != None:
            param.Next = self.Next.Clone()
        return param
    
    def toSerializable(self) -> dict:
        jsonObject = self._CreateSerializableType()
        
        jsonObject["ParameterName"] = self.ParameterName

        self._SerializeNext(jsonObject)
        return jsonObject