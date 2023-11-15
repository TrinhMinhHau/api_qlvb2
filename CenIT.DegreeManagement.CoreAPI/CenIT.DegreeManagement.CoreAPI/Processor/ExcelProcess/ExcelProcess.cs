using ClosedXML.Excel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OfficeOpenXml;
using System.Data;
using System.Globalization;
namespace CenIT.DegreeManagement.CoreAPI.Processor.ExcelProcess
{

    public static class ExcelProcess
    {
        static ExcelProcess()
        {
            // Set the license context to avoid LicenseException
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // Use LicenseContext.Commercial for commercial projects
        }

        //public static DataTable ReadExcelData(IFormFile file)
        //{
        //    var dataTable = new DataTable();
        //    CultureInfo cultureInfo = new CultureInfo("en-US");
        //    Thread.CurrentThread.CurrentCulture = cultureInfo;
        //    Thread.CurrentThread.CurrentUICulture = cultureInfo;

        //    using (var stream = file.OpenReadStream())
        //    {
        //        using (var workbook = new XLWorkbook(stream))
        //        {
        //            var worksheet = workbook.Worksheet(1); // Lấy worksheet đầu tiên

        //            // Tạo cột cho DataTable từ dòng đầu tiên của sheet (tên cột)
        //            var headerRow = worksheet.Row(1);
        //            foreach (var cell in headerRow.Cells())
        //            {
        //                // Sử dụng kiểu dữ liệu double cho cột số thập phân
        //                dataTable.Columns.Add(cell.Value.ToString(), typeof(double));
        //            }

        //            // Lặp qua các hàng trong sheet từ dòng thứ 2 (dữ liệu)
        //            for (int rowIndex = 3; rowIndex <= worksheet.LastRowUsed().RowNumber(); rowIndex++)
        //            {
        //                var row = worksheet.Row(rowIndex);
        //                if (row.IsEmpty())
        //                    continue;

        //                // Tạo một hàng mới trong DataTable
        //                var dataRow = dataTable.NewRow();

        //                // Lặp qua các ô trong hàng và đổ dữ liệu vào DataRow
        //                for (int cellIndex = 0; cellIndex < row.LastCellUsed().CellNumber(); cellIndex++)
        //                {
        //                    var cell = row.Cell(cellIndex + 1);

        //                    // Sử dụng kiểu dữ liệu double cho cột số thập phân
        //                    dataRow[cellIndex] = cell.GetDouble();
        //                }

        //                // Thêm DataRow vào DataTable
        //                dataTable.Rows.Add(dataRow);
        //            }
        //        }
        //    }

        //    return dataTable;
        //}
        //    public static DataTable ReadExcelData(IFormFile file)
        //    {
        //        var dataTable = new DataTable();
        //        CultureInfo cultureInfo = new CultureInfo("en-US");
        //        Thread.CurrentThread.CurrentCulture = cultureInfo;
        //        Thread.CurrentThread.CurrentUICulture = cultureInfo;

        //        using (var stream = file.OpenReadStream())
        //        {
        //            using (var workbook = new XLWorkbook(stream))
        //            {
        //                var worksheet = workbook.Worksheet(1); // Lấy worksheet đầu tiên

        //                // Tạo cột cho DataTable từ dòng đầu tiên của sheet (tên cột)
        //                var headerRow = worksheet.Row(1);
        //                foreach (var cell in headerRow.Cells())
        //                {
        //                    dataTable.Columns.Add(cell.Value.ToString());
        //                }

        //                // Lặp qua các hàng trong sheet từ dòng thứ 2 (dữ liệu)
        //                for (int rowIndex = 3; rowIndex <= worksheet.LastRowUsed().RowNumber(); rowIndex++)
        //                {
        //                    var row = worksheet.Row(rowIndex);
        //                    if (row.IsEmpty())
        //                        continue;

        //                    // Tạo một hàng mới trong DataTable
        //                    var dataRow = dataTable.NewRow();

