from typing_extensions import Self
import abc
from typing import Any

class ITypeNameProvider(abc.ABC):
    
    @abc.abstractmethod
    def GetTypeName(self, Type: Any) -> str:
        pass

class DefaultTypeNameProvider(ITypeNameProvider):
    def GetTypeName(self, Type: Any) -> str:
        typeName = Type.__class__.__name__

        if typeName == "type":
            instance = Type()
            typeName = instance.__class__.__name__
        return typeName