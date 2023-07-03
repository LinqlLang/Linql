from typing_extensions import Self
from .LinqlExpression import LinqlExpression
from .LinqlType import LinqlType
from typing import Any, TypeVar, Generic

class LinqlLambda(LinqlExpression):
    
    type: str = "LinqlLambda"
    Body: LinqlExpression | None
    Parameters: list[LinqlExpression] | None
    
    def Clone(self) -> Self:
        clone = LinqlLambda()
        if self.Next != None:
            clone.Next = self.Next.Clone()

        clone.Parameters = list(map(lambda x: x.Clone(), self.Parameters))
                                
        if self.Body != None:
            clone.Body = self.Body.Clone()

        return clone
    
    def toSerializable(self) -> dict:
        jsonObject = self._CreateSerializableType()
        
        if hasattr(self, "Parameters") and len(self.Parameters) > 0:
            jsonObject["Parameters"] = list(map(lambda x: x.toSerializable(), self.Parameters))

        jsonObject["Body"] = self.Body.toSerializable()
        self._SerializeNext(jsonObject)
        return jsonObject