
from typing import TypeVar, Generic, Self, Any
from linql_core.LinqlExpression import LinqlExpression
from .ALinqlSearch import ALinqlSearch
from .LinqlParser import LinqlParser
from linql_core.ITypeNameProvider import ITypeNameProvider
import abc
import json

T = TypeVar("T")
TResult = TypeVar("TResult")


class ALinqlContext(ITypeNameProvider, abc.ABC):
    
    _LinqlSearchType: type
    _BaseUrl: str

    def __init__(self, LinqlSearchType: type, BaseUrl: str) -> None:
        super().__init__()
        self._LinqlSearchType = LinqlSearchType
        self._BaseUrl = BaseUrl

    @abc.abstractmethod
    async def GetResult(self, Search: ALinqlSearch[T]) -> TResult:
        pass

    @abc.abstractmethod
    async def SendHttpRequest(self, Endpoint: str, Search: ALinqlSearch[T]) -> TResult:
        pass

    def Parse(self, Expression: str | None) -> LinqlExpression | None:
        parser = LinqlParser(Expression)
        return parser.Root
    
    def ToJson(self, Search: ALinqlSearch[T]) -> str:
        json.dumps(Search.__dict__, skipkeys=True)
    
    def GetTypeName(self, Type: Any) -> str:
        typeName = Type.__class__.__name__

        if typeName == "type":
            instance = Type()
            typeName = instance.__class__.__name__
        return typeName
    
    def GetSearchTypeString(self, Search: ALinqlSearch[T]) -> str:
        return self.GetTypeName(Search.ModelType)
    
    def GetRoute(self, Search: ALinqlSearch[T]):
        endpoint = self.GetSearchTypeString(Search)
        return f"linql/{endpoint}"
    
    def GetEndpoint(self, Search: ALinqlSearch[T]) -> str:
        return self.GetRoute(Search)
    
    def Set(self, Type: type) -> ALinqlSearch[T]:
        return self._LinqlSearchType(Type, self)