
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
    
    async def _executeCustomLinqlFunction(self, FunctionName: str, Predicate: Any | None = None):
        search = self.CustomLinqlFunction(FunctionName, Predicate)
        return await self.Context.GetResult(search)

    async def ToListAsync(self) -> list[T]:
        result: list[T] = await self._executeCustomLinqlFunction("ToListAsync")
        return result
    
    async def FirstOrDefaultAsync(self, Predicate: Callable[[T], bool] | None = None) -> T | None:
        result: T = await self._executeCustomLinqlFunction("FirstOrDefaultAsync", Predicate)
        return result

    async def LastOrDefaultAsync(self, Predicate: Callable[[T], bool] | None = None) -> T | None:
        result: T = await self._executeCustomLinqlFunction("LastOrDefaultAsync", Predicate)
        return result

    async def AnyAsync(self, Predicate: Callable[[T], bool] | None = None) -> bool:
        result: bool = await self._executeCustomLinqlFunction("AnyAsync", Predicate)
        return result

    async def AllAsync(self, Predicate: Callable[[T], bool] | None = None) -> bool:
        result: bool = await self._executeCustomLinqlFunction("AllAsync", Predicate)
        return result

    async def MinAsync(self, Predicate: Callable[[T], Output] | None = None) -> Output:
        result: Output = await self._executeCustomLinqlFunction("MinAsync", Predicate)
        return result

    async def MaxAsync(self, Predicate: Callable[[T], Output] | None = None) -> Output:
        result: Output = await self._executeCustomLinqlFunction("MaxAsync", Predicate)
        return result
    
    async def MinByAsync(self, Predicate: Callable[[T], Output] | None = None) -> T:
        result: T = await self._executeCustomLinqlFunction("MinByAsync", Predicate)
        return result
    
    async def MaxByAsync(self, Predicate: Callable[[T], Output] | None = None) -> T:
        result: T = await self._executeCustomLinqlFunction("MaxByAsync", Predicate)
        return result

    async def SumAsync(self, Predicate: Callable[[T], Output] | None = None) -> int:
        result: T = await self._executeCustomLinqlFunction("SumAsync", Predicate)
        return result

    async def AverageAsync(self, Predicate: Callable[[T], Output] | None = None) -> int:
        result: T = await self._executeCustomLinqlFunction("AverageAsync", Predicate)
        return result
