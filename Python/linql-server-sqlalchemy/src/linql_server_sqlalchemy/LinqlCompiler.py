from linql_client.LinqlSearch import LinqlSearch
from linql_core.LinqlExpression import LinqlExpression
from linql_core.LinqlConstant import LinqlConstant
from typing import Any

class LinqlCompiler:

    ValidAssemblies: set
    UseCache: bool
    Parameters: dict[str, type]


    def __init__(self, ValidAssemblies: set, UseCache: bool = True) -> None:
        self.ValidAssemblies = ValidAssemblies
        self.UseCache = UseCache

    async def ExecuteAsync(self, Search: LinqlSearch, SqlAlchemyQuery: Any):

        for expression in Search["Expressions"]:
            if expression["$type"] == LinqlConstant.type and expression["ConstantType"]["TypeName"] == "LinqlSearch":
                pass
        pass