using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Frgo.Dohod.DbModel.Extension.OpenXml.Xlsx
{
    static class Xlsx
    {
        //  cell name : "F23"   =>   "F" : column name.
        public static string GetColumnName(this string cellName)
        {
            // Create a regular expression to match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellName);

            return match.Value;
        }
        public static int GetColumnIndex(this string cellName)
        {
            // (!!! From 0)  Create a regular expression to match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellName);

            int index = 0;
            foreach (var ch in match.Value)
            {
                if (char.IsLetter(ch))
                {
                    int value = ch - 'A' + 1;
                    index = value + index * 26;
                }
                else
                    break;
            }

            return index - 1;

        }


        // cell name : "F23"   =>   22" : row index.
        public static int GetRowIndex(this string cellName)
        {
            // (!!! From 0) Create a regular expression to match the row index portion the cell name.
            Regex regex = new Regex(@"\d+");
            Match match = regex.Match(cellName);

            return int.Parse(match.Value)-1;
        }


        public static int LetterToColIndex(this string colName)
        {
            if (string.IsNullOrWhiteSpace(colName))
                return -1;

            int index = 0;
            foreach (var ch in colName)
            {
                if (char.IsLetter(ch))
                {
                    int value = ch - 'A' + 1;
                    index = value + index * 26;
                }
                else
                    break;
            }

            return index - 1;
        }

        public static string GetCellValue(this WorkbookPart workbookPart, Cell cell)
        {
            var value = cell?.InnerText;
            if (value == null)
                return null;

            if (cell.DataType == null) // number & dates
                value = cell.InnerText;
            else // Shared string or boolean
            {
                switch (cell.DataType.Value)
                {
                    case CellValues.SharedString:
                         //SharedStringItem ssi = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(int.Parse(cell.CellValue.InnerText));
                         //value = ssi.Text.Text;

                        value = workbookPart.SharedStringTablePart.SharedStringTable.ChildElements[int.Parse(cell.CellValue.InnerText)].InnerText;

                        //value = workbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault().SharedStringTable.ElementAt(int.Parse(cell.InnerText)).InnerText;
                        break;
                    case CellValues.Boolean:
                        value = cell.CellValue.InnerText == "0" ? "false" : "true";
                        break;
                    default:
                        value = cell.CellValue.InnerText;
                        break;
                }
            }

            return value;
        }

        public static string GetCellValue0(this  SpreadsheetDocument document, Cell theCell)
        {
           //https://docs.microsoft.com/en-us/office/open-xml/how-to-retrieve-the-values-of-cells-in-a-spreadsheet
            string value = null;

            // If the cell does not exist, return an empty string.
            if (theCell.InnerText.Length > 0)
            {
                value = theCell.InnerText;

                // If the cell represents an integer number, you are done. 
                // For dates, this code returns the serialized value that 
                // represents the date. The code handles strings and 
                // Booleans individually. For shared strings, the code 
                // looks up the corresponding value in the shared string 
                // table. For Booleans, the code converts the value into 
                // the words TRUE or FALSE.
                if (theCell.DataType != null)
                {
                    switch (theCell.DataType.Value)
                    {
                        case CellValues.SharedString:

                            // For shared strings, look up the value in the
                            // shared strings table.
                            var stringTable =
                                document.WorkbookPart.GetPartsOfType<SharedStringTablePart>()
                                .FirstOrDefault();

                            // If the shared string table is missing, something 
                            // is wrong. Return the index that is in
                            // the cell. Otherwise, look up the correct text in 
                            // the table.
                            if (stringTable != null)
                            {
                                value =
                                    stringTable.SharedStringTable
                                    .ElementAt(int.Parse(value)).InnerText;
                            }
                            //
                            var stringTablePart = document.WorkbookPart.SharedStringTablePart;
                            value = stringTablePart.SharedStringTable.ChildElements[int.Parse(value)].InnerText;

                            break;

                        case CellValues.Boolean:
                            switch (value)
                            {
                                case "0":
                                    value = "FALSE";
                                    break;
                                default:
                                    value = "TRUE";
                                    break;
                            }
                            break;
                    }
                }
               
            }
            return value;
        }
        public static string GetCellValue1(this  SpreadsheetDocument document, Cell theCell)
        {
            //https://docs.microsoft.com/en-us/office/open-xml/how-to-retrieve-the-values-of-cells-in-a-spreadsheet
            string value = null;

            // If the cell does not exist, return an empty string.
            if (theCell.InnerText.Length > 0)
            {
                value = theCell.InnerText;

                // If the cell represents an integer number, you are done. 
                // For dates, this code returns the serialized value that 
                // represents the date. The code handles strings and 
                // Booleans individually. For shared strings, the code 
                // looks up the corresponding value in the shared string 
                // table. For Booleans, the code converts the value into 
                // the words TRUE or FALSE.
                if (theCell.DataType != null)
                {
                    switch (theCell.DataType.Value)
                    {
                        case CellValues.SharedString:

                            // For shared strings, look up the value in the
                            // shared strings table.
                            var stringTable =
                                document.WorkbookPart.GetPartsOfType<SharedStringTablePart>()
                                .FirstOrDefault();

                            // If the shared string table is missing, something 
                            // is wrong. Return the index that is in
                            // the cell. Otherwise, look up the correct text in 
                            // the table.
                            if (stringTable != null)
                            {
                                value =
                                    stringTable.SharedStringTable
                                    .ElementAt(int.Parse(value)).InnerText;
                            }
                            //
                            var stringTablePart = document.WorkbookPart.SharedStringTablePart;
                            value = stringTablePart.SharedStringTable.ChildElements[int.Parse(value)].InnerText;

                            break;

                        case CellValues.Boolean:
                            switch (value)
                            {
                                case "0":
                                    value = "FALSE";
                                    break;
                                default:
                                    value = "TRUE";
                                    break;
                            }
                            break;
                    }
                }
                else
                {
                    // DataType property is null for numeric and date types
                    //var cellFormats = document.WorkbookPart.WorkbookStylesPart.Stylesheet.CellFormats;
                    //var numberingFormats = workbookPart.WorkbookStylesPart.Stylesheet.NumberingFormats;

                    //bool isDate = false;
                    //var styleIndex = theCell.StyleIndex.Value;
                    //var cellFormat = (CellFormat)cellFormats.ElementAt((int)styleIndex);
                    //if (cellFormat.NumberFormatId != null)
                    //{
                    //    var numberFormatId = cellFormat.NumberFormatId.Value;
                    //    var numberingFormat = numberingFormats.Cast<NumberingFormat>()
                    //        .SingleOrDefault(f => f.NumberFormatId.Value == numberFormatId);

                    //    // Here's yer string! Example: $#,##0.00_);[Red]($#,##0.00)
                    //    if (numberingFormat != null && numberingFormat.FormatCode.Value.Contains("mm/dd/yy"))
                    //    {
                    //        string formatString = numberingFormat.FormatCode.Value;
                    //        isDate = true;
                    //    }
                    //}
                    //value = DateTime.FromOADate(double.Parse(value)).ToShortDateString();
                    
                }
            }
            return value;
        }
       public static string GetCellValue2(this  SpreadsheetDocument document, Cell cell)
        {
            //https://docs.microsoft.com/en-us/office/open-xml/how-to-retrieve-the-values-of-cells-in-a-spreadsheet
            var value = cell.CellValue?.InnerXml;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                var stringTablePart = document.WorkbookPart.SharedStringTablePart;
                return stringTablePart.SharedStringTable.ChildElements[int.Parse(value)].InnerText;
            }
            else if(cell.StyleIndex==10 || cell.StyleIndex==3)
            {
                value = DateTime.FromOADate(double.Parse(value)).ToShortDateString();
            }
            return value;
        }

        private static string GetFormattedCellValue(WorkbookPart workbookPart, Cell cell)
        {
            if (cell == null)
            {
                return null;
            }

            string value = "";
            if (cell.DataType == null) // number & dates
            {
                int styleIndex = (int)cell.StyleIndex.Value;
                CellFormat cellFormat = (CellFormat)workbookPart.WorkbookStylesPart.Stylesheet.CellFormats.ElementAt(styleIndex);
                uint formatId = cellFormat.NumberFormatId.Value;

                //if (formatId == (uint)Formats.DateShort || formatId == (uint)Formats.DateLong)
                //{
                //    double oaDate;
                //    if (double.TryParse(cell.InnerText, out oaDate))
                //    {
                //        value = DateTime.FromOADate(oaDate).ToShortDateString();
                //    }
                //}
                //else
                {
                    value = cell.InnerText;
                }
            }
            else // Shared string or boolean
            {
                switch (cell.DataType.Value)
                {
                    case CellValues.SharedString:
                        SharedStringItem ssi = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(int.Parse(cell.CellValue.InnerText));
                        value = ssi.Text.Text;
                        break;
                    case CellValues.Boolean:
                        value = cell.CellValue.InnerText == "0" ? "false" : "true";
                        break;
                    default:
                        value = cell.CellValue.InnerText;
                        break;
                }
            }

            return value;
        }
        public static DataTable XlToDataTable(this byte[] fileData) // , string sheetName=""
        {
            var dt = new DataTable();
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(fileData, 0, (int)fileData.Length);
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(stream, false))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    Worksheet sheet = worksheetPart.Worksheet;
                    var sheetData = sheet.GetFirstChild<SheetData>();
                    var rows = sheetData.Descendants<Row>().ToList();
 
                    foreach (var row in rows) 
                    {
                        var dtRow = dt.NewRow();

                        var colCount = row.Descendants<Cell>().Count();
                        foreach (var cell in row.Descendants<Cell>())
                        {
                            var index = cell.CellReference.ToString().GetColumnIndex();

                            // Add Columns
                            for (var i = dt.Columns.Count; i <= index; i++)
                                dt.Columns.Add();

                            dtRow[index] = workbookPart.GetCellValue(cell);
                        }

                        dt.Rows.Add(dtRow);
                    }
                    var xx = dt.Columns;
                }
 
            }
            return dt;
        }

       public static DataTable XlToDataTable1(this byte[] fileData)
        {
            var dt = new DataTable();
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(fileData, 0, (int)fileData.Length);

                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(stream, false))
                {
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
                    WorksheetPart worksheetPart = workbookPart.WorksheetParts.First();
                    Worksheet sheet = worksheetPart.Worksheet;

                    var usedrange = sheet.SheetDimension.Reference.ToString().Split(':'); // "A1:N175"
                    var i0 = usedrange[0].GetRowIndex();
                    var i1 = usedrange[1].GetRowIndex();
                    var j0 = usedrange[0].GetColumnIndex();
                    var j1 = usedrange[1].GetColumnIndex();

                    var sheetData = sheet.GetFirstChild<SheetData>();
                    var rows = sheetData.Descendants<Row>().ToList();
 
                    foreach (var row in rows) //this will also include your header row...
                    {
                        var tempRow = dt.NewRow();

                        var colCount = row.Descendants<Cell>().Count();
                        foreach (var cell in row.Descendants<Cell>())
                        {
                            var index = cell.CellReference.ToString().GetColumnIndex();

                            // Add Columns
                            for (var i = dt.Columns.Count; i <= index; i++)
                                dt.Columns.Add();

                            tempRow[index] = workbookPart.GetCellValue(cell);
                        }

                        dt.Rows.Add(tempRow);
                    }
                    var xx = dt.Columns;
                }
 
            }
            return dt;
        }

    }
}
