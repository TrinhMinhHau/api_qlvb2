using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Helpers
{
    public static class CacheHelper
    {
        public static List<string> GetCacheKeys(IMemoryCache cache)
        {
            var keys = new List<string>();

            var coherentState = typeof(MemoryCache)
                .GetField("_coherentState", BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(cache);
            var entriesCollection = coherentState?
                .GetType().GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance)?
                .GetValue(coherentState) as ICollection;

            if (entriesCollection != null)
            {
                keys.AddRange(entriesCollection.Cast<object>()
                    .Select(item => item.GetType()
                    .GetProperty("Key")?
                    .GetValue(item)?.ToString()));
            }
            return keys;
        }
    }
}
