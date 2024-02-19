Author: Alissa Shaw
Partner: None
Start Date: 01/11/2024
Course: CS 3500, University of Utah, School of Computing
GitHub ID: shawlissa
Repo: https://github.com/uofu-cs3500-spring24/spreadsheet-shawlissa
Commit Date: 2/18/2024
Solution: Spreadsheet
Copyright: CS 3500 and Alissa Shaw - This work may not be copied for use in Academic Coursework.
```
Credit:
	1. Assignment One:
		https://learn.microsoft.com/en-us/dotnet/csharp/how-to/parse-strings-using-split
		https://learn.microsoft.com/en-us/dotnet/api/system.int32.tryparse?view=net-8.0
		https://stackoverflow.com/questions/7383254/is-there-a-method-for-removing-whitespace-characters-from-a-string
	2. Assignment Two:
		https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.sortedlist-2.getkeyatindex?view=net-8.0
		https://stackoverflow.com/questions/12188634/how-to-test-an-ienumerableint
		https://learn.microsoft.com/en-us/dotnet/api/system.collections.dictionarybase?view=net-8.0
	3. Assignment Three:
		https://stackoverflow.com/questions/3561202/check-if-instance-is-of-a-type
		https://stackoverflow.com/questions/3879463/parse-a-number-from-exponential-notation
		https://stackoverflow.com/questions/7899525/how-to-split-a-string-by-space
	4. Assignment Four: 
		N/A
	5. Assignment Five:
		https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/base
		https://learn.microsoft.com/en-us/troubleshoot/developer/visualstudio/csharp/language-compilers/read-write-text-file
		https://learn.microsoft.com/en-us/visualstudio/version-control/git-manage-repository?view=vs-2022



# Overview of the Spreadsheet Functionality

The spreadsheet solution is currently capable of creating, writing, saving, and parsing in spreadsheet objects.
Can create cells, edit their cell contents, and all of their dependencies within the spreadsheet. Cell contents
may be a double, string, or formula. If a Formula object, then is evaluated as one. Can return the spreadsheet as
an XML string and an XML file. If contents change and contains dependencies or other cells are dependent on this changing
cell then the dependencies are evaluated and contents are changed. If spreadsheet changes at all, then it may be saved, 
if nothing changes then the spreadsheet will not be saved. Spreadsheet is capable of determining what names/contents are valid,
how to normalize the variables, and maintains a version.

# Time Expenditures:

	1. Assignment One:   Predicted Hours: 15			Actual Hours: 20
	2. Assignment Two:	 Predicted Hours: 18			Actual Hours: 15
	3. Assignment Three: Predicited Hours: 20		    Actual Hours: 20
	4. Assignment Four: 
			 			 Predicted Hours: 14			Actual Hours: 15
						 Assignment Eval: 3 Hours		Assignment Eval: 5 Hours
						 Writing Code: 5 Hours			Writing Code: 4 Hours
						 Debugging Code: 8 Hours		Debugging Code: 6 Hours
	5. Assignment Five: 
						Predicted Hours: 12				Actual Hours: 13
						Assignment Eval: 2 Hours		Assignment Eval: 2 Hours
						Writing Code: 4 Hours			Writing Code: 4 Hours
						Debugging Code: 6 Hours			Debugging Code: 7 Hours


