using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Utils
{
    public static class Token
    {
        public static string CreateToken()
        {
            // Tạo một chuỗi ngẫu nhiên làm token
            string token = Guid.NewGuid().ToString();

            // Mã hóa token thành chuỗi Base64
            byte[] tokenBytes = System.Text.Encoding.UTF8.GetBytes(token);
            string base64Token = Convert.ToBase64String(tokenBytes);

            return base64Token;
        }

        public static string ResetPasswordTokenGenerator(int length = 32, string email = "")
        {
            byte[] randomBytes = new byte[length];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            // Chuyển đổi các byte ngẫu nhiên thành chuỗi hexa
            string token = BitConverter.ToString(randomBytes).Replace("-", "").ToLower();

            // Mã hóa MD5 cho địa chỉ email
            string emailMD5 = EHashMd5.CalculateMD5(email);

            // Kết hợp mã thông báo với email MD5
            string emailToken = token + "-" + emailMD5;

            return emailToken;
        }
    }
}
