from typing_extensions import Self
from .LinqlExpression import LinqlExpression
from .LinqlType import LinqlType
from typing import Any

class LinqlBinary(LinqlExpression):
    
    type: str = "LinqlBinary"
    BinaryName: str
    Left: LinqlExpression | None
    right: LinqlExpression | None

    def __init__(self, BinaryName: str, Left: LinqlExpression | None, Right: LinqlExpression | None):
        self.BinaryName = BinaryName
        self.Left = Left
        self.Right = Right
    
    def Clone(self) -> Self:
        if self.Left != None:
            left = self.Left.Clone()
        if self.Right != None:
            right = self.Right.Clone()

        clone = LinqlBinary(self.BinaryName, left, right)

        if self.Next != None:
            clone.Next = self.Next.Clone()

        return clone