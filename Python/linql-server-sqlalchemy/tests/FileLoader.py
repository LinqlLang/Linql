import inspect
import os

class FileLoader:

    BasePath: str
    DebugMode: bool

    def __init__(self, BasePath: str, DebugMode: bool = True) -> None:
        self.BasePath = BasePath
        self.DebugMode = DebugMode

    def GetFile(self, TestName: str) -> str:
        listdir = os.listdir(self.BasePath)
        print(listdir)
        file = open(f"{self.BasePath}/{TestName}.json")
        text = file.read()
        return text
