
from typing import TypeVar, Generic, Self, Callable, Any
from linql_core.LinqlExpression import LinqlExpression
from linql_core.LinqlConstant import LinqlConstant
from linql_core.LinqlType import LinqlType
from linql_core.LinqlFunction import LinqlFunction
from .ALinqlContext import ALinqlContext
from .ALinqlSearch import ALinqlSearch
import abc

T = TypeVar("T")
Output = TypeVar("Output")

class AOrderedLinqlSearch(ALinqlSearch, Generic[T]):
    
    Context: ALinqlContext

    def __init__(self, ModelType: type, Context: ALinqlContext) -> None:
        super().__init__(ModelType)
        self.Context = Context

    def ThenByDescending(self, Expression: Callable[[T], Output]) -> Self:
        return self.CustomLinqlFunction("ThenByDescending", Expression)
    
    def ThenBy(self, Expression: Callable[[T], Output]) -> Self:
        return self.CustomLinqlFunction("ThenBy", Expression)
