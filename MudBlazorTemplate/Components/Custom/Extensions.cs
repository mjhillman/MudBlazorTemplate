using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace MudBlazorTemplate.Components.Custom
{
    /// <summary>
    /// This class contains an extension to convert a DataTable to a List of T
    /// </summary>
    public static class Extensions
    {
        //DATA EXTENSIONS **********************************************************************************

        public static Int32 GetAge(this DateTime dateOfBirth)
        {
            var today = DateTime.Today;

            var a = (today.Year * 100 + today.Month) * 100 + today.Day;
            var b = (dateOfBirth.Year * 100 + dateOfBirth.Month) * 100 + dateOfBirth.Day;

            return (a - b) / 10000;
        }

        //OBJECT EXTENSIONS ********************************************************************************

        /// <summary>
        /// This method checks if an object value is DBNull or Null
        /// </summary>
        /// <param name="value">the object value to test.</param>
        /// <returns>true if value is DBNull or Null; otherwise, false.</returns>
        /// When you use instance method syntax to call this method, omit the parameter
        public static bool IsNull(this object value)
            {
                return value == null || Convert.IsDBNull(value);
            }

        /// <summary>
        /// This method checks if an object is a List
        /// </summary>
        /// <param name="value">the object value to test.</param>
        /// <returns>true if value is a List; otherwise, false.</returns>
        /// When you use instance method syntax to call this method, omit the parameter
        public static bool IsList(this object value)
        {
            return value is IList &&
                   value.GetType().IsGenericType &&
                   value.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        /// <summary>
        /// This method checks if an object is a Dictionary
        /// </summary>
        /// <param name="value">the object value to test.</param>
        /// <returns>true if value is a Dictionary; otherwise, false.</returns>
        /// When you use instance method syntax to call this method, omit the parameter
        public static bool IsDictionary(this object value)
        {
            return value is IDictionary &&
                value.GetType().IsGenericType &&
                value.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
        }

        /// <summary>
        /// This method returns the string value of an object.
        /// </summary>
        /// <param name="value">the object to test.</param>
        /// <returns>the value as a string otherwise; the string.Empty</returns>
        /// When you use instance method syntax to call this method, omit the parameter
        public static string AsString(this object value)
        {
            return value.IsNull() ? string.Empty : value.ToString();
        }

        /// <summary>
        /// This method returns the string value of an object.
        /// </summary>
        /// <param name="value">the object to test.</param>
        /// <param name="defaultStringValue">the string value to return if the object is empty</param>
        /// <returns>the value as a string otherwise; the defaultStringValue</returns>
        /// When you use instance method syntax to call this method, omit the parameter
        public static string AsString(this object value, string defaultStringValue)
        {
            return value.IsNull() ? defaultStringValue : value.ToString();
        }

        /// <summary>
        /// This method converts an object to an integer.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns 0.
        /// </remarks>
        public static int AsInt(this object value)
        {
            return value.AsString().AsInt();
        }

        /// <summary>
        /// This method converts an object to a long.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns 0.
        /// </remarks>
        public static long AsLong(this object value)
        {
            return value.AsString().AsLong();
        }

        /// <summary>
        /// This method converts an object to a Decimal.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns 0.
        /// </remarks>
        public static Decimal AsDecimal(this object value)
        {
            return value.AsString().As<Decimal>();
        }

        /// <summary>
        /// This method converts an object to a Double.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns 0.
        /// </remarks>
        public static Double AsDouble(this object value)
        {
            return value.AsString().As<Double>();
        }

        /// <summary>
        /// This method converts an object to a Single.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns 0.
        /// </remarks>
        public static Single AsSingle(this object value)
        {
            return value.AsString().As<Single>();
        }

        /// <summary>
        /// This method converts an object to a Float.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns 0.
        /// </remarks>
        public static float AsFloat(this object value)
        {
            return value.AsString().AsFloat();
        }

        /// <summary>
        /// This method converts an object to a bool.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns the false.
        /// </remarks>
        public static bool AsBool(this object value)
        {
            return value.AsString().AsBool();
        }

        /// <summary>
        /// This method converts a type of T to type of T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]   //hides method from Intellisense only for DLL reference, not project reference.
        public static T As<T>(this object value)
        {
            Type t = typeof(T);

            // Get the type that was made nullable.
            Type valueType = Nullable.GetUnderlyingType(typeof(T));

            if (valueType != null)
            {
                // nullable type.

                if (value == null)
                {
                    // you may want to do something different here.
                    return default(T);
                }
                else
                {
                    // Convert to the value type.
                    object result = Convert.ChangeType(value, valueType);

                    // Cast the value type to the nullable type.
                    return (T)result;
                }
            }
            else
            {
                // Not nullable.
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }

        /// <summary>
        /// This method will convert object properties to a dictionary.
        /// </summary>
        /// <param name="source">the object</param>
        /// <param name="bindingAttr">non default binding properties</param>
        /// <returns>string, object dictionary</returns>
        /// When you use instance method syntax to call this method, omit the parameter
        public static IDictionary<string, object> AsDictionary<T>(this T source, BindingFlags bindingAttr = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );

        }

        /// <summary>
        /// This method will convert a dictionary of object properties to a new populated object.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="source">string, object dictionary</param>
        /// <returns>new object of type T</returns>
        /// When you use instance method syntax to call this method, omit the parameter
        public static T ToObject<T>(this IDictionary<string, object> source) where T : class, new()
        {
            T newObject = new T();
            Type someObjectType = newObject.GetType();

            foreach (KeyValuePair<string, object> item in source)
            {
                someObjectType.GetProperty(item.Key).SetValue(newObject, item.Value, null);
            }

            return newObject;
        }

        /// <summary>
        /// This method will copy the source object to the destination object.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void CopyTo(this object source, object destination)
        {
            // Purpose : Use reflection to set property values of objects that share the same property names.
            Type s = source.GetType();
            Type d = destination.GetType();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            var objSourceProperties = s.GetProperties(flags);
            var objDestinationProperties = d.GetProperties(flags);

            var propertyNames = objSourceProperties
            .Select(c => c.Name)
            .ToList();

            foreach (var properties in objDestinationProperties.Where(properties => propertyNames.Contains(properties.Name)))
            {

                try
                {
                    PropertyInfo piSource = source.GetType().GetProperty(properties.Name);

                    properties.SetValue(destination, piSource.GetValue(source, null), null);
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }

        /// <summary>
        /// This method will compare the properties of two objects.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="source">the source object</param>
        /// <param name="target">the target object</param>
        /// <returns>true if the object properties are not equal</returns>
        /// When you use instance method syntax to call this method, omit the parameter
        public static bool IsDifferentFrom<T>(this T source, T target)
        {
            if (source != null && target == null) return true;
            if (target != null && source == null) return true;
            IDictionary<string, object> aa = source.AsDictionary();
            IDictionary<string, object> bb = target.AsDictionary();
            if (bb.Count != aa.Count) return true;
            foreach (KeyValuePair<string, object> pair in aa)
            {
                string aaString = pair.Value.AsString();
                string bbString = bb[pair.Key].AsString();
                if (aaString != null)
                {
                    if (!aaString.Equals(bbString, StringComparison.CurrentCulture))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// This method returns a json string for the given POCO object.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SerializeToJson(this object value)
        {
            if (value == null) return null;
            JsonSerializerOptions options = new JsonSerializerOptions() { WriteIndented = true };
            return JsonSerializer.Serialize(value, options);
        }

        /// <summary>
        /// This method return true if the DateTime value falls within the supplied date/time range, inclusive of the starting and end times.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static bool IsBetween(DateTime value, DateTime startDate, DateTime endDate)
        {
            return (value >= startDate && value <= endDate);
        }

        //STRING EXTENSIONS **********************************************************************************

        /// <summary>
        /// This method checks if a string value is DBNull, Null or Whitespace.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if value is DBNull, Null or Whitespace; otherwise, false.</returns>
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// This method strips all non-digit characters from a string.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>new string</returns>
        public static string GetDigits(this string value)
        {
            return new string(value.Where(c => char.IsDigit(c)).ToArray());
        }

        /// <summary>
        /// This method returns just the letters (upper and lower case) from a string.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>new string</returns>
        public static string GetLetters(this string value)
        {
            return new string(value.Where(c => char.IsLetter(c)).ToArray());
        }

        /// <summary>
        /// This method returns just the letters and white space from a string.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>new string</returns>
        public static string GetLettersAndSpaces(this string value)
        {
            return new string(value.Where(c => char.IsLetter(c) || char.IsWhiteSpace(c)).ToArray());
        }

        /// <summary>
        /// This method returns just the letters and digits from a string.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>new string</returns>
        public static string GetLetterOrDigit(this string value)
        {
            return new string(value.Where(c => char.IsLetterOrDigit(c)).ToArray());
        }

        /// <summary>
        /// This method will return only the characters of the string containing letter, numbers whitespace and punctuation.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>new string</returns>
        public static string GetSafeString(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            byte[] bytes = Encoding.Default.GetBytes(value);
            value = Encoding.UTF8.GetString(bytes);
            return new string(value.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c) || char.IsControl(c)).ToArray());
        }

        /// <summary>
        /// This method returns string.Empty if the value is DBNull, Null or Whitespace.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>string.Empty if value is DBNull, Null or Whitespace; otherwise, the value.</returns>
        public static string AsString(this string value)
        {
            return value.IsEmpty() ? string.Empty : value;
        }

        /// <summary>
        /// This method converts a string to an integer.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns 0.
        /// </remarks>
        public static int AsInt(this string value)
        {
            return value.AsInt(0);
        }

        /// <summary>
        /// This method converts a string to an integer and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if value is null or invalid.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static int AsInt(this string value, int defaultValue)
        {
            int result;
            if (!int.TryParse(value, out result))
                return defaultValue;
            return result;
        }

        /// <summary>
        /// This method converts a string to a long.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns 0.
        /// </remarks>
        public static long AsLong(this string value)
        {
            return value.AsLong(0);
        }

        /// <summary>
        /// This method converts a string to a long and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if value is null or invalid.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static long AsLong(this string value, int defaultValue)
        {
            long result;
            if (!long.TryParse(value, out result))
                return defaultValue;
            return result;
        }

        /// <summary>
        /// This method converts a string to a Decimal.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns 0.
        /// </remarks>
        public static Decimal AsDecimal(this string value)
        {
            return value.As<Decimal>();
        }

        /// <summary>
        /// This method converts a string to a Decimal number and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if value is null or invalid.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static Decimal AsDecimal(this string value, Decimal defaultValue)
        {
            decimal result;
            if (!decimal.TryParse(value, out result))
                return defaultValue;
            return result;
        }

        /// <summary>
        /// This method converts a string to a Double.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns 0.
        /// </remarks>
        public static Double AsDouble(this string value)
        {
            return value.As<Double>();
        }

        /// <summary>
        /// This method converts a string to a Double and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if value is null or invalid.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static Double AsDouble(this string value, Double defaultValue)
        {
            Double result;
            if (!Double.TryParse(value, out result))
                return defaultValue;
            return result;
        }

        /// <summary>
        /// This method converts a string to a Single.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns 0.
        /// </remarks>
        public static Single AsSingle(this string value)
        {
            return value.As<Single>();
        }

        /// <summary>
        /// This method converts a string to a Single and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if value is null or invalid.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static Single AsSingle(this string value, Single defaultValue)
        {
            Single result;
            if (!Single.TryParse(value, out result))
                return defaultValue;
            return result;
        }

        /// <summary>
        /// This method converts a string to a Float.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns 0.
        /// </remarks>
        public static float AsFloat(this string value)
        {
            return value.AsFloat(0.0f);
        }

        /// <summary>
        /// This method converts a string to a Float and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if value is null or invalid.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static float AsFloat(this string value, float defaultValue)
        {
            float result;
            if (!float.TryParse(value, out result))
                return defaultValue;
            return result;
        }

        /// <summary>
        /// This method converts a string to a DateTime.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns the default DateTime.
        /// </remarks>
        public static DateTime AsDateTime(this string value)
        {
            return value.AsDateTime(new DateTime());
        }

        /// <summary>
        /// This method converts a string to a DateTime and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if value is null or invalid.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static DateTime AsDateTime(this string value, DateTime defaultValue)
        {
            DateTime result;
            if (!DateTime.TryParse(value, out result))
                return defaultValue;
            return result;
        }

        /// <summary>
        /// This method converts a string to a bool.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>
        /// When you use instance method syntax to call this method, omit the parameter
        /// If you pass a value that cannot be converted to the type, the method returns the false.
        /// </remarks>
        public static bool AsBool(this string value)
        {
            return value.AsBool(false);
        }

        /// <summary>
        /// This method converts a string to a bool and specifies a default value.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if value is null or invalid.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static bool AsBool(this string value, bool defaultValue)
        {
            bool result;

            switch(value)
            {
                case "True":
                case "true":
                case "1":
                    result = true;
                    break;
                default:
                    result = false;
                    break;
            }

            return result;
        }

        /// <summary>
        /// This method converts a string to it's base64 representation.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>base64 encoded string</returns>
        public static string AsBase64Encoded(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(value);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// This method converts a base64 value back to a string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string AsBase64Decoded(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return value;
            byte[] base64EncodedBytes = System.Convert.FromBase64String(value);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        /// This method converts a string to a strongly typed value of the specified data type.
        /// </summary>
        /// <typeparam name="TValue">The data type to convert to.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static TValue As<TValue>(this string value)
        {
            return value.As<TValue>(default(TValue));
        }

        /// <summary>
        /// This method converts a string to the specified data type and specifies a default value.
        /// </summary>
        /// <typeparam name="TValue">The data type to convert to.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <param name="defaultValue">The value to return if value is null.</param>
        /// <returns>The converted value</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static TValue As<TValue>(this string value, TValue defaultValue)
        {
            try
            {
                TypeConverter converter1 = TypeDescriptor.GetConverter(typeof(TValue));
                if (converter1.CanConvertFrom(typeof(string)))
                    return (TValue)converter1.ConvertFrom((object)value);
                TypeConverter converter2 = TypeDescriptor.GetConverter(typeof(string));
                if (converter2.CanConvertTo(typeof(TValue)))
                    return (TValue)converter2.ConvertTo((object)value, typeof(TValue));
            }
            catch
            {
            }
            return defaultValue;
        }

        /// <summary>
        /// This method checks whether a string can be converted to the Boolean (true/false) type.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if value can be converted to the specified type; otherwise, false.</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static bool IsBool(this string value)
        {
            bool result;
            return bool.TryParse(value, out result);
        }

        /// <summary>
        /// This method checks whether a string can be converted to an integer.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if value can be converted to the specified type; otherwise, false.</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static bool IsInt(this string value)
        {
            int result;
            return int.TryParse(value, out result);
        }

        /// <summary>
        /// This method checks whether a string can be converted to an long.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if value can be converted to the specified type; otherwise, false.</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static bool IsLong(this string value)
        {
            long result;
            return long.TryParse(value, out result);
        }

        /// <summary>
        /// This method checks whether a string can be converted to a Decimal.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if value can be converted to the specified type; otherwise, false.</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static bool IsDecimal(this string value)
        {
            decimal result;
            return decimal.TryParse(value, out result);
        }

        /// <summary>
        /// This method checks whether a string can be converted to a float.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if value can be converted to the specified type; otherwise, false.</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static bool IsFloat(this string value)
        {
            float result;
            return float.TryParse(value, out result);
        }

        /// <summary>
        /// This method checks whether a string contains only digit characters.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if value is all digits; otherwise, false.</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static bool IsAllDigits(this string value)
        {
            if (value.IsEmpty()) return false;

            foreach (char c in value)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        /// <summary>
        /// This method checks whether a string can be converted to a single.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if value can be converted to the specified type; otherwise, false.</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static bool IsSingle(this string value)
        {
            Single result;
            return Single.TryParse(value, out result);
        }

        /// <summary>
        /// This method checks whether a string can be converted to a double.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if value can be converted to the specified type; otherwise, false.</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static bool IsDouble(this string value)
        {
            double result;
            return double.TryParse(value, out result);
        }

        /// <summary>
        /// This method checks whether a string can be converted to a DateTime.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if value can be converted to the specified type; otherwise, false.</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static bool IsDateTime(this string value)
        {
            DateTime result;
            return DateTime.TryParse(value, out result);
        }

        /// <summary>
        /// This method checks whether a string can be converted to the specified data type.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if value can be converted to the specified type; otherwise, false.</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter.</remarks>
        public static bool Is<TValue>(this string value)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TValue));
            if (converter != null)
            {
                try
                {
                    if (value != null)
                    {
                        if (!converter.CanConvertFrom((ITypeDescriptorContext)null, value.GetType()))
                        {
                            return false;
                        }
                    }
                    converter.ConvertFrom((ITypeDescriptorContext)null, CultureInfo.CurrentCulture, (object)value);
                    return true;
                }
                catch
                {
                }
            }
            return false;
        }

        /// <summary>
        /// This method checks whether a string contains a valid email address.
        /// </summary>
        /// <param name="value">The string value to test.</param>
        /// <returns>true if a valid email address</returns>
        public static bool IsValidEmailAddress(this string value)
        {
            try
            {
                if (!value.Contains("@")) return false;
                var emailChecked = new System.Net.Mail.MailAddress(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// This method formats an XML document.
        /// </summary>
        /// <param name="doc">the XML document</param>
        /// <returns>formatted XML string</returns>
        public static string AsPrettyXml(this XmlDocument doc)
        {
            StringWriter stringWriter = new StringWriter();

            try
            {
                // Format the XML text.
                XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
                xmlTextWriter.Formatting = Formatting.Indented;
                doc.WriteTo(xmlTextWriter);
                return stringWriter.ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
            finally
            {
                stringWriter.Dispose();
            }
        }

        /// <summary>
        /// This method formats an XML string.
        /// </summary>
        /// <param name="xmlString">the XML string</param>
        /// <returns>formatted XML string</returns>
        public static string AsPrettyXml(this string xmlString)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlString);
            return AsPrettyXml(doc);
        }

        /// <summary>
        /// This method formats an JSON string.
        /// </summary>
        /// <param name="json">the JSON string</param>
        /// <returns>formatted JSON string</returns>
        /// <remarks>https://stackoverflow.com/questions/4580397/json-formatter-in-c/24782322#24782322</remarks>
        public static string AsPrettyJson(this string json)
        {
            if (json.IsEmpty()) return json;

            const string INDENT_STRING = "    ";
            int indentation = 0;
            int quoteCount = 0;

            try
            {
                var result =
                    from ch in json
                    let quotes = ch == '"' ? quoteCount++ : quoteCount
                    let lineBreak = ch == ',' && quotes % 2 == 0 ? ch + Environment.NewLine + string.Concat(Enumerable.Repeat(INDENT_STRING, indentation)) : null
                    let openChar = ch == '{' || ch == '[' ? ch + Environment.NewLine + string.Concat(Enumerable.Repeat(INDENT_STRING, ++indentation)) : ch.ToString()
                    let closeChar = ch == '}' || ch == ']' ? Environment.NewLine + string.Concat(Enumerable.Repeat(INDENT_STRING, --indentation)) + ch : ch.ToString()
                    select lineBreak == null
                                ? openChar.Length > 1
                                    ? openChar
                                    : closeChar
                                : lineBreak;

                return string.Concat(result);
            }
            catch (Exception)
            {
                return json;
            }
        }

        /// <summary>
        /// This method will convert a json string to a POCO object.
        /// </summary>
        /// <typeparam name="T">the object type</typeparam>
        /// <param name="jsonString">the json string</param>
        /// <returns>POCO object of type T</returns>
        /// <remarks>See the method Serialize in the ObjectExtensions class</remarks>
        public static T Deserialize<T>(this string jsonString)
        {
            if (jsonString.IsEmpty()) return default(T);
            return JsonSerializer.Deserialize<T>(jsonString);
        }

        /// <summary>
        /// This method will convert a POCO object to json
        /// </summary>
        /// <param name="dataToSerialize">the POCO object</param>
        /// <returns>json string</returns>
        /// <remarks>When you use instance method syntax to call this method, omit the parameter</remarks> 
        public static string Serialize(this object dataToSerialize)
        {
            if (dataToSerialize == null) return null;
            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(dataToSerialize, options);
        }

        /// <summary>
        /// This method replaces carriage return and line feeds with a space.
        /// </summary>
        /// <param name="value">the string value to trim</param>
        /// <returns>string without carriage returns and line feeds</returns>
        public static string TrimCrLf(this string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                value = value.Replace("\\r\\n", " ")
                            .Replace("\\r", " ")
                            .Replace("\\n", " ")
                            .Replace("\r\n", " ")
                            .Replace("\r", " ")
                            .Replace("\n", " ")
                            .Replace("\\", string.Empty)
                            .Replace("  ", " ")
                            .Trim();
            }

            return value;
        }

    }
}
