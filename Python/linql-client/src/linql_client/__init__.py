# SPDX-FileCopyrightText: 2023-present sodroski@bu.edu <sodroski@bu.edu>
#
# SPDX-License-Identifier: MIT

import builtins
from typing import Callable, Generic, TypeVar

Input = TypeVar("Input")
Output = TypeVar("Output")

def selectMany(Expression: Callable[[Input], list[Output]], Input: list[Input]) -> list[Output]:
    pass