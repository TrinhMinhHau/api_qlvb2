using CenIT.DegreeManagement.CoreAPI.Bussiness.Account;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Account;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Account;

namespace CenIT.DegreeManagement.CoreAPI.Caching.Account
{
    public class AuthCL
    {
        private CacheLayer _cache;
        private AuthBL _authBL;
        private string _masterCacheKey = "AuthCL";
        public AuthCL(ICacheService cacheService)
        {
            _cache = (CacheLayer)cacheService;
            var connectDBString = _cache.GetCacheKey<string>(_cache.CONFIGURATION_KEY);
            _authBL = new AuthBL(connectDBString ?? "");
        }

        /// <summary>
        /// Đăng nhập (Caching)
        /// </summary>
        /// <param name="LoginModel"></param>
        /// <returns>
        ///     Trả về ResponseLogin trong đó:
        ///     + userId > 0 (Đăng nhập thành công)
        ///     + userId = -9 (Tài khoản đã ngưng hoạt động)
        ///     + userId = -1 (Tài khoản hoặc mật khẩu không tồn tại)
        /// </returns>
        public ResponseLogin Login(AccountInputModel model)
        {
            ResponseLogin item = _authBL.Login(model);
            if (item.UserId > 0)
            {
                _cache.AddCacheItem(item.Token, item, _masterCacheKey);
            }
            return item;
        }

        /// <summary>
        /// Đăng Xuất
        /// </summary>
        /// <param name="LogoutModel"></param>
        /// <returns>
        ///     Trả về kiểu int: 
        ///     + > 0 (Đăng xuất thành công)
        ///     + < 0 (Đăng xuất thất bại)
        /// </returns>
        public int Logout(string token)
        {
            int result = _authBL.Logout(token);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        /// <summary>
        /// Lưu token reset password 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <param name="tokenExpires">Thời gian hết hạn của token (1 ngày)</param>
        /// <returns></returns>

        public int SavePasswordResetToken(string email, string token, DateTime tokenExpires)
        {
            int result = _authBL.SavePasswordResetToken(email, token, tokenExpires);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <param name="ResetPasswordModel"></param>
        /// <returns>
        ///    Trả về kiểu int: 
        ///     + > 0 (Reset thành công)
        ///     + == -1 (Token không đúng hoặc token hết hạn)
        ///     + == -9 (email không tồn tại)
        ///     + reset thất bại
        /// </returns>
        public int ResetPassword(AccountInputModel model)
        {
            int result = _authBL.ResetPassword(model);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }
            return result;
        }

        public ResponseLogin GetCacheLogin(string username = "", string token = "")
        {
            ResponseLogin item = _cache.GetCacheKey<ResponseLogin>(token, _masterCacheKey)!;
            if (item == null)
            {
                item = _authBL.GetTokenFromDB(username, token);
                _cache.AddCacheItem(token, item);
            }
            return item;
        }
    }
}