//=============================================================================
// Vici Core - Productivity Library for .NET 3.5 
//
// Copyright (c) 2008-2012 Philippe Leybaert
//
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in 
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
// IN THE SOFTWARE.
//=============================================================================

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Vici.Core.Json;

namespace Vici.Core.Test
{
    [TestFixture]
    public class JSONParserFixture
    {
        private const string _json1 = @"
{ 
    ""name"" :""John Doe"" ,
    ""salary"" : 4500.50
}
";

        private const string _json = @"
{ 
    ""name"" :""John Doe"" ,
    ""salary"" : 4500.20 ,
    ""children"" : [ ""Sarah"", ""Jessica"" ]
}
";

        private const string _json2 = @"

{
    ""glossary"": {
        ""title"": ""example glossary"",
		""GlossDiv"": {
            ""title"": ""S"",
			""GlossList"": {
                ""GlossEntry"": {
                    ""ID"": ""SGML"",
					""SortAs"": ""SGML"",
					""GlossTerm"": ""Standard Generalized Markup Language"",
					""Acronym"": ""SGML"",
					""Abbrev"": ""ISO 8879:1986"",
					""GlossDef"": {
                        ""para"": ""A meta-markup language, used to create markup languages such as DocBook."",
						""GlossSeeAlso"": [""GML"", ""XML""]
                    },
					""GlossSee"": ""markup""
                }
            }
        }
    }
}
";

        private const string _json3 = @"
{""menu"": {
    ""header"": ""SVG Viewer"",
    ""items"": [
        {""id"": ""Open""},
        {""id"": ""OpenNew"", ""label"": ""Open New""},
        null,
        {""id"": ""ZoomIn"", ""label"": ""Zoom In""},
        {""id"": ""ZoomOut"", ""label"": ""Zoom Out""},
        {""id"": ""OriginalView"", ""label"": ""Original View""},
        null,
        {""id"": ""Quality""},
        {""id"": ""Pause""},
        {""id"": ""Mute""},
        null,
        {""id"": ""Find"", ""label"": ""Find...""},
        {""id"": ""FindAgain"", ""label"": ""Find Again""},
        {""id"": ""Copy""},
        {""id"": ""CopyAgain"", ""label"": ""Copy Again""},
        {""id"": ""CopySVG"", ""label"": ""Copy SVG""},
        {""id"": ""ViewSVG"", ""label"": ""View SVG""},
        {""id"": ""ViewSource"", ""label"": ""View Source""},
        {""id"": ""SaveAs"", ""label"": ""Save As""},
        null,
        {""id"": ""Help""},
        {""id"": ""About"", ""label"": ""About Adobe CVG Viewer...""}
    ]
}}
";

        private class Person
        {
            public string name;
            public decimal  salary;
            public string[] children;
        }

        [Test]
        public void SimpleTypedObject()
        {
            Person person = JsonParser.Parse<Person>(_json);

            Assert.AreEqual("John Doe",person.name);
            Assert.AreEqual(4500.20m,person.salary);
            Assert.AreEqual(2,person.children.Length);
            Assert.AreEqual("Sarah", person.children[0]);
            Assert.AreEqual("Jessica", person.children[1]);
        }

        [Test]
        public void ComplexDictionary()
        {
            JsonObject jsonObject = JsonParser.Parse(_json3);

            Assert.IsTrue(jsonObject.IsObject);
            Assert.IsTrue(jsonObject["menu"].IsObject);

            Assert.AreEqual(2, jsonObject["menu"].Keys.Length);

            Assert.AreEqual("SVG Viewer",jsonObject["menu.header"].As<string>());
        }

        [Test]
        public void EmptyArray()
        {
            JsonObject jsonObject = JsonParser.Parse(@"{ ""array"" : [] }")["array"];

            Assert.IsTrue(jsonObject.IsArray);
            Assert.AreEqual(0,jsonObject.AsArray<long>().Length);
        }

        [Test]
        public void TestEscapes()
        {
            JsonObject obj = JsonParser.Parse(@"{ ""obj"" : ""\n"" }");

            obj = obj["obj"];

            string s = JsonParser.Parse(@"{ ""obj"" : ""\n"" }")["obj"].As<string>();

            Assert.AreEqual("\n", s);

            s = JsonParser.Parse(@"{ ""obj"" : ""\t"" }")["obj"].As<string>();

            Assert.AreEqual("\t", s);

            s = JsonParser.Parse(@"{ ""obj"" : ""\\"" }")["obj"].As<string>();

            Assert.AreEqual(@"\", s);

            s = JsonParser.Parse(@"{ ""obj"" : ""\u00aa"" }")["obj"].As<string>();

            Assert.AreEqual("\u00aa", s);
        }

        [Test]
        public void SimpleValues()
        {
            JsonObject jsonObject = JsonParser.Parse("\"x\"");

            Assert.AreEqual("x",jsonObject.As<string>());
        }
    }
}
