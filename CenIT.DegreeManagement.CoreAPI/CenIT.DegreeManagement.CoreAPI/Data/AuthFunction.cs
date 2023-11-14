namespace CenIT.DegreeManagement.CoreAPI.Data
{
    public class AuthFunction
    {
        public int FunctionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public AuthFunction()
        {
            // Khởi tạo các giá trị mặc định hoặc logic khởi tạo khác nếu cần thiết
            FunctionId = 0;
            Name = "Auth";
            Description = "Xác Thực";
        }
    }
}
