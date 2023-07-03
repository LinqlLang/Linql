
from typing import TypeVar, Generic, Self, Callable, Any
from linql_core.LinqlExpression import LinqlExpression
from linql_core.LinqlConstant import LinqlConstant
from linql_core.LinqlType import LinqlType
from linql_core.LinqlFunction import LinqlFunction
from linql_core.LinqlSearch import LinqlSearch as CoreLinqlSearch
import abc

T = TypeVar("T")

class ALinqlSearch(abc.ABC, CoreLinqlSearch, Generic[T]):

    ModelType: type
    Expressions: list[LinqlExpression] | None

    def __init__(self, ModelType: type) -> None:
        super().__init__(LinqlType.GetLinqlType(ModelType))
        self.ModelType = ModelType
        self.Expressions = []
        searchExpression = self.BuildLinqlSeachExpression()
        self.Expressions.append(searchExpression)

    def BuildLinqlSeachExpression(self) -> LinqlConstant:
        linqlType = LinqlType.GetLinqlType(self.ModelType)
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


