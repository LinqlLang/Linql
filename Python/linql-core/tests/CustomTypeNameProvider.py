from src.linql_core.ITypeNameProvider import ITypeNameProvider
from typing import Any

class CustomTypeNameProvider(ITypeNameProvider):
    def GetTypeName(self, Type: Any) -> str:
        typeName = Type.__class__.__name__

        if typeName == "type":
            instance = Type()
            typeName = instance.__class__.__name__
        return typeName
