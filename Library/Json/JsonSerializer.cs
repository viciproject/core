using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Vici.Core.Json
{
    public enum JsonDateFormat
    {
        SlashDate,
        EscapedSlashDate,
        NewDate,
        Date,
        UtcISO,
        LocalISO
    }

    public class JsonSerializer
    {
        private readonly StringBuilder _output = new StringBuilder();
        private readonly JsonDateFormat _dateFormat;

        public JsonSerializer()
        {
            _dateFormat = JsonDateFormat.UtcISO;
        }

        public JsonSerializer(JsonDateFormat dateFormat)
        {
            _dateFormat = dateFormat;
        }

        public static string ToJson(object obj,JsonDateFormat dateFormat)
        {
            return new JsonSerializer(dateFormat).ConvertToJson(obj);
        }

        public static string ToJson(object obj)
        {
            return new JsonSerializer().ConvertToJson(obj);
        }

        private string ConvertToJson(object obj)
        {
            WriteValue(obj);

            return _output.ToString();
        }

        private void WriteValue(object obj)
        {
            if (obj == null)
                _output.Append("null");
            else if (obj is sbyte || obj is byte || obj is short || obj is ushort || obj is int || obj is uint || obj is long || obj is ulong || obj is decimal || obj is double || obj is float)
                _output.Append(Convert.ToString(obj, NumberFormatInfo.InvariantInfo));
            else if (obj is bool)
                _output.Append(obj.ToString().ToLower());
            else if (obj is char || obj is Enum || obj is Guid)
                WriteString("" + obj);
            else if (obj is DateTime)
                WriteDate((DateTime) obj);
            else if (obj is string)
                WriteString((string)obj);
            else if (obj is IDictionary)
                WriteDictionary((IDictionary)obj);
            else if (obj is ICollection)
                WriteArray((IEnumerable)obj);
            else
                WriteObject(obj);
        }

        private void WriteDate(DateTime date)
        {
            long ticks = ((long)(date.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds);

            switch (_dateFormat)
            {
                case JsonDateFormat.NewDate: 
                    _output.AppendFormat("new Date({0})",ticks);
                    break;

                case JsonDateFormat.Date:
                    _output.AppendFormat("\"Date({0})\"", ticks);
                    break;

                case JsonDateFormat.SlashDate:
                    _output.AppendFormat("\"/Date({0})/\"", ticks);
                    break;

                case JsonDateFormat.EscapedSlashDate:
                    _output.AppendFormat("\"\\/Date({0})\\/\"", ticks);
                    break;

                case JsonDateFormat.UtcISO:
                    _output.AppendFormat("\"{0}\"", date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"));
                    break;

                case JsonDateFormat.LocalISO:
                    _output.AppendFormat("\"{0}\"", date.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:ss"));
                    break;
            }
            
        }

        private void WriteObject(object obj)
        {
            _output.Append('{');

            bool pendingSeparator = false;

            foreach (FieldInfo field in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (pendingSeparator)
                    _output.Append(',');

                WritePair(field.Name, field.GetValue(obj));

                pendingSeparator = true;
            }

            foreach (PropertyInfo property in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!property.CanRead)
                    continue;

                if (pendingSeparator)
                    _output.Append(',');

                WritePair(property.Name, property.GetValue(obj, null));

                pendingSeparator = true;
            }

            _output.Append('}');
        }

        private void WritePair(string name, object value)
        {
            WriteString(name);

            _output.Append(':');

            WriteValue(value);
        }

        private void WriteArray(IEnumerable array)
        {
            _output.Append('[');

            bool pendingSeperator = false;

            foreach (object obj in array)
            {
                if (pendingSeperator)
                    _output.Append(',');

                WriteValue(obj);

                pendingSeperator = true;
            }

            _output.Append(']');
        }

        private void WriteDictionary(IDictionary dic)
        {
            _output.Append('{');

            bool pendingSeparator = false;

            foreach (DictionaryEntry entry in dic)
            {
                if (pendingSeparator)
                    _output.Append(',');

                WritePair(entry.Key.ToString(), entry.Value);

                pendingSeparator = true;
            }

            _output.Append('}');
        }

        private void WriteString(string s)
        {
            _output.Append('\"');

            foreach (char c in s)
            {
                switch (c)
                {
                    case '\t': _output.Append("\\t"); break;
                    case '\r': _output.Append("\\r"); break;
                    case '\n': _output.Append("\\n"); break;
                    case '"':
                    case '\\': _output.Append("\\" + c); break;
                    default:
                        {
                            if (c >= ' ' && c < 128)
                                _output.Append(c);
                            else
                                _output.Append("\\u" + ((int)c).ToString("X4"));
                        }
                        break;
                }
            }

            _output.Append('\"');
        }
    }
}
