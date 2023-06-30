from .LinqlExpression import LinqlExpression
from .LinqlType import LinqlType

class LinqlSearch():
    
    Type: LinqlType
    Expressions: list[LinqlExpression] | None

    def __init__(self, Type: LinqlType):
        self.Type = Type
    