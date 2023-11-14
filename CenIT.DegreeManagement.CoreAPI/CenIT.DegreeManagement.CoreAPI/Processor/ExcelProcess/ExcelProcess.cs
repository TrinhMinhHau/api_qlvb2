using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;

namespace CenIT.DegreeManagement.CoreAPI.Processor.ExcelProcess
{
    public static class ExcelProcess
    {
        public static DataTable ReadExcelData(IFormFile file)
        {
            var dataTable = new DataTable();

            using (var stream = file.OpenReadStream())
            {
                IWorkbook workbook = new XSSFWorkbook(stream);
                ISheet sheet = workbook.GetSheetAt(0); // Lấy sheet đầu tiên

                // Tạo cột cho DataTable từ dòng đầu tiên của sheet (tên cột)
                IRow headerRow = sheet.GetRow(0);
                foreach (var cell in headerRow.Cells)
                {
                    dataTable.Columns.Add(cell.ToString());
                }

                // Lặp qua các hàng trong sheet từ dòng thứ 2 (dữ liệu)
                for (int rowIndex = 2; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    //if (row == null) continue;

                    if (row == null || IsRowEmpty(row))
                        continue;

                    // Tạo một hàng mới trong DataTable
                    DataRow dataRow = dataTable.NewRow();

                    // Lặp qua các ô trong hàng và đổ dữ liệu vào DataRow
                    for (int cellIndex = 0; cellIndex < row.LastCellNum; cellIndex++)
                    {
                        ICell cell = row.GetCell(cellIndex);
                        dataRow[cellIndex] = cell?.ToString() ?? string.Empty;
                    }

                    // Thêm DataRow vào DataTable
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }

        private static bool IsRowEmpty(IRow row)
        {
            foreach (var cell in row.Cells)
            {
                if (cell.CellType != CellType.Blank)
                    return false;
            }
            return true;
        }


    }
}
