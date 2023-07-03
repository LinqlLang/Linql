from src.linql_client.LinqlSearch import LinqlSearch
import inspect

class FileLoader:

    BasePath: str
    DebugMode: bool

    def __init__(self, BasePath: str, DebugMode: bool = False) -> None:
        self.BasePath = BasePath
        self.DebugMode = DebugMode

    def ExecuteTest(self, newSearch: LinqlSearch):
        TestName = inspect.stack()[1][3]
        json = newSearch.toJson()
        compare = self.GetFile(TestName)

        if self.DebugMode == True:
            print(json)
            print(compare)

        assert json == compare

    def GetFile(self, TestName: str) -> str:
        file = open(f"{self.BasePath}/{TestName}.json")
        text = file.readlines()
        return text
