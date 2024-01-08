## Overview
Provide a basic toolkit for Unity game development.

Include the following functions:
- String Extension
- Type Util
- GameObject Util
- Singleton
- Factory
- FSM
- Debug
- Log
- Job Manager
- Environment Manager
- Download Request
- Data Util
- Remote File Util
- Timer
- Graphic 3D Util

About CSV Util:

CSV Util provides a type for abstraction of CSV table data: CSV Table. 
To write your own CSV logic code, just use the following one namespaces:
```
using Liquid.CommonUtils;
```

A typical example of CSV is as follows:
```
[string]Name,[int]Age,[double]Height
Bill,16,1.6
Joe,17,1.7
Bruce,18,1.8
```

The first row generally represents the table header declaration identifying the data name and data type of each column.
The remaining lines are data.

Or you can use a pure CSV without header declaration row:
```
Name,Age,Height
Bill,16,1.6
Joe,17,1.7
Bruce,18,1.8
```

Both format of CSV can be parsed and serialized correctly. 
CSVTable will distinguish, parse and serialize them automatically.

After explaining the difference between the two forms in the first line,
let's quickly preview the format rules of CSV:

CSV elements separated by commas.

If there are commas, double quotes, spaces or line breaks in one data element,
the data will be wrapped in a pair of double quotes.

A single double quotes mark need to be replaced by two consecutive double quotes marks.

The above is the simple format rule of a CSV format.
CSVTable can handle this kind of data, including parsing, serialization and CRUD operations on CSV table content.