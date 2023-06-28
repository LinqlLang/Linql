from typing_extensions import Self
from LinqlExpression import LinqlExpression
from typing import Any

class LinqlType():
    
    ListType: str = "List"
    TypeName: str | None
    GenericParameters: list[Self]  | None

    def IsList(self) -> bool:
        return self.TypeName == Self.ListType