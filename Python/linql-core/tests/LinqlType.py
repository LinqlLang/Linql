import unittest
from test import support
from ..src.linql_core.LinqlType import LinqlType

class LinqlTypeTests(unittest.TestCase):

    # Only use setUp() and tearDown() if necessary

    def setUp(self):
        return

    def tearDown(self):
        return
    
    def IsList(self):
        type = LinqlType()
        type.TypeName = "Not List"
        self.assertFalse(type.IsList())
        type.TypeName = LinqlType.ListType
        self.assertTrue(type.IsList)

if __name__ == '__main__':
    unittest.main()