# SPDX-FileCopyrightText: 2023-present kris.sodroski@cbre.com <kris.sodroski@cbre.com>
#
# SPDX-License-Identifier: MIT


from linql_client.LinqlContext import LinqlContext
from linql_client.LinqlSearch import LinqlSearch
from datetime import datetime

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

context = LinqlContext(LinqlSearch, "https://localhost:7113/")

context.Set(State)

