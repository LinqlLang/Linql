from typing_extensions import Self
import abc

class LinqlExpression(abc.ABC):
    
    type: str = None
    Next: Self = None
    
    def GetLastExpressionInNextChain(self) -> Self:
        if self.Next == None:
            return self
        return self.Next.GetLastExpressionInNextChain()

    @abc.abstractmethod
    def Clone(self) -> Self:
        pass
    
    @abc.abstractmethod
    def toSerializable(self) -> object:
        pass

    def _CreateSerializableType(self) -> dict:
        serializeable = { "$type": self.type }
        return serializeable
    
    def _SerializeNext(self, existingObject: dict) -> dict:
        if hasattr(self, "Next") and self.Next != None:
            existingObject["Next"] = self.Next.toSerializable()
        return existingObject