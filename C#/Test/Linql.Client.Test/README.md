# Linql.Client.Test

Unit tests for the [Linql C# client](../../Linql.Client/). 

## Development 

- Visual Studio 2022 
- .Net 7 (requires Visual Studio Preview at the time of this writing.  Can change this to .Net 6 without issue)

## Testing Procedure

These tests use the files in Linql.Test.Files.  Whenever a test is executed, the LinqlSearch that is being tested is turned into the Json, and compared to the File that matches the test method name. 

For example, the test [SimpleExpressions_Test.LinqlConstant](./Expressions/SimpleExpressions_Test.cs) will match the generated json with the [LinqlConstant](../Linql.Test.Files/TestFiles/SimpleExpressions/LinqlConstant.json).

To add a test, create a new test method and its implementation, and ensure WriteOutput is set to true in TestFileTests.  When running the tests, the output of the statement will be put in the Output folder.  

Once your manual testing/verification is complete, move the outputted file to the Linql.Test.Files project and create a pull request. 
