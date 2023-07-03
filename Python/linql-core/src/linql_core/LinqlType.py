from typing_extensions import Self
from .LinqlExpression import LinqlExpression
from .ITypeNameProvider import ITypeNameProvider, DefaultTypeNameProvider
from typing import Any
import inspect

class LinqlType():
    
    ListType: str = "List"
    TypeName: str | None
    GenericParameters: list[Self]  | None


    def IsList(self) -> bool:
        return self.TypeName == LinqlType.ListType
    
    def GetLinqlType(Value: Any, TypeNameProvider: ITypeNameProvider = DefaultTypeNameProvider()) -> Self:
        linqlType = LinqlType()
        pythonType = type(Value)
        
        _typeSwitch = { 
            str: "String", 
            int: LinqlType.GetNumberType, 
            float: LinqlType.GetNumberType, 
            complex: LinqlType.InvalidType, 
            list: LinqlType.GetListType,
            bool: "Boolean",
            }

        if Value == None:
            switchResult = "undefined"
        else:
            switchResult = _typeSwitch.get(pythonType)

        if switchResult != None:
            if callable(switchResult):
                linqlType = switchResult(Value, linqlType, TypeNameProvider)
            else:
                linqlType.TypeName = switchResult
        else:
           linqlType.TypeName = TypeNameProvider.GetTypeName(Value)
        
        return linqlType

    def InvalidType(Value: Any, Type: Self, TypeNameProvider: ITypeNameProvider):
        raise Exception(f'Value {Value} is not supported at this time.')
    
    def GetNumberType(Value: int | float | complex, Type: Self, TypeNameProvider: ITypeNameProvider) -> Self:
        if Value > 2^32:
            Type.TypeName = "Long"
        else:
            Type.TypeName = "Int32"
        return Type

    def GetListType(Value: list[Any], Type: Self, TypeNameProvider: ITypeNameProvider) -> Self:
        Type.TypeName = LinqlType.ListType
        if len(Value) > 0:
            firstVal = Value[0]
            Type.GenericParameters = []
            Type.GenericParameters.append(LinqlType.GetLinqlType(firstVal, TypeNameProvider))
        else:
            objectType = LinqlType()
            objectType.TypeName = "object"
            Type.GenericParameters = []
            Type.GenericParameters.append(objectType)
        return Type
    
    def toSerializable(self) -> dict:
        jsonObject = {}
        
        jsonObject = { "TypeName": self.TypeName }
        if hasattr(self, "GenericParameters") and len(self.GenericParameters) > 0:
            jsonObject["GenericParameters"] = list(map(lambda x: x.toSerializable(), self.GenericParameters))

        return jsonObject