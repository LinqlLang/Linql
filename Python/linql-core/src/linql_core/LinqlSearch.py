from .LinqlExpression import LinqlExpression
from .LinqlType import LinqlType

class LinqlSearch():
    
    Type: LinqlType
    Expressions: list[LinqlExpression] | None

    def __init__(self, Type: LinqlType):
        self.Type = Type

    def toSerializable(self) -> dict:
        jsonObject = {}
        
        jsonObject = { "Type": self.Type.toSerializable() }
        if hasattr(self, "Expressions") and len(self.Expressions) > 0:
            jsonObject["Expressions"] = list(map(lambda x: x.toSerializable(), self.Expressions))

        return jsonObject    