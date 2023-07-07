
from typing import TypeVar, Generic, Self, Callable, Any
from linql_core.LinqlExpression import LinqlExpression
from linql_core.LinqlConstant import LinqlConstant
from linql_core.LinqlType import LinqlType
from linql_core.LinqlFunction import LinqlFunction
from linql_core.LinqlSearch import LinqlSearch as CoreLinqlSearch
from linql_core.ITypeNameProvider import ITypeNameProvider
from .IGrouping import IGrouping
import abc

T = TypeVar("T")
Output = TypeVar("Output")

class ALinqlSearch(abc.ABC, CoreLinqlSearch, Generic[T]):

    ModelType: type
    Expressions: list[LinqlExpression] | None
    TypeNameProvider: ITypeNameProvider

    def __init__(self, ModelType: type, ITypeNameProvider: ITypeNameProvider) -> None:
        super().__init__(LinqlType.GetLinqlType(ModelType, ITypeNameProvider))
        self.TypeNameProvider = ITypeNameProvider
        self.ModelType = ModelType
        self.Expressions = []
        searchExpression = self.BuildLinqlSeachExpression()
        self.Expressions.append(searchExpression)

    def BuildLinqlSeachExpression(self) -> LinqlConstant:
        linqlType = LinqlType.GetLinqlType(self.ModelType, self.TypeNameProvider)
        searchType = LinqlType()
        searchType.TypeName = "LinqlSearch"
        searchType.GenericParameters = []
        searchType.GenericParameters.append(linqlType)
        searchExpression = LinqlConstant(searchType, None)
        return searchExpression

    @abc.abstractmethod
    def Copy(self) -> Self:
        pass
        
    def AttachTopLevelFunction(self, customFunction: LinqlFunction, search: Self):

        if len(search.Expressions) > 0:
            lastExpression =  search.Expressions[0].GetLastExpressionInNextChain()
            lastExpression.Next = customFunction
        elif customFunction != None:
            search.Expressions.append(customFunction)
            
    def CustomLinqlFunction(self, FunctionName: str, Expression: Any | None):
        customFunction = LinqlFunction(FunctionName, [])
        functionArguments = self.Context.Parse(Expression)

        if functionArguments != None:
            customFunction.Arguments = []
            customFunction.Arguments.append(functionArguments)
        
        newSearch = self.Copy()
        self.AttachTopLevelFunction(customFunction, newSearch)
        return newSearch
    
    def Where(self, Expression: Callable[[T], bool]) -> Self:
        return self.CustomLinqlFunction("Where", Expression)
    
    def SelectMany(self, Expression: Callable[[T], list[Output]]):
        result: ALinqlSearch[Output] = self.CustomLinqlFunction("SelectMany", Expression)
        return result
    
    def Select(self, Expression: Callable[[T], Output]):
        result: ALinqlSearch[Output] = self.CustomLinqlFunction("Select", Expression)
        return result
    
    def Distinct(self, Expression: Callable[[T], Output]):
        result: ALinqlSearch[Output] = self.CustomLinqlFunction("Distinct", Expression)
        return result    
    
    def Include(self, Expression: Callable[[T], Output] | str):
        result: Self = self.CustomLinqlFunction("Include", Expression)
        return result    
    
    def GroupBy(self, Expression: Callable[[T], Output] | str):
        result: list[IGrouping[T, Output]] = self.CustomLinqlFunction("GroupBy", Expression)
        return result    
    
    def Skip(self, Skip: int):
        linqlType = LinqlType.GetLinqlType(Skip, self.TypeNameProvider)
        constant = LinqlConstant(linqlType, Skip)
        fun = LinqlFunction("Skip", [constant])
        newSearch = self.Copy()
        self.AttachTopLevelFunction(fun, newSearch)
        return newSearch
    
    def Take(self, Take: int):
        linqlType = LinqlType.GetLinqlType(Take, self.TypeNameProvider)
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
