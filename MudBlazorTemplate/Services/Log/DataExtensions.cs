using MudBlazorTemplate.Components.Custom;
using System.Data;
using System.Reflection;

namespace MudBlazorTemplate.Services
{
    public static class DataExtensions
    {
        /// <summary>
        /// This method converts a DataTable to an IList of type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            List<PropertyInfo> properties = GetPropertiesForType<T>();
            List<T> result = new List<T>();

            if (!table.IsEmpty())
            {
                foreach (var row in table.Rows)
                {
                    var item = CreateItemFromRow<T>((DataRow)row, properties);
                    result.Add(item);
                }
            }

            return result;
        }

        /// <summary>
        /// This method get the properties of type T
        /// </summary>
        /// <typeparam name="T">the type</typeparam>
        /// <returns>List of type PropertyInfo</returns>
        public static List<PropertyInfo> GetPropertiesForType<T>()
        {
            Dictionary<Type, List<PropertyInfo>> typeDictionary = new Dictionary<Type, List<PropertyInfo>>();

            var type = typeof(T);
            if (!typeDictionary.ContainsKey(typeof(T)))
            {
                typeDictionary.Add(type, type.GetProperties().ToList());
            }

            return typeDictionary[type];
        }

        /// <summary>
        /// This method is used to prepare SQL text fragments.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object PrepValue(this object value)
        {
            if (value is string)
            {
                return value?.AsString().Replace("'", "''");
            }
            else
            {
                return value;
            }
        }

        private static T CreateItemFromRow<T>(DataRow row, List<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (row.Table.Columns.Contains(property.Name))
                {
                    if (property.PropertyType == typeof(double))
                    {
                        property.SetValue(item, row[property.Name].AsDouble(), null);
                    }
                    else if (property.PropertyType == typeof(decimal))
                    {
                        property.SetValue(item, row[property.Name].AsDecimal(), null);
                    }
                    else if (property.PropertyType == typeof(float))
                    {
                        property.SetValue(item, row[property.Name].AsFloat(), null);
                    }
                    else if (property.PropertyType == typeof(long))
                    {
                        property.SetValue(item, row[property.Name].AsLong(), null);
                    }
                    else if (property.PropertyType == typeof(int))
                    {
                        property.SetValue(item, row[property.Name].AsInt(), null);
                    }
                    else if (property.PropertyType == typeof(bool))
                    {
                        property.SetValue(item, row[property.Name].AsBool(), null);
                    }
                    else if (property.PropertyType == typeof(DateTime))
                    {
                        property.SetValue(item, row[property.Name].AsString().AsDateTime(), null);
                    }
                    else //string
                    {
                        property.SetValue(item, row[property.Name].AsString(), null);
                    }
                }
            }
            return item;
        }

        /// <summary>
        /// This method will determine if a DataTable contains and data.
        /// </summary>
        /// <param name="dt">DataTable object</param>
        /// <returns></returns>
        public static bool IsEmpty(this DataTable dt)
        {
            if (dt == null) return true;
            if (dt.Rows == null) return true;
            if (dt.Rows.Count == 0) return true;
            return false;
        }
    }
}
