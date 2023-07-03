from src.linql_core.LinqlType import LinqlType
from tests.CustomTypeNameProvider import CustomTypeNameProvider
from tests.TestClass import TestClass
from typing import Any

class TestLinqlTypeName:

    typeNameProvider = CustomTypeNameProvider()

    def test_IsList(self):
        type = LinqlType()
        type.TypeName = LinqlType.ListType
        assert type.IsList() == True

    def test_IsNotList(self):
        type = LinqlType()
        type.TypeName = "Not List"
        assert type.IsList() == False

    def test_Int(self):
        type = LinqlType.GetLinqlType(5, self.typeNameProvider)
        assert type.TypeName == "Int32"

    def test_Long(self):
        type = LinqlType.GetLinqlType(2^33, self.typeNameProvider)
        assert type.TypeName == "Long"

    def test_String(self):
        type = LinqlType.GetLinqlType('Test', self.typeNameProvider)
        assert type.TypeName == "String"

    def test_Bool(self):
        type = LinqlType.GetLinqlType(False, self.typeNameProvider)
        assert type.TypeName == "Boolean"

    def test_None(self):
        type = LinqlType.GetLinqlType(None, self.typeNameProvider)
        assert type.TypeName == "undefined"

    def test_Class(self):
        testClass = TestClass()
        type = LinqlType.GetLinqlType(testClass, self.typeNameProvider)
        assert type.TypeName == "TestClass"

    def test_ClassDefinition(self):
        type = LinqlType.GetLinqlType(TestClass, self.typeNameProvider)
        assert type.TypeName == "TestClass"

    def test_Serialization(self):
        testClass = TestClass()
        type = LinqlType.GetLinqlType(testClass, self.typeNameProvider)
        serial = type.toSerializable()
        assert "TypeName" in serial
        assert "GenericParameters" not in serial
