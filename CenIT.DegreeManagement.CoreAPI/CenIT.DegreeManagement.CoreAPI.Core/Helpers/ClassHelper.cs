using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CenIT.DegreeManagement.CoreAPI.Core.Helpers
{
    public static class ClassHelper
    {
        public static T CloneObjectSerializable<T>(this T obj) where T : class
        {
            var ms = new MemoryStream();
            var bf = new BinaryFormatter();
            bf.Serialize(ms, obj);
            ms.Position = 0;
            var result = bf.Deserialize(ms);
            ms.Close();
            return (T)result;
        }

        public static T Clone<T>(this T source)
        {
            if (!typeof(T).IsSerializable) throw new ArgumentException("The type must be serializable.", "source");

            // Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null)) return default(T);

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static T CloneJson<T>(this T source)
        {
            //Don't serialize a null object, simply return the default for that object
            if (ReferenceEquals(source, null)) return default(T);

             //initialize inner objects individually
             //for example in default constructor some list property initialized with some values,
             //but in 'source' these items are cleaned -
             //without ObjectCreationHandling.Replace default constructor values will be added to result
            var deserializeSettings = new JsonSerializerSettings
            { ObjectCreationHandling = ObjectCreationHandling.Replace };

            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
        }
    }
}
