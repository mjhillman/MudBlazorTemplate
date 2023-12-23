using HITS.LIB.Utilities;
using System.Data;
using System.Reflection;
using System.Text;

namespace HITS.LIB.Sqlite
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
        /// This method derives a SQL Update statement from a POCO class
        /// </summary>
        /// <typeparam name="T">the POCO type</typeparam>
        /// <param name="poco">the POCO object</param>
        /// <param name="tableName">the database table name</param>
        /// <param name="whereClause">the SQL where clause</param>
        /// <returns>string SQL statment</returns>
        public static string AsSqlUpdate<T>(this object poco, string tableName, string whereClause)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"UPDATE {tableName}");
            sb.Append("SET ");
            List<PropertyInfo> properties = GetPropertiesForType<T>();

            foreach (PropertyInfo property in properties)
            {
                bool skipData = GetSkipDataAttributeValue(property);

                if (!skipData)
                {
                    if (!property.GetValue(poco).IsNull())
                    {
                        if (property.PropertyType == typeof(double) ||
                            property.PropertyType == typeof(decimal) ||
                            property.PropertyType == typeof(float) ||
                            property.PropertyType == typeof(int) ||
                            property.PropertyType == typeof(long))
                        {
                            sb.Append($"[{property.Name}] = {property.GetValue(poco).AsString()},");
                        }
                        else if (property.PropertyType == typeof(bool))
                        {
                            bool bvalue = property.GetValue(poco).AsBool();
                            sb.Append(bvalue ? $"[{property.Name}] = 1, " : $"[{property.Name}] = 0,");
                        }
                        else //string
                        {
                            sb.Append($"[{property.Name}] = '{PrepValue(property.GetValue(poco).AsString())}',");
                        }
                    }
                }
            }

            sb.Remove(sb.Length - 1, 1);
            sb.AppendLine(string.IsNullOrWhiteSpace(whereClause) ? ";" : $"{Environment.NewLine}{whereClause};");

            return sb.ToString();
        }

        /// <summary>
        /// The SkipData attribute is used to exclude auto-number fields and any other field not used in an insert or update statement.    
        /// </summary>
        /// <param name="property">PropertyInfo</param>
        /// <returns>true if the field should be excluded</returns>
        private static bool GetSkipDataAttributeValue(PropertyInfo property)
        {
            try
            {
                IEnumerable<object> attributes = property.GetCustomAttributes(false);
                foreach (object attributeClass in attributes)
                {
                    if (attributeClass is DataAttributes)
                    {
                        return ((DataAttributes)attributeClass).SkipData;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// This method derives a SQL Insert statement from a POCO class
        /// </summary>
        /// <typeparam name="T">the POCO type</typeparam>
        /// <param name="poco">the POCO object</param>
        /// <param name="tableName">the database table name</param>
        /// <returns>string SQL statment</returns>
        public static string AsSqlInsert<T>(this object poco, string tableName)
        {
            List<PropertyInfo> properties = GetPropertiesForType<T>();

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"INSERT INTO {tableName}");
            sb.Append("(");
            foreach (PropertyInfo property in properties)
            {
                bool skipData = GetSkipDataAttributeValue(property);

                if (!skipData)
                {
                    if (!property.GetValue(poco).IsNull())
                    {
                        sb.Append($"[{property.Name}],");
                    }
                }
            }

            sb.Remove(sb.Length - 1, 1);
            sb.AppendLine(")");
            sb.AppendLine(" VALUES ");
            sb.Append("(");

            foreach (PropertyInfo property in properties)
            {
                bool skipData = GetSkipDataAttributeValue(property);

                if (!skipData)
                {
                    if (!property.GetValue(poco).IsNull())
                    {
                        if (property.PropertyType == typeof(double) ||
                            property.PropertyType == typeof(decimal) ||
                            property.PropertyType == typeof(float) ||
                            property.PropertyType == typeof(int) ||
                            property.PropertyType == typeof(long))
                        {
                            sb.Append($"'{property.GetValue(poco).AsString()}', {Environment.NewLine}");
                        }
                        else if (property.PropertyType == typeof(bool))
                        {
                            bool bvalue = property.GetValue(poco).AsBool();
                            sb.Append(bvalue ? $"1, {Environment.NewLine}" : $"0, {Environment.NewLine}");
                        }
                        else //string
                        {
                            sb.Append($"'{PrepValue(property.GetValue(poco).AsString())}', {Environment.NewLine}");
                        }
                    }
                }
            }

            sb.Remove(sb.Length - 4, 4);
            sb.AppendLine($");");
            return sb.ToString();
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
        /// This method converts a DataTable to a List of type string having comma separated values.
        /// </summary>
        /// <param name="table">the data table</param>
        /// <returns>List of type string</returns>
        public static List<string> ToLines(this DataTable table)
        {
            if (table.IsEmpty()) return null;
            List<string> list = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                string line = string.Empty;
                foreach (DataColumn column in table.Columns)
                {
                    line += $"{row[column.ColumnName].AsString()},";
                }
                list.Add(line);
            }
            return list;
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

        /// <summary>
        /// This method will determine if a DataSet contains and data.
        /// </summary>
        /// <param name="ds">DataSet object</param>
        /// <returns></returns>
        public static bool IsEmpty(this DataSet ds)
        {
            if (ds == null) return true;
            if (ds.Tables == null) return true;
            if (ds.Tables.Count == 0) return true;
            if (ds.Tables[0].Rows == null) return true;
            if (ds.Tables[0].Rows.Count == 0) return true;
            return false;
        }

    }
}
