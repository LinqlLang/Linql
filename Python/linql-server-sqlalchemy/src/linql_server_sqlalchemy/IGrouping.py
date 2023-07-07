from typing import TypeVar, Generic

Key = TypeVar("Key")
Value = TypeVar("Value")

class IGrouping(list[Value], Generic[Key, Value]):

    Key: Key | None