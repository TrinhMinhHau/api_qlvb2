using System.Data;

namespace CenIT.DegreeManagement.CoreAPI.Processor.ExcelProcess
{
    public interface IExcelProcess
    {
        public string SaveDataTableErrorToExcel(DataTable dataTable, string nguoiThucHien, string prefixNameFile, List<ExcelColumnConfig> columnConfigs, string savePathFile, string getPathFile);
        public DataTable ReadExcelData(IFormFile file);
    }
}
