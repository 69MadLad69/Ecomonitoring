using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ecomonitoring
{
    internal class Excel_Reader
    {

        public static List<List<String>> Read(string filePath)
        {
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets.First();
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                List<List<String>> rows = new List<List<String>>();


                for (int row = 2; row <= rowCount; row++)
                {
                    List<String> readRow = new List<String>();
                    for (int col = 1; col <= colCount; col++) { 
                        readRow.Add(worksheet.Cells[row, col].Text);
                    }
                    rows.Add(readRow);
                }
                return rows;
            }

        }
    }
}
