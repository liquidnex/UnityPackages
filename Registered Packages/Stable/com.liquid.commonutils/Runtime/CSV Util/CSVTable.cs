using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Liquid.CommonUtils
{
    /// <summary>
    /// CSV column header.
    /// Each header includes a type and a field name.
    /// </summary>
    public class CSVColumnHeader
    {
        /// <summary>
        /// Construct a CSV column header.
        /// </summary>
        /// <param name="headerName">Header name.</param>
        /// <param name="type">Header type.</param>
        public CSVColumnHeader(string headerName, CSVElem.Type type = CSVElem.Type.VOID)
        {
            HeaderType = type;
            HeaderName = headerName;
        }

        public CSVElem.Type HeaderType;
        public string HeaderName;
    }

    /// <summary>
    /// CSV row.
    /// A row of CSV contains information about all elements of the current row and their headers.
    /// </summary>
    public class CSVRow
    {
        /// <summary>
        /// Construct a CSV row with a list of elements and headers.
        /// </summary>
        /// <param name="elems">CSV elements of the row.</param>
        /// <param name="headers">CSV headers of the row.</param>
        public CSVRow(List<CSVElem> elems, List<CSVColumnHeader> headers = null)
        {
            RowElems = elems;
            if (headers != null)
                Headers = headers;
        }

        /// <summary>
        /// Accessing a element in CSV row by index.
        /// </summary>
        /// <param name="i">Index.</param>
        /// <returns>Picked element.</returns>
        public CSVElem this[int i]
        {
            get
            {
                if (i >= 0 && i < RowElems.Count)
                    return RowElems[i];
                else
                    return null;
            }
            set
            {
                if (i >= 0 && i < RowElems.Count)
                    RowElems[i] = value;
            }
        }

        /// <summary>
        /// Get a element by its header name.
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <returns>Picked elem.</returns>
        public CSVElem GetElemByName(string name)
        {
            if (name == null)
                return null;

            int idx = Headers.FindIndex(
                delegate (CSVColumnHeader header)
                {
                    return header != null &&
                        header.HeaderName == name;
                }
            );

            if (idx >= 0 && idx < RowElems.Count)
                return RowElems[idx];
            else
                return null;
        }

        /// <summary>
        /// Count the number of elements in CSV row.
        /// </summary>
        /// <returns>Counted number.</returns>
        public int Count => RowElems.Count;

        /// <summary>
        /// Original CSV row elements.
        /// </summary>
        public List<CSVElem> RowElems = new List<CSVElem>();

        /// <summary>
        /// Original row headers.
        /// </summary>
        public List<CSVColumnHeader> Headers = new List<CSVColumnHeader>();
    }

    /// <summary>
    /// CSV Column.
    /// A column of CSV contains information about all elements of the current column and the header of current column.
    /// </summary>
    public class CSVColumn
    {
        /// <summary>
        /// Construct a CSV column with a list of elements and the column header.
        /// </summary>
        /// <param name="elems">CSV elements of the column.</param>
        /// <param name="header">Header of column.</param>
        public CSVColumn(List<CSVElem> elems, CSVColumnHeader header = null)
        {
            ColumnElems = elems;
            Header = header;
        }

        /// <summary>
        /// Accessing a element in CSV column by index.
        /// </summary>
        /// <param name="i">Index.</param>
        /// <returns>Picked element.</returns>
        public CSVElem this[int i]
        {
            get
            {
                if (i >= 0 && i < ColumnElems.Count)
                    return ColumnElems[i];
                else
                    return null;
            }
            set
            {
                if (i >= 0 && i < ColumnElems.Count)
                    ColumnElems[i] = value;
            }
        }

        /// <summary>
        /// Count the number of elements in CSV column.
        /// </summary>
        /// <returns>Counted number.</returns>
        public int Count()
        {
            return ColumnElems.Count;
        }

        /// <summary>
        /// Original column header.
        /// </summary>
        public CSVColumnHeader Header;
        /// <summary>
        /// Original CSV column elements.
        /// </summary>
        public List<CSVElem> ColumnElems = new List<CSVElem>();
    }

    /// <summary>
    /// CSV Table.
    /// Contains a CSV structure consisting of row & column elements and headers.
    /// </summary>
    public class CSVTable
    {
        private enum ASTState
        { 
            NONE,
            HEAD,
            ES_HEAD,
            CONTENT,
            ES_CONTENT,
            TAIL,
            ES_TAIL,
            END
        }

        /// <summary>
        /// Construct a CSV table from a CSV string.
        /// </summary>
        /// <param name="tableName">CSV table name.</param>
        /// <param name="tableStr">CSV string.</param>
        /// <param name="delimiter">Customized delimiter symbol.</param>
        /// <param name="escapeChar">Customized escape character.</param>
        /// <param name="isPureCSV">It is used to identify whether this CSV has a header.</param>
        public CSVTable(string tableName, string tableStr, char delimiter = ',', char escapeChar = '"', bool isPureCSV = false)
        {
            TableName = tableName;
            d = delimiter;
            e = escapeChar;
            IsPureCSV = isPureCSV;

            try
            {
                Regex headerRgx = new Regex(@"\[([^\[\]]+)\]([^\[\]]+)");

                string[] csvRows = tableStr.Split('\n');
                for (int i = 0; i < csvRows.Length; ++i)
                {
                    string curRowStr = csvRows[i];
                    if (curRowStr.Length == 0)
                        continue;

                    List<string> elemArray = SplitCSVRow(curRowStr);

                    // Judgment whether it is a pure CSV
                    if (!IsPureCSV && i == 0)
                    {
                        List<CSVColumnHeader> tempHeaders = new List<CSVColumnHeader>();
                        foreach (string curElemStr in elemArray)
                        {
                            Match matches = headerRgx.Match(curElemStr);
                            if (matches.Success && matches.Groups.Count == 3)
                            {
                                string headerType = matches.Groups[1].Value;
                                string headerName = matches.Groups[2].Value;

                                CSVElem.Type vt = ParseStrToType(headerType);
                                CSVColumnHeader header = new CSVColumnHeader(headerName, vt);
                                tempHeaders.Add(header);
                            }
                            else
                            {
                                tempHeaders.Add(null);
                            }
                        }

                        columnHeaders = tempHeaders;
                        IsPureCSV = !columnHeaders.TrueForAll(h => h != null);
                        if (IsPureCSV)
                        {
                            for (int headerIdx = 0; headerIdx < columnHeaders.Count; ++headerIdx)
                            {
                                columnHeaders[headerIdx] = null;
                            }
                        }
                    }

                    // Handle with eath data row
                    if (IsPureCSV || i != 0)
                    {
                        AddElemListAsRow(elemArray);
                    }
                }
            }
            catch (Exception e)
            {
                string message = string.Format("Failed to serialize tableStr: {0}. Exception: {1}", tableStr, e.Message);
                Debug.LogError(message);
            }

            void AddElemListAsRow(List<string> elemArray)
            {
                if (elemArray == null)
                    return;

                List<CSVElem> curRow = new List<CSVElem>();
                foreach (string curElemStr in elemArray)
                {
                    string v = curElemStr;
                    CSVElem e = new CSVElem(v);
                    curRow.Add(e);
                }
                csvTable.Add(curRow);
            }
        }

        /// <summary>
        /// Construct a pure CSV table from explicit CSV elements.
        /// </summary>
        /// <param name="tableName">CSV table name.</param>
        /// <param name="tableElems">CSV elements in table.</param>
        /// <param name="delimiter">Customized delimiter symbol.</param>
        /// <param name="escapeChar">Customized escape character.</param>
        public CSVTable(string tableName, List<List<CSVElem>> tableElems, char delimiter = ',', char escapeChar = '"')
        {
            TableName = tableName;
            csvTable = tableElems;

            int headerCount = 0;
            if (csvTable.Count > 0)
                headerCount = csvTable[0].Count;
            for (int i = 0; i < headerCount; ++i)
            {
                columnHeaders.Add(null);
            }

            IsPureCSV = true;
            d = delimiter;
            e = escapeChar;
        }

        /// <summary>
        /// Construct a CSV table from explicit CSV elements.
        /// </summary>
        /// <param name="tableName">CSV table name.</param>
        /// <param name="tableElems">CSV elements in table.</param>
        /// <param name="headers">CSV Headers.</param>
        /// <param name="delimiter">Customized delimiter symbol.</param>
        /// <param name="escapeChar">Customized escape character.</param>
        public CSVTable(string tableName, List<List<CSVElem>> tableElems, List<CSVColumnHeader> headers, char delimiter = ',', char escapeChar = '"')
        {
            TableName = tableName;
            csvTable = tableElems;

            int headerCount = 0;
            if (csvTable.Count > 0)
                headerCount = csvTable[0].Count;
            int hc = headers.Count;
            if (hc < headerCount)
            {
                for (int i = 0; i < headerCount - hc; ++i)
                {
                    headers.Add(new CSVColumnHeader(""));
                }
            }
            else if (hc > headerCount)
            {
                headers.RemoveRange(headerCount-1, hc-headerCount);
            }
            columnHeaders = headers;

            IsPureCSV = !columnHeaders.TrueForAll(h => h != null);
            if (IsPureCSV)
            {
                for (int headerIdx = 0; headerIdx < columnHeaders.Count; ++headerIdx)
                {
                    columnHeaders[headerIdx] = null;
                }
            }

            d = delimiter;
            e = escapeChar;
        }

        /// <summary>
        /// Serialize the CSV table into CSV string.
        /// </summary>
        /// <returns>CSV string.</returns>
        public string Serialize()
        {
            // Usual length constant of one CSV cell
            const int lenPerCell = 20;
            StringBuilder strBuild = new StringBuilder(GetRowSize() * GetColumnSize() * lenPerCell);

            if (!IsPureCSV)
            {
                // Handle with column headers
                for (int i = 0; i < columnHeaders.Count; ++i)
                {
                    CSVColumnHeader h = columnHeaders[i];
                    if (h == null)
                    {
                        strBuild.Append(RegularParam(""));
                    }
                    else
                    {
                        string typeStr = ParseTypeToStr(h.HeaderType);
                        string f = string.Format($"[{typeStr}]{h.HeaderName}");
                        strBuild.Append(RegularParam(f));
                    }

                    if (i != columnHeaders.Count - 1)
                    {
                        strBuild.Append(d);
                    }
                }
                strBuild.Append("\n");
            }

            // Handle with each csv row
            for (int i = 0; i < csvTable.Count; ++i)
            {
                List<CSVElem> row = csvTable[i];
                for (int j = 0; j < row.Count; ++j)
                { 
                    string valueStr = row[j];
                    string f = string.Format($"{valueStr}");
                    strBuild.Append(RegularParam(f));

                    if (j != row.Count - 1)
                    {
                        strBuild.Append(d);
                    }
                }

                if (i != csvTable.Count - 1)
                {
                    strBuild.Append("\n");
                }
            }

            return strBuild.ToString();
        }

        /// <summary>
        /// Accessing a row in CSV table by index.
        /// </summary>
        /// <param name="i">Index.</param>
        /// <returns>Picked CSV row.</returns>
        public CSVRow this[int i]
        {
            get => GetRow(i);
        }

        /// <summary>
        /// Accessing a element in CSV table by index.
        /// </summary>
        /// <param name="i">Row index.</param>
        /// <param name="j">Column index.</param>
        /// <returns>Picked CSV element.</returns>
        public CSVElem this[int i, int j]
        {
            get => GetElem(i, j);
        }

        /// <summary>
        /// Get a element in CSV table by index.
        /// </summary>
        /// <param name="rowIdx">Row index.</param>
        /// <param name="columnIdx">Column index.</param>
        /// <returns>Picked CSV element.</returns>
        public CSVElem GetElem(int rowIdx, int columnIdx)
        {
            if (rowIdx < GetRowSize() && columnIdx < GetColumnSize())
                return csvTable[rowIdx][columnIdx];
            else
                return null;
        }

        /// <summary>
        /// Get a element in CSV table by index and header name.
        /// </summary>
        /// <param name="rowIdx">Row index.</param>
        /// <param name="columnName">Header name of the target column.</param>
        /// <returns>Picked CSV element.</returns>
        public CSVElem GetElem(int rowIdx, string columnName)
        {
            int columnIdx = FindColumnIdxByName(columnName);
            return GetElem(rowIdx, columnIdx);
        }

        /// <summary>
        /// Count the number of rows in CSV table.
        /// </summary>
        /// <returns>Counted number.</returns>
        public int GetRowSize()
        {
            return csvTable.Count;
        }

        /// <summary>
        /// Count the number of columns in CSV table.
        /// </summary>
        /// <returns>Counted number.</returns>
        public int GetColumnSize()
        {
            return columnHeaders.Count;
        }

        /// <summary>
        /// Get a row in CSV table by index.
        /// </summary>
        /// <param name="idx">Index.</param>
        /// <returns>Picked CSV row.</returns>
        public CSVRow GetRow(int idx)
        {
            if (idx >= 0 &&
                idx < GetRowSize())
            {
                CSVRow r = new CSVRow(csvTable[idx], columnHeaders);
                return r;
            }
            else
                return null;
        }

        /// <summary>
        /// Get a column in CSV table by index.
        /// </summary>
        /// <param name="idx">Index.</param>
        /// <returns>Picked CSV column.</returns>
        public CSVColumn GetColumnByIdx(int idx)
        {
            if (idx < GetColumnSize())
            {
                List<CSVElem> elems = new List<CSVElem>();
                int rowSize = GetRowSize();
                for (int i = 0; i < rowSize; ++i)
                {
                    elems.Add(csvTable[i][idx]);
                }

                return new CSVColumn(elems, columnHeaders[idx]);
            }
            else
                return null;
        }

        /// <summary>
        /// Get a column in CSV table by name.
        /// </summary>
        /// <param name="name">Header name.</param>
        /// <returns>Picked CSV column.</returns>
        public CSVColumn GetColumnByName(string name)
        {
            int idx = FindColumnIdxByName(name);
            return GetColumnByIdx(idx);
        }

        /// <summary>
        /// Set the specific value for a element in the CSV table.
        /// </summary>
        /// <param name="rowIdx">Row index of the target element.</param>
        /// <param name="columnIdx">Column index of the target element.</param>
        /// <param name="value">New value.</param>
        /// <returns>
        /// If the setting is successful, it returns true;
        /// otherwise, it returns false.
        /// </returns>
        public bool SetElemValue(int rowIdx, int columnIdx, string value)
        {
            if (rowIdx < GetRowSize() && columnIdx < GetColumnSize())
            {
                csvTable[rowIdx][columnIdx].SetValue(value);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Set the specific value for a element in the CSV table.
        /// </summary>
        /// <param name="rowIdx">Row index of the target element.</param>
        /// <param name="columnName">Header name of the target element's column.</param>
        /// <param name="value">>New value.</param>
        /// <returns>
        /// If the setting is successful, it returns true;
        /// otherwise, it returns false.
        /// </returns>
        public bool SetElemValue(int rowIdx, string columnName, string value)
        {
            int columnIdx = FindColumnIdxByName(columnName);
            return SetElemValue(rowIdx, columnIdx, value);
        }

        /// <summary>
        /// Append a new row for CSV table.
        /// </summary>
        /// <param name="csvElems">A elements list of CSV row.</param>
        /// <returns>
        /// If the setting is successful, it returns true;
        /// otherwise, it returns false.
        /// </returns>
        public bool AppendRow(List<CSVElem> csvElems)
        {
            int rowLength = csvElems.Count;
            if (rowLength == GetColumnSize())
            {
                csvTable.Add(csvElems);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Append a new row for CSV table.
        /// </summary>
        /// <param name="row">
        /// A CSV row with elements.
        /// The headers of CSV row can be empty.
        /// </param>
        /// <returns>
        /// If the setting is successful, it returns true;
        /// otherwise, it returns false.
        /// </returns>
        public bool AppendRow(CSVRow row)
        {
            return AppendRow(row.RowElems);
        }

        /// <summary>
        /// Append a new column for CSV table.
        /// </summary>
        /// <param name="column">A CSV row with elements.</param>
        /// <returns>
        /// If the setting is successful, it returns true;
        /// otherwise, it returns false.
        /// </returns>
        public bool AppendColumn(CSVColumn column)
        {
            if (column.Count() == GetRowSize())
            {
                if (IsPureCSV)
                    columnHeaders.Add(null);
                else
                    columnHeaders.Add(column.Header);

                for (int i = 0; i < csvTable.Count; ++i)
                {
                    csvTable[i].Add(column[i]);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove a row from CSV table.
        /// </summary>
        /// <param name="idx">Target row index.</param>
        /// <returns>
        /// If the setting is successful, it returns true;
        /// otherwise, it returns false.
        /// </returns>
        public bool RemoveRow(int idx)
        {
            if (idx >= 0 &&
                idx < GetRowSize())
            {
                csvTable.RemoveAt(idx);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove a column from CSV table.
        /// The related column header will also be removed.
        /// </summary>
        /// <param name="idx">Target column index.</param>
        /// <returns>
        /// If the setting is successful, it returns true;
        /// otherwise, it returns false.
        /// </returns>
        public bool RemoveColumnByIdx(int idx)
        {
            if (idx >= 0 &&
                idx < GetColumnSize())
            {
                foreach (List<CSVElem> row in csvTable)
                {
                    row.RemoveAt(row.Count - 1);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove a column from CSV table.
        /// The related column header will also be removed.
        /// </summary>
        /// <param name="name">Target column header name.</param>
        /// <returns>
        /// If the setting is successful, it returns true;
        /// otherwise, it returns false.
        /// </returns>
        public bool RemoveColumnByName(string name)
        {
            return RemoveColumnByIdx(FindColumnIdxByName(name));
        }

        private int FindColumnIdxByName(string name)
        {
            if (name == null)
                return -1;

            return columnHeaders.FindIndex(
                delegate (CSVColumnHeader header)
                {
                    return header != null &&
                        header.HeaderName == name;
                }
            );
        }

        private CSVElem.Type ParseStrToType(string str)
        {
            str = str.ToLower();
            ValueTuple<CSVElem.Type, string> keyPair = TypeDescPair.Find(
                delegate (ValueTuple<CSVElem.Type, string> pair)
                {
                    return pair.Item2 == str;
                }
            );
            return keyPair.Item1;
        }

        private string ParseTypeToStr(CSVElem.Type type)
        {
            ValueTuple<CSVElem.Type, string> keyPair = TypeDescPair.Find(
                delegate (ValueTuple<CSVElem.Type, string> pair)
                {
                    return pair.Item1 == type;
                }
            );
            return keyPair.Item2;
        }

        private string RegularParam(string str)
        {
            if (str == null)
                return null;

            string ds = "" + d;
            string es = "" + e;

            if (str.Contains(ds)||
                str.Contains(es) ||
                str.Contains("\n") ||
                str.Contains(" "))
            {
                str.Replace(es, es+es);
                str = es + str + es;
            }
            return str;
        }

        private List<string> SplitCSVRow(string str)
        {
            List<string> result = new List<string>();

            ASTState latestState = ASTState.NONE;
            string elemStr = "";
            int idx = 0;
            while (latestState != ASTState.END)
            {
                bool success = false;

                if (idx >= str.Length)
                {
                    result.Add(elemStr);
                    latestState = ASTState.END;
                    success = true;
                }
                else if (latestState == ASTState.NONE ||
                    latestState == ASTState.TAIL ||
                    latestState == ASTState.ES_TAIL)
                {
                    success = ParseHeader(str, ref idx, ref latestState, ref elemStr, ref result);
                }
                else if (latestState == ASTState.HEAD ||
                         latestState == ASTState.CONTENT)
                {
                    success = ParseTail(str, ref idx, ref latestState, ref elemStr, ref result);
                    if (!success)
                    {
                        success = ParseContent(str, ref idx, ref latestState, ref elemStr, ref result);
                    }
                }
                else if (latestState == ASTState.ES_HEAD ||
                         latestState == ASTState.ES_CONTENT)
                {
                    success = ParseESTail(str, ref idx, ref latestState, ref elemStr, ref result);
                    if (!success)
                    {
                        success = ParseESContent(str, ref idx, ref latestState, ref elemStr, ref result);
                    }
                }

                if (!success)
                {
                    Debug.LogError("Parsing aborted because unexpected characters were encountered.");
                    break;
                }
            }

            // Delete redundant empty contents on before and after
            if (result[0] == "")
                result.RemoveAt(0);
            string lastElemStr = result[result.Count - 1];
            if (lastElemStr == "\n")
                result.RemoveAt(result.Count - 1);

            return result;
        }

        private bool ParseHeader(string str, ref int idx, ref ASTState state, ref string elemStr, ref List<string> result)
        {
            char? cur = SafeGet(str, idx);
            if (cur == null)
                return false;

            if (idx != 0 &&
                SafeGet(str, idx - 1) != d)
                return false;

            if (cur == e)
            {
                idx += 1;
                state = ASTState.ES_HEAD;
                return true;
            }
            else
            {
                idx += 1;
                state = ASTState.HEAD;
                elemStr += cur;
                return true;
            }
        }

        private bool ParseContent(string str, ref int idx, ref ASTState state, ref string elemStr, ref List<string> result)
        {
            char? cur = SafeGet(str, idx);
            if (cur == null)
                return false;

            if (cur == d)
                return false;

            if (cur == e)
                return false;

            if (cur == '\n')
                return false;

            if (cur == ' ')
                return false;

            idx += 1;
            elemStr += cur;
            return true;
        }

        private bool ParseESContent(string str, ref int idx, ref ASTState state, ref string elemStr, ref List<string> result)
        {
            char? cur = SafeGet(str, idx);
            if (cur == null)
                return false;

            if (cur == e)
            {
                if (SafeGet(str, idx + 1) == e)
                {
                    idx += 2;
                    state = ASTState.ES_CONTENT;
                    elemStr += e;
                    return true;
                }
                return false;
            }
            else
            {
                idx += 1;
                elemStr += cur;
                return true;
            }
        }

        private bool ParseTail(string str, ref int idx, ref ASTState state, ref string elemStr, ref List<string> result)
        {
            char? cur = SafeGet(str, idx);
            if (cur == null)
                return false;

            if (idx == str.Length - 1 &&
                cur == d)
            {
                result.Add(elemStr);
                elemStr = "";

                idx += 1;
                state = ASTState.TAIL;
                return true;
            }
            else if (cur == d)
            {
                result.Add(elemStr);
                elemStr = "";

                idx += 1;
                state = ASTState.TAIL;
                return true;
            }
            else if (idx == str.Length - 1)
            {
                elemStr += cur;
                result.Add(elemStr);
                elemStr = "";

                idx += 1;
                state = ASTState.END;
                return true;
            }

            return false;
        }

        private bool ParseESTail(string str, ref int idx, ref ASTState state, ref string elemStr, ref List<string> result)
        {
            char? cur = SafeGet(str, idx);
            if (cur == null)
                return false;

            if (SafeGet(str, idx - 1) != e &&
                cur == e &&
                SafeGet(str, idx + 1) == d &&
                idx + 1 == str.Length - 1)
            {
                result.Add(elemStr);
                elemStr = "";

                idx += 2;
                state = ASTState.ES_TAIL;
                return true;
            }
            else if (SafeGet(str, idx - 1) != e &&
                     cur == e &&
                     SafeGet(str, idx + 1) == d)
            {
                result.Add(elemStr);
                elemStr = "";

                idx += 2;
                state = ASTState.ES_TAIL;
                return true;
            }
            else if (SafeGet(str, idx - 1) != e &&
                     cur == e && 
                     idx + 1 == str.Length)
            {
                result.Add(elemStr);
                elemStr = "";

                idx += 3;
                state = ASTState.END;
                return true;
            }

            return false;
        }

        private char? SafeGet(string str, int idx)
        {
            if (idx < 0 ||
                idx >= str.Length)
                return null;

            return str[idx];
        }

        public readonly bool IsPureCSV = false;
        public readonly string TableName;

        private List<CSVColumnHeader> columnHeaders = new List<CSVColumnHeader>();
        private List<List<CSVElem>> csvTable = new List<List<CSVElem>>();

        private List<ValueTuple<CSVElem.Type, string>> TypeDescPair =
            new List<ValueTuple<CSVElem.Type, string>> {
                        (CSVElem.Type.BOOL, "bool"),
                        (CSVElem.Type.CHAR, "char"),
                        (CSVElem.Type.BYTE, "byte"),
                        (CSVElem.Type.SBYTE, "sbyte"),
                        (CSVElem.Type.USHORT, "ushort"),
                        (CSVElem.Type.SHORT, "short"),
                        (CSVElem.Type.UINT, "uint"),
                        (CSVElem.Type.INT, "int"),
                        (CSVElem.Type.ULONG, "ulong"),
                        (CSVElem.Type.LONG, "long"),
                        (CSVElem.Type.FLOAT, "float"),
                        (CSVElem.Type.DOUBLE, "double"),
                        (CSVElem.Type.DECIMAL, "decimal"),
                        (CSVElem.Type.STRING, "string")
            };

        private char d = ',';
        private char e = '"';
    }
}