using CenIT.DegreeManagement.CoreAPI.Bussiness.Sys;
using CenIT.DegreeManagement.CoreAPI.Core.Caching;
using CenIT.DegreeManagement.CoreAPI.Core.Models;
using CenIT.DegreeManagement.CoreAPI.Core.Utils;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Input.Sys;
using CenIT.DegreeManagement.CoreAPI.Model.Models.Output.Sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Caching.Sys
{
    public class MessageCL
    {
        private string _masterCacheKey = "MessageCL";
        private CacheLayer _cache;
        private MessageBL _BL;

        public MessageCL(ICacheService cacheService)
        {
            _cache = (CacheLayer)cacheService;
            _cache.setMasterKey(_masterCacheKey);

            var connectDBString = _cache.GetCacheKey<string>(_cache.CONFIGURATION_KEY);
            _BL = new MessageBL(connectDBString ?? "");
        }

        public List<MessageModel> GetByPageSize(int userId, int pageSize)
        {
            //Get item from cache
            List<MessageModel> messages = _BL.GetByPageSize(userId, pageSize);
            return messages;
        }

        public int Save(MessageInputModel model)
        {
            var result = _BL.Save(model);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

        public MessageModel GetMessageById(string id)
        {
            var hashKey = EHashMd5.FromObject(id);
            var rawKey = string.Concat("GetMessageById-", hashKey);

            //Get item from cache
            MessageModel item = _cache.GetCacheKey<MessageModel>(rawKey, _masterCacheKey)!;
            if (item == null)
            {
                item = _BL.GetMessageById(id);
                _cache.AddCacheItem(rawKey, item, _masterCacheKey);
            }

            return item;
        }

        public List<NotificationModel> GetAllMessages(SearchParamFilterDateModel model)
        {
            var hashKey = EHashMd5.FromObject(model);
            var rawKey = string.Concat("GetAllMessages-", hashKey);


            //Get item from cache
            List<NotificationModel> message = _cache.GetCacheKey<List<NotificationModel>>(rawKey, _masterCacheKey)!;
            if (message == null)
            {
                message = _BL.GetAllMessages(model);
                _cache.AddCacheItem(rawKey, message, _masterCacheKey);
            };

            return message;
        }

        public List<NotificationModel> GetAllMessagesByUserId(int userId,SearchParamFilterDateModel model)
        {
            var hashKey = EHashMd5.FromObject(model) + userId;
            var rawKey = string.Concat("GetAllMessagesByUserId-", hashKey);


            //Get item from cache
            List<NotificationModel> message = _cache.GetCacheKey<List<NotificationModel>>(rawKey, _masterCacheKey)!;
            if (message == null)
            {
                message = _BL.GetAllMessagesByUserId(userId, model);
                _cache.AddCacheItem(rawKey, message, _masterCacheKey);
            };

            return message;
        }

        public int UpdateReadStatus(string idMessge)
        {
            var result = _BL.UpdateReadStatus(idMessge);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

        public int UpdateAllReadStatus(int userId)
        {
            var result = _BL.UpdateAllReadStatus(userId);
            if (result > 0)
            {
                _cache.InvalidateCache(_masterCacheKey);
            }

            return result;
        }

        public int GetUnreadMessagesCount(int? userId)
        {

            //Get item from cache
            var messages = _BL.GetUnreadMessagesCount(userId);

            return messages;
        }

        public List<MessageModel> GetAll(int userId)
        {
            var hashKey = EHashMd5.FromObject(userId);
            var rawKey = string.Concat("GetTop10-", hashKey);


            //Get item from cache
            List<MessageModel> messages = _cache.GetCacheKey<List<MessageModel>>(rawKey, _masterCacheKey)!;
            if (messages == null)
            {
                messages = _BL.GetAll(userId);
                _cache.AddCacheItem(rawKey, messages, _masterCacheKey);
            };

            return messages;
        }
    }
}
