using System;
using System.Collections.Generic;
using System.Linq;
using Vici.Core.StringExtensions;

namespace Vici.Core.Json
{
    public class JsonObject
    {
        private readonly object _value;
        private readonly bool _empty;

        internal JsonObject(object value = null, bool empty = false)
        {
            _value = value;
            _empty = empty;
        }

        public bool IsObject { get { return _value is Dictionary<string, JsonObject>; }}
        public bool IsArray { get { return _value is JsonObject[]; } }
        public bool IsValue { get { return !IsObject && !IsArray; }}
        public bool IsEmpty { get { return _empty; }}

        public object As(Type type)
        {
            return _value.Convert(type);
        }

        public T As<T>()
        {
            return _value.Convert<T>();
        }

        public JsonObject[] AsArray()
        {
            return _value as JsonObject[];
        }

        public T[] AsArray<T>()
        {
            if (!IsArray)
                return null;

            return AsArray().Select(x => x.As<T>()).ToArray();
        }

        public Dictionary<string,JsonObject> AsDictionary() { return _value as Dictionary<string,JsonObject>; } 

        public string[] Keys
        {
            get { return IsObject ? AsDictionary().Keys.ToArray() : new string[0]; }
        }

        private JsonObject Get(string key)
        {
            return IsObject ? ValueForExpression(this,key) : null;
        }

        public JsonObject this[string key]
        {
            get
            {
                var value = Get(key);

                return value ?? new JsonObject(empty:true);
            }
        }

        private static IEnumerable<string> AllKeyParts(string key)
        {
            int index = 0;

            for (; ; )
            {
                int dotIndex = key.IndexOf('.', index);

                if (dotIndex < 0)
                {
                    yield return key;
                    break;
                }

                yield return key.Substring(0, dotIndex);

                index = dotIndex + 1;
            }
        }

        private static JsonObject ValueForExpression(JsonObject obj, string key)
        {
            foreach (var keyPart in AllKeyParts(key).Reverse().ToArray())
            {
                var dic = obj.AsDictionary();

                if (dic.ContainsKey(keyPart))
                {
                    var value = dic[keyPart];

                    if (keyPart.Length == key.Length)
                        return value;

                    string key2 = key.Substring(keyPart.Length + 1);

                    return ValueForExpression(value,key2);
                }

            }

            return null;
        }


    }
}