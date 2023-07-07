# SPDX-FileCopyrightText: 2023-present sodroski@bu.edu <sodroski@bu.edu>
#
# SPDX-License-Identifier: MIT


from linql_client.LinqlContext import LinqlContext
from linql_client.LinqlSearch import LinqlSearch
import asyncio
from datetime import datetime
from typing import TypeVar


class StateData: 
    Year: int
    Value: int
    Variable: str
    DateOfRecording: datetime

class State:
    FID: int
    Program: str | None
    State_Code: str | None
    State_Name: str | None
    Flowing_St: str | None
    FID_1: int
    Data: list[StateData] | None

T = TypeVar("T")

class CustomLinqlContext(LinqlContext):
    def GetRoute(self, Search: LinqlSearch[T]):
        endpoint = self.GetSearchTypeString(Search)
        return f"/{endpoint}"


context = CustomLinqlContext(LinqlSearch, "localhost:7113")

async def main():
    set: LinqlSearch[State] = context.Set(State)
    results = await set.ToListAsync()
    
    results = await set.Where(lambda r: "A" in r.State_Code).ToListAsync()
    results = await set.Where(lambda r: "en" in r.State_Name.lower()).ToListAsync()
    print(results)
    result = await set.FirstOrDefaultAsync()
    print(result)


if __name__ == "__main__":
    loop = asyncio.get_event_loop()
    loop.run_until_complete(main())

