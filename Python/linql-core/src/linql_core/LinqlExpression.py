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