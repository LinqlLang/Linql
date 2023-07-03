from src.linql_client.LinqlSearch import LinqlSearch
import inspect
import os

class FileLoader:

    BasePath: str
    DebugMode: bool

    def __init__(self, BasePath: str, DebugMode: bool = True) -> None:
        self.BasePath = BasePath
        self.DebugMode = DebugMode

    def ExecuteTest(self, newSearch: LinqlSearch):
        TestName = inspect.stack()[1][3]
        TestName = TestName.replace("test_", "")
        json = newSearch.toJson()
        compare = self.GetFile(TestName)

        if self.DebugMode == True:
            print(json)
            print(compare)

        assert json == compare

    def GetFile(self, TestName: str) -> str:
        listdir = os.listdir(self.BasePath)
        print(listdir)
        file = open(f"{self.BasePath}/{TestName}.json")
        text = file.read()
        return text
