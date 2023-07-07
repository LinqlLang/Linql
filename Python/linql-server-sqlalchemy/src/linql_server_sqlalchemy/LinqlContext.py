
from typing import TypeVar, Generic, Self, Any
from linql_core.LinqlExpression import LinqlExpression
from .ALinqlSearch import ALinqlSearch
from .LinqlParser import LinqlParser
from .ALinqlContext import ALinqlContext
from linql_core.ITypeNameProvider import ITypeNameProvider
from .LinqlSearch import LinqlSearch
import http.client
import ssl
import json

T = TypeVar("T")
TResult = TypeVar("TResult")


class LinqlContext(ALinqlContext):
    
    def __init__(self, LinqlSearchType: type, BaseUrl: str) -> None:
        super().__init__(LinqlSearchType, BaseUrl)


    async def GetResult(self, Search: ALinqlSearch[T]) -> TResult:
        endpoint = self.GetEndpoint(Search)
        return await self.SendHttpRequest(endpoint, Search)

    async def SendHttpRequest(self, Endpoint: str, Search: ALinqlSearch[T]) -> TResult:
        jsonSearch = self.ToJson(Search)
        connection = http.client.HTTPSConnection(self._BaseUrl, context = ssl._create_unverified_context())
        headers = {'Content-type': 'application/json'}
        connection.request("POST", Endpoint, jsonSearch, headers)
        response = connection.getresponse()
        jsonResponse = response.read().decode()
        jsonResult = json.loads(jsonResponse)
        return jsonResult
    
    def Set(self, Type: type) -> LinqlSearch[T]:
        return self._LinqlSearchType(Type, self)