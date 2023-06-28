from src.linql_core.LinqlType import LinqlType

def test_IsList():
    type = LinqlType()
    type.TypeName = LinqlType.ListType
    assert type.IsList() == True

def test_IsNotList():
    type = LinqlType()
    type.TypeName = "Not List"
    assert type.IsList() == False