using CenIT.DegreeManagement.CoreAPI.Resources;
using ClosedXML.Excel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;

namespace CenIT.DegreeManagement.CoreAPI.Processor.ExcelProcess
{
    public class ExcelProces : IExcelProcess
    {
        private readonly AppPaths _appPaths;

        public ExcelProces(AppPaths appPaths)
        {
            _appPaths = appPaths;
        }

        public DataTable ReadExcelData(IFormFile file)
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

        public string SaveDataTableErrorToExcel(DataTable dataTable, string nguoiThucHien, string prefixNameFile, 
            List<ExcelColumnConfig> columnConfigs,string savePathFile ,string getPathFile)
        {
            string fileName = $"{nguoiThucHien + "_" + prefixNameFile}";
            string uploadDirectory = savePathFile;
            string filePath = Path.Combine(uploadDirectory, fileName);

            Directory.CreateDirectory(uploadDirectory);

            // Remove the "ErrorCode" column if it exists
            if (dataTable.Columns.Contains("ErrorCode"))
            {
                dataTable.Columns.Remove("ErrorCode");
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("ImportError");

                // Set up column configurations
                for (int columnIndex = 0; columnIndex < columnConfigs.Count; columnIndex++)
                {
                    ExcelColumnConfig config = columnConfigs[columnIndex];
                    var cell1 = worksheet.Cell(1, columnIndex + 1);
                    cell1.Value = config.Label;
                    cell1.Style.Fill.BackgroundColor = XLColor.AliceBlue;
                    cell1.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                    cell1.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                    cell1.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                    cell1.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    // Apply styling based on config
                    if (config.Danger)
                    {
                        cell1.Style.Font.FontColor = XLColor.Red;
                    }

                    if (!string.IsNullOrEmpty(config.Description))
                    {
                        var cell2 = worksheet.Cell(2, columnIndex + 1);
                        cell2.Value = config.Description;
                        cell2.Style.Fill.BackgroundColor = XLColor.AliceBlue;
                        cell2.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        cell2.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        cell2.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        cell2.Style.Border.TopBorder = XLBorderStyleValues.Thin;
  
                    }

                    if (config.BackgroundYellow)
                    {
                        var col = worksheet.Column(columnIndex + 1);

                        col.Style.Fill.BackgroundColor = XLColor.Yellow;
                        col.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        col.Style.Border.LeftBorder = XLBorderStyleValues.Thin;
                        col.Style.Border.RightBorder = XLBorderStyleValues.Thin;
                        col.Style.Border.TopBorder = XLBorderStyleValues.Thin;
                    }
                }

                // Load the data from DataTable to worksheet starting from row 3
                worksheet.Cell(3, 1).InsertData(dataTable.AsEnumerable());

                // Highlight rows with errors (based on the "Message" column)
                if (dataTable.Columns.Contains("Message"))
                {
                    // Highlight rows with errors (based on the "Message" column)
                    var rowsWithErrors = dataTable.AsEnumerable().Where(row => !string.IsNullOrEmpty(row.Field<string>("Message")));
                    foreach (var rowWithError in rowsWithErrors)
                    {
                        var rowIndex = dataTable.Rows.IndexOf(rowWithError) + 3; // Offset by 3 because data starts from row 3
                        worksheet.Row(rowIndex).Style.Font.FontColor = XLColor.Red;
                    }
                }

                worksheet.Columns().AdjustToContents();

                // Save the workbook to a file
                workbook.SaveAs(filePath);
            }

            return "/" + getPathFile + "/" + fileName;
        }
    }
}
