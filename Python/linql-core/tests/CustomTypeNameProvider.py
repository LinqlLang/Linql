from src.linql_core.ITypeNameProvider import ITypeNameProvider
from typing import Any

class CustomTypeNameProvider(ITypeNameProvider):
    def GetTypeName(self, Type: Any) -> str:
        return Type.__class__.__name__