        //                    // Lặp qua các ô trong hàng và đổ dữ liệu vào DataRow
        //                    foreach (var cell in row.Cells())
        //                    {
        //                        dataRow[cell.Address.ColumnNumber - 1] = cell.Value.ToString() ?? string.Empty;
        //                    }

        //                    // Thêm DataRow vào DataTable
        //                    dataTable.Rows.Add(dataRow);
        //                }
        //            }
        //        }

        //        return dataTable;
        //    }

   
        //public static DataTable ReadExcelDataUsingClosedXML(IFormFile file)
        //{
        //    var dataTable = new DataTable();

        //    // Đặt CultureInfo trước khi đọc file Excel

        //    using (var stream = file.OpenReadStream())
        //    {
        //        using (var workbook = new XLWorkbook(stream))
        //        {
        //            var worksheet = workbook.Worksheet(1); // Lấy worksheet đầu tiên

        //            // Tạo cột cho DataTable từ dòng đầu tiên của sheet (tên cột)
        //            var headerRow = worksheet.Row(1);
        //            foreach (var cell in headerRow.Cells())
        //            {
        //                dataTable.Columns.Add(cell.Value.ToString());
        //            }

        //            // Lặp qua các hàng trong sheet từ dòng thứ 2 (dữ liệu)
        //            for (int rowIndex = 2; rowIndex <= worksheet.LastRowUsed().RowNumber(); rowIndex++)
        //            {
        //                var row = worksheet.Row(rowIndex);
        //                if (row.IsEmpty())
        //                    continue;

        //                // Tạo một hàng mới trong DataTable
        //                var dataRow = dataTable.NewRow();

        //                // Lặp qua các ô trong hàng và đổ dữ liệu vào DataRow
        //                foreach (var cell in row.Cells())
        //                {
        //                    // Sử dụng Convert.ToDouble để đảm bảo đọc số thập phân đúng
        //                    dataRow[cell.Address.ColumnNumber - 1] = Convert.ToDouble(cell.Value.ToString() ?? "0", CultureInfo.InvariantCulture);
        //                }

        //                // Thêm DataRow vào DataTable
        //                dataTable.Rows.Add(dataRow);
        //            }
        //        }
        //    }

        //    return dataTable;
        //}


        //public static DataTable ReadExcelData(IFormFile file)
        //{
        //    var dataTable = new DataTable();

        //    using (var stream = file.OpenReadStream())
        //    {
        //        IWorkbook workbook = new XSSFWorkbook(stream);
        //        ISheet sheet = workbook.GetSheetAt(0); // Lấy sheet đầu tiên

        //        // Tạo cột cho DataTable từ dòng đầu tiên của sheet (tên cột)
        //        IRow headerRow = sheet.GetRow(0);
        //        foreach (var cell in headerRow.Cells)
        //        {
        //            dataTable.Columns.Add(cell.ToString());
        //        }

        //        // Lặp qua các hàng trong sheet từ dòng thứ 2 (dữ liệu)
        //        for (int rowIndex = 2; rowIndex <= sheet.LastRowNum; rowIndex++)
        //        {
        //            IRow row = sheet.GetRow(rowIndex);
        //            //if (row == null) continue;

        //            if (row == null || IsRowEmpty(row))
        //                continue;

        //            // Tạo một hàng mới trong DataTable
        //            DataRow dataRow = dataTable.NewRow();

        //            // Lặp qua các ô trong hàng và đổ dữ liệu vào DataRow
        //            for (int cellIndex = 0; cellIndex < row.LastCellNum; cellIndex++)
        //            {
        //                ICell cell = row.GetCell(cellIndex);


        //                dataRow[cellIndex] = cell?.ToString() ?? string.Empty;
        //            }

        //            // Thêm DataRow vào DataTable
        //            dataTable.Rows.Add(dataRow);
        //        }
        //    }

        //    return dataTable;
        //}
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
