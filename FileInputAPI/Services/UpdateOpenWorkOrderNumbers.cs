using FileInputAPI.Entities;
using Microsoft.AspNetCore.Http;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using OfficeOpenXml;
using System.Linq;
using System.Data;
using System;
using System.Threading.Tasks;

namespace FileInputAPI.Services
{
    public class UpdateOpenWorkOrderNumbers : IUpdateOpenWorkOrderNumbers
    {
        private readonly AppDbContext _context;

        public UpdateOpenWorkOrderNumbers(AppDbContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// This method use npoi package to save the file
        /// </summary>
        /// <param name="file"></param>
        public void UpdateWONumbers(IFormFile file)
        {   
            var stream = file.OpenReadStream();

            XSSFWorkbook workBook = new XSSFWorkbook(stream);

            var excelSheet = workBook.GetSheetAt(0);

            List<WordOrderNumbers> workOrderNumbers = new List<WordOrderNumbers>();

            for (int i = 0; i < excelSheet.LastRowNum; i++)
            {
                WordOrderNumbers workOrderNumber = new WordOrderNumbers()
                {
                    WorkOrderNumber = excelSheet.GetRow(i).GetCell(0).ToString(),
                    SerialNumber = excelSheet.GetRow(i).GetCell(1).ToString()
                };

                workOrderNumbers.Add(workOrderNumber);
            }

            this._context.WordOrderNumber.AddRange(workOrderNumbers);
            this._context.SaveChanges();
        }

        public void UpdateWONumbersWithEPPlus(IFormFile file)
        {
            DataTable tblContent = GenerateDataTableFromExcel(file);

            List<WordOrderNumbers> workOrderNumbers = new List<WordOrderNumbers>();

            foreach (DataRow tblRow in tblContent.Rows)
            {
                WordOrderNumbers workOrderNumber = new WordOrderNumbers()
                {
                    WorkOrderNumber = tblRow["Work Order Number"].ToString(),
                    SerialNumber = tblRow["Serial Number (Primary Incident Asset Serial Number) (Asset)"].ToString()
                };

                workOrderNumbers.Add(workOrderNumber);
            }

            this._context.WordOrderNumber.AddRange(workOrderNumbers);
            this._context.SaveChanges();
        }

        private DataTable GenerateDataTableFromExcel(IFormFile file)
        {
            using (ExcelPackage package = new ExcelPackage())
            {
                using (var stream = file.OpenReadStream())
                {
                    package.Load(stream);
                }

                ExcelWorksheet workSheet = package.Workbook.Worksheets.FirstOrDefault();

                DataTable table = new DataTable();

                foreach (ExcelRangeBase workSheetHeader in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
                {
                    table.Columns.Add(workSheetHeader.Text);
                }

                for (int rowNumber = 2; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
                {
                    var workSheetRow = workSheet.Cells[rowNumber, 1, rowNumber, workSheet.Dimension.End.Column];

                    DataRow row = table.Rows.Add();

                    foreach (ExcelRangeBase cell in workSheetRow)
                    {
                        row[cell.Start.Column - 1] = cell.Text;
                    }
                }

                return table;
            }
        }
    }
}
