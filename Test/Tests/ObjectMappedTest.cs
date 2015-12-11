using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Markup;
using NUnit.Framework;
using Vici.Core.Parser;

namespace Vici.Core.Test
{
    [TestFixture]
    public class ObjectMappedTest
    {
        private class _MappableClass
        {
            public int IntField;
            public int? IntFieldNullable;
            public int IntProperty { get; set; }
            public string StringField;
            public string StringProperty { get; set; }
            public DateTime DateField;
            public DateTime DateProperty { get; set; }
            public Stream UnmappableField = Stream.Null;
        }

        private T X<T>(T x)
        {
            return x;
        }

        Func<string, Tuple<object, bool>> f = key =>
        {
            switch (key)
            {
                case "IntField": return new Tuple<object, bool>(101, true);
                case "IntFieldNullable": return new Tuple<object, bool>("A", true);
                case "IntProperty": return new Tuple<object, bool>(102, true);
                case "StringField": return new Tuple<object, bool>("A", true);
                case "StringProperty": return new Tuple<object, bool>("B", true);
                case "DateField": return new Tuple<object, bool>(100, true);

            }

            return new Tuple<object, bool>(null, false);
        };

        private Dictionary<string, object> dic = new Dictionary<string, object>()
        {
            {"IntField", 101},
            {"IntFieldNullable", "A"},
            {"IntProperty", 102},
            {"StringField", "A"},
            {"StringProperty", "B"},
            {"DateField", 100},
        };

        private Dictionary<string, object> dicLower = new Dictionary<string, object>()
        {
            {"intField", 101},
            {"intFieldNullable", "A"},
            {"intProperty", 102},
            {"stringField", "A"},
            {"stringProperty", "B"},
            {"dateField", 100},
        };
    
        [Test]
        public void ByCallback()
        {
            ObjectMapper mapper = new ObjectMapper();

            _MappableClass obj;

            obj = mapper.CreateObject<_MappableClass>(f);

            Assert.AreEqual(101,obj.IntField);
            Assert.AreEqual(null,obj.IntFieldNullable);
            Assert.AreEqual(102,obj.IntProperty);
            Assert.AreEqual("A",obj.StringField);
            Assert.AreEqual("B",obj.StringProperty);
            Assert.AreEqual(new DateTime(1970,1,1).AddSeconds(100), obj.DateField);
            Assert.AreEqual(DateTime.MinValue, obj.DateProperty);
            Assert.AreEqual(Stream.Null, obj.UnmappableField);
        }

        [Test]
        public void ByDictionary()
        {
            ObjectMapper mapper = new ObjectMapper();

            _MappableClass obj = mapper.CreateObject<_MappableClass>(dic);

            Assert.AreEqual(101, obj.IntField);
            Assert.AreEqual(null, obj.IntFieldNullable);
            Assert.AreEqual(102, obj.IntProperty);
            Assert.AreEqual("A", obj.StringField);
            Assert.AreEqual("B", obj.StringProperty);
            Assert.AreEqual(new DateTime(1970, 1, 1).AddSeconds(100), obj.DateField);
            Assert.AreEqual(DateTime.MinValue, obj.DateProperty);
            Assert.AreEqual(Stream.Null, obj.UnmappableField);
        }

        [Test]
        public void MismatchedCase_Fail()
        {
            ObjectMapper mapper = new ObjectMapper();

            _MappableClass obj = mapper.CreateObject<_MappableClass>(dicLower);

            Assert.AreEqual(0, obj.IntField);
            Assert.AreEqual(null, obj.IntFieldNullable);
            Assert.AreEqual(0, obj.IntProperty);
            Assert.AreEqual(null, obj.StringField);
            Assert.AreEqual(null, obj.StringProperty);
            Assert.AreEqual(DateTime.MinValue, obj.DateField);
            Assert.AreEqual(DateTime.MinValue, obj.DateProperty);
            Assert.AreEqual(Stream.Null, obj.UnmappableField);
        }

        [Test]
        public void MismatchedCase_CaseInsensitive()
        {
            ObjectMapper mapper = new ObjectMapper(ignoreCase: true);

            _MappableClass obj = mapper.CreateObject<_MappableClass>(dicLower);

            Assert.AreEqual(101, obj.IntField);
            Assert.AreEqual(null, obj.IntFieldNullable);
            Assert.AreEqual(102, obj.IntProperty);
            Assert.AreEqual("A", obj.StringField);
            Assert.AreEqual("B", obj.StringProperty);
            Assert.AreEqual(new DateTime(1970, 1, 1).AddSeconds(100), obj.DateField);
            Assert.AreEqual(DateTime.MinValue, obj.DateProperty);
            Assert.AreEqual(Stream.Null, obj.UnmappableField);
        }



    }
}