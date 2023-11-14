using CenIT.DegreeManagement.CoreAPI.Caching.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;
using CenIT.DegreeManagement.CoreAPI.Processor.Mail;
using CenIT.DegreeManagement.CoreAPI.Processor.SendNotification;

namespace CenIT.DegreeManagement.CoreAPI.Processor
{
    public class BackgroundJobManager
    {
        private readonly ISendMailService _sendMailService;

        private readonly FirebaseNotificationUtils _firebaseNotificationUtils;

        public BackgroundJobManager(ISendMailService sendMailService, FirebaseNotificationUtils firebaseNotificationUtils)
        {
            _sendMailService = sendMailService;
            _firebaseNotificationUtils = firebaseNotificationUtils;

        }

        public void SendEmailInBackground(MailContent content)
        {
            // Gửi email ở đây
            _sendMailService.SendMail(content).Wait();
        }

        public void SendNotificationInBackground(string title, string body,List<DeviceTokenModel> deviceTokens)
        {
            try
            {
                _firebaseNotificationUtils.SendNotificationToSingleDeviceAsync(title, body, deviceTokens).Wait();
                // Đợi cho việc gửi thông báo hoàn thành và không chạy ngầm nữa
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu có
                Console.WriteLine($"Error sending notification: {ex.Message}");
            }
        }
    }
}
