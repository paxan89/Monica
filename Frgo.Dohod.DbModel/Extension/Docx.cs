using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using WorldTable = DocumentFormat.OpenXml.Wordprocessing.Table;
using WorldText = DocumentFormat.OpenXml.Wordprocessing.Text;

namespace Frgo.Dohod.DbModel.Extension.OpenXml.Docx
{
    static class Docx
    { 
        public static DataTable DocxTableToDataTableByCaptionStart(this byte[] fileData, string strCaptionStart) 
        {
            var dt = new DataTable();
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(fileData, 0, (int)fileData.Length);
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(stream, false))
                {
                    IEnumerable<Paragraph> paragraphElement = wordDocument.MainDocumentPart.Document.Descendants<Paragraph>();
                    var sections = wordDocument.MainDocumentPart.Document.Body.Elements<OpenXmlElement>();
                    foreach (OpenXmlElement section in sections)
                    {
                        if (section.GetType().Name == "Table" &&
                            section.InnerText.StartsWith(strCaptionStart))
                        {
                            dt = new DataTable();
                            var tab = (WorldTable)section;
                            foreach (TableRow row in tab.Descendants<TableRow>())
                            {
                                var dtRow = dt.NewRow();
                                var index = 0;
                                foreach (TableCell cell in row.Descendants<TableCell>())
                                {
                                    if(dt.Columns.Count<=index)
                                       dt.Columns.Add();
                                    dtRow[index++] = cell.InnerText.Trim();
                                }
                                dt.Rows.Add(dtRow);
                            }
                        }
                    }                  
                } 
            }
            return dt;
        }
        public static DataTable DocxTableToDataTableByTbNumber(this byte[] fileData, int numTb) // нумерация от 1
        {
            var dt = new DataTable();
            var iTb = 0;
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(fileData, 0, (int)fileData.Length);
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(stream, false))
                {
                    IEnumerable<Paragraph> paragraphElement = wordDocument.MainDocumentPart.Document.Descendants<Paragraph>();
                    var sections = wordDocument.MainDocumentPart.Document.Body.Elements<OpenXmlElement>();
                    foreach (OpenXmlElement section in sections)
                    {
                        if (section.GetType().Name == "Table" &&
                                 ++iTb == numTb)
                        {
                            dt = new DataTable();
                            var tab = (WorldTable)section;
                            foreach (TableRow row in tab.Descendants<TableRow>())
                            {
                                var dtRow = dt.NewRow();
                                var index = 0;
                                foreach (TableCell cell in row.Descendants<TableCell>())
                                {
                                    if(dt.Columns.Count<=index)
                                       dt.Columns.Add();
                                    dtRow[index++] = cell.InnerText.Trim();
                                }
                                dt.Rows.Add(dtRow);
                            }
                        }
                    }                  
                } 
            }
            return dt;
        }
        public static string DocxTextBetweenParagaphs(this byte[] fileData, string strParagraphStart1, string strParagraphStart2, string[] startSamples=null)
        {
            var sb = new StringBuilder();
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(fileData, 0, (int)fileData.Length);
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(stream, false))
                {
                    var isStarted = string.IsNullOrWhiteSpace(strParagraphStart1);
                    IEnumerable<Paragraph> paragraphElement = wordDocument.MainDocumentPart.Document.Descendants<Paragraph>();
                    var sections = wordDocument.MainDocumentPart.Document.Body.Elements<OpenXmlElement>();
                    foreach (OpenXmlElement section in sections)
                    {
                        if (section.GetType().Name == "Paragraph")
                        {
                            Paragraph par = (Paragraph)section;
                            var parText = par.InnerText.Trim();
                            
                            if (!isStarted && parText.StartsWith(strParagraphStart1))
                                isStarted = true;
                            else if(isStarted)
                            {
                                if (!string.IsNullOrWhiteSpace(strParagraphStart2) &&
                                    par.InnerText.StartsWith(strParagraphStart2))
                                    break; // isStarted = false;
                                if (isStarted && 
                                    ( startSamples==null || startSamples.Any(x=>parText.StartsWith(x.Trim()))) ) 
                                    sb.AppendLine(par.InnerText.Trim());
                            }
                        }
                    }                  
                } 
            }
            return sb.ToString();
        }
        public static DataTable DocxTableToDataTable(this byte[] fileData) // , string sheetName=""
        {
            var dt = new DataTable();
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(fileData, 0, (int)fileData.Length);
                using (WordprocessingDocument wordDocument = WordprocessingDocument.Open(stream, false))
                {
                    IEnumerable<Paragraph> paragraphElement = wordDocument.MainDocumentPart.Document.Descendants<Paragraph>();
                    var sections = wordDocument.MainDocumentPart.Document.Body.Elements<OpenXmlElement>();
                    foreach (OpenXmlElement section in sections)
                    {
                        if (section.GetType().Name == "Paragraph")
                        {
                            var sb = new StringBuilder();
                            Paragraph par = (Paragraph)section;
                            IEnumerable<WorldText> textElements = par.Descendants<WorldText>();
                            //Append inner text
                            foreach (Text t in textElements)
                            {
                                sb.Append(t.Text);
                            }
                        }
                        else if (section.GetType().Name == "Table")
                        {
                            dt = new DataTable();
                            var tab = (WorldTable)section;
                            foreach (TableRow row in tab.Descendants<TableRow>())
                            {
                                var dtRow = dt.NewRow();
                                var index = 0;
                                foreach (TableCell cell in row.Descendants<TableCell>())
                                {
                                    if(dt.Columns.Count<=index)
                                       dt.Columns.Add();
                                    dtRow[index++] = cell.InnerText.Trim();
                                }
                                dt.Rows.Add(dtRow);
                            }
                        }
                        var xx = dt.Columns;
                    }                  
                } 
            }
            return dt;
        }


    }
}
