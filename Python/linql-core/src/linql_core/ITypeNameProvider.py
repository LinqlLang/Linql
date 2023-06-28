from typing_extensions import Self
import abc
from typing import Any

class ITypeNameProvider(abc.ABC):
    
    @abc.abstractmethod
    def GetTypeName(self, Type: Any) -> str:
        pass