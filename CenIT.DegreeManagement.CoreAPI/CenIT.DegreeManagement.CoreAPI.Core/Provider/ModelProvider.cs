using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CenIT.DegreeManagement.CoreAPI.Core.Provider
{
    public class ModelProvider
    {
        public static T CreateModelFromRow<T>(DataRow row) where T : new()
        {
            // create a new object
            var item = new T();

            // set the item
            SetDataFromRow(item, row);

            // return 
            return item;
        }

        public static void SetDataFromRow<T>(T item, DataRow row) where T : new()
        {
            // go through each column
            foreach (DataColumn c in row.Table.Columns)
            {
                // find the property for the column
                var p = item.GetType().GetProperty(c.ColumnName);

                // if exists, set the value
                if (p != null && row[c] != DBNull.Value)
                {
                    Type targetType = p.PropertyType;
                    object value = ConvertToPropertyType(row[c], c.ColumnName);
                    p.SetValue(item, value, null);
                }
            }
        }

        private static object? ConvertToPropertyType(object value, string columnName)
        {
            string stringValue = value.ToString();

            if (columnName.ToLower().Contains("stt"))
            {
                //NẾU RỖNG HOC EMTY THÌ TRẢ VỀ NULL 
                return int.TryParse(stringValue, out int intValue) ? intValue : (int?)null;
            }
            else
            // Kiểm tra tên cột có chứa chuỗi "gioiTinh"
            if (columnName.ToLower().Contains("gioitinh"))
            {
                stringValue = stringValue.ToLower();
                return stringValue == "nam" || string.IsNullOrEmpty(stringValue) ? true : false;
            }
            else if (columnName.ToLower().Contains("ngay"))
            {
                //NẾU RỖNG HOC EMTY THÌ TRẢ VỀ NULL
                return DateTime.TryParse(stringValue, out DateTime dateTimeValue) ? dateTimeValue : (DateTime?)null;
            }
            else if (columnName.ToLower().Contains("diem"))
            {
                //NẾU RỖNG HOC EMTY THÌ TRẢ VỀ NULL 
                return double.TryParse(stringValue, out double doubleValue) ? doubleValue : (double?)null;
            }
            // Đối với các kiểu dữ liệu khác hoặc không phải trường hợp "gioiTinh", "ngay", "diem"
            // trả về giá trị ban đầu
            return value;
        }


        public static List<T> CreateListFromTable<T>(DataTable tbl) where T : new()
        {
            // define return list
            var lst = new List<T>();

            // go through each row
            foreach (DataRow r in tbl.Rows)
                // add to the list
                lst.Add(CreateModelFromRow<T>(r));

            // return the list
            return lst;
        }

        public static DataTable ToDataTable<T>(IList<T> data)
        {
            var props = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            for (var i = 0; i < props.Count; i++)
            {
                var prop = props[i];
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(
                                                 prop.PropertyType) ?? prop.PropertyType);
            }

            var values = new object[props.Count];
            foreach (var item in data)
            {
                for (var i = 0; i < values.Length; i++) values[i] = props[i].GetValue(item);
                table.Rows.Add(values);
            }

            return table;
        }

        public static TTarget MapModelFromModel<TSource, TTarget>(TSource source) where TTarget : new()
        {
            var target = new TTarget();
            var sourceProperties = typeof(TSource).GetProperties();
            var targetProperties = typeof(TTarget).GetProperties();

            foreach (var sourceProp in sourceProperties)
            {
                var targetProp = targetProperties.FirstOrDefault(p => p.Name == sourceProp.Name && p.PropertyType == sourceProp.PropertyType);
                if (targetProp != null && targetProp.CanWrite)
                {
                    var value = sourceProp.GetValue(source);
                    targetProp.SetValue(target, value);
                }
            }

            return target;
        }

        public static void MapProperties<TSource, TDestination>(TSource source, TDestination destination)
        {
            var srcProperties = typeof(TSource).GetProperties();
            var destProperties = typeof(TDestination).GetProperties();

            foreach (var srcProp in srcProperties)
            {
                var destProp = destProperties.FirstOrDefault(p => p.Name == srcProp.Name);
                if (destProp != null && destProp.CanWrite && destProp.PropertyType == srcProp.PropertyType)
                {
                    var srcValue = srcProp.GetValue(source);
                    if (srcValue != null)
                    {
                        destProp.SetValue(destination, srcValue);
                    }
                }
            }
        }

        public static List<T> ExtractDataFromMongoList<T>(object data, out int total)
        {
            var kq = JsonConvert.SerializeObject(data);
            JObject json = JObject.Parse(kq);

            JArray firstBatchArray = (JArray)json["cursor"]["firstBatch"];
            List<T> result = new List<T>();

            total = 0;

            if (firstBatchArray != null && firstBatchArray.Count > 0)
            {
                total = firstBatchArray[0]["totalRow"].Value<int>();
                result = firstBatchArray[0]["data"].ToObject<List<T>>();
            }

            return result;
        }

        public static string ExtractJsonFromMongo(object data)
        {
            var result = JsonConvert.SerializeObject(data);
            JObject json = JObject.Parse(result);
            var firstElement = (json["cursor"]["firstBatch"] as JArray)?.FirstOrDefault();
            string jsonString = firstElement?.ToString();
            return jsonString;
        }

        public static List<T> MapFromMongoDB<T>(object result)
        {
            var kq = JsonConvert.SerializeObject(result);

            JObject json = JObject.Parse(kq);

            JArray firstBatchArray = (JArray)json["cursor"]["firstBatch"];

            List<T> resultList = new List<T>();

            foreach (var item in firstBatchArray)
            {
                T model = Activator.CreateInstance<T>();

                foreach (var property in typeof(T).GetProperties())
                {
                    var propertyName = property.Name;
                    if (propertyName == "Id")
                        propertyName = "_id";

                    if (item[propertyName] != null)
                    {
                        var propertyValue = item[propertyName].ToString();

                        if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(model, propertyValue);
                        }
                        else if (property.PropertyType == typeof(int))
                        {
                            property.SetValue(model, Convert.ToInt32(propertyValue));
                        }
                        // Thêm các kiểu dữ liệu khác tương ứng
                    }
                }

                resultList.Add(model);
            }

            return resultList;
        }

    }
}
