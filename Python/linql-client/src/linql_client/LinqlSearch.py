
from typing import TypeVar, Generic, Self, Callable, Any
from linql_core.LinqlExpression import LinqlExpression
from linql_core.LinqlConstant import LinqlConstant
from linql_core.LinqlType import LinqlType
from linql_core.LinqlFunction import LinqlFunction
from .ALinqlContext import ALinqlContext
from .ALinqlSearch import ALinqlSearch
from .AOrderedLinqlSearch import AOrderedLinqlSearch
from .IGrouping import IGrouping

import abc

T = TypeVar("T")
Output = TypeVar("Output")
class LinqlSearch(ALinqlSearch, Generic[T]):
    
    Context: ALinqlContext

    def __init__(self, ModelType: type, Context: ALinqlContext) -> None:
        super().__init__(ModelType, Context)
        self.Context = Context

    def Copy(self) -> Self:
        search = LinqlSearch(self.ModelType, self.Context)

        if self.Expressions != None and len(self.Expressions) > 0:
            search.Expressions = list(map(lambda r: r.Clone(), self.Expressions))

        return search
    
    def toJson(self) -> str:
        return self.Context.ToJson(self)
    
    def OrderBy(self, Expression: Callable[[T], Output]):
        result: AOrderedLinqlSearch[Output] = self.CustomLinqlFunction("OrderBy", Expression)
        return result
    
    def Where(self, Expression: Callable[[T], bool]) -> Self:
        return self.CustomLinqlFunction("Where", Expression)
    
    def SelectMany(self, Expression: Callable[[T], list[Output]]):
        result: LinqlSearch[Output] = self.CustomLinqlFunction("SelectMany", Expression)
        return result
    
    def Select(self, Expression: Callable[[T], Output]):
        result: LinqlSearch[Output] = self.CustomLinqlFunction("Select", Expression)
        return result
    
    def Distinct(self, Expression: Callable[[T], Output]):
        result: LinqlSearch[Output] = self.CustomLinqlFunction("Distinct", Expression)
        return result    
    
    def Include(self, Expression: Callable[[T], Output] | str):
        result: Self = self.CustomLinqlFunction("Include", Expression)
        return result    
    
    def GroupBy(self, Expression: Callable[[T], Output] | str):
        result: list[IGrouping[T, Output]] = self.CustomLinqlFunction("GroupBy", Expression)
        return result    
    
    def Skip(self, Skip: int):
        linqlType = LinqlType.GetLinqlType(Skip)
        constant = LinqlConstant(linqlType, Skip)
        fun = LinqlFunction("Skip", [constant])
        newSearch = self.Copy()
        self.AttachTopLevelFunction(fun, newSearch)
        return newSearch
    
    def Take(self, Take: int):
        linqlType = LinqlType.GetLinqlType(Take)
        constant = LinqlConstant(linqlType, Take)
        fun = LinqlFunction("Take", [constant])
        newSearch = self.Copy()
        self.AttachTopLevelFunction(fun, newSearch)
        return newSearch
    
    def ToListAsyncSearch(self):
        newSearch = self.CustomLinqlFunction("ToListAsync", None)
        return newSearch
    
    def FirstOrDefaultSearch(self, Predicate: Callable[[T], bool] | None = None):
        newSearch = self.CustomLinqlFunction("FirstOrDefaultAsync", Predicate)
        return newSearch

    def LastOrDefaultSearch(self, Predicate: Callable[[T], bool] | None = None):
        newSearch = self.CustomLinqlFunction("LastOrDefaultAsync", Predicate)
        return newSearch
    
