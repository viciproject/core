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
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vici.Core.Json;

namespace Vici.Core.Test
{
    [TestClass]
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

        [TestMethod]
        public void SimpleTypedObject()
        {
            JsonParser jsonParser = new JsonParser();

            Person person = jsonParser.Parse<Person>(_json);

            Assert.AreEqual("John Doe",person.name);
            Assert.AreEqual(4500.20m,person.salary);
            Assert.AreEqual(2,person.children.Length);
            Assert.AreEqual("Sarah", person.children[0]);
            Assert.AreEqual("Jessica", person.children[1]);
        }

        [TestMethod]
        public void ComplexDictionary()
        {
            JsonParser jsonParser = new JsonParser();

            object obj = jsonParser.Parse(_json3);

            Assert.IsInstanceOfType(obj,typeof(Dictionary<string,object>));

            Dictionary<string, object> dic = (Dictionary<string, object>) obj;

            Assert.AreEqual(1, dic.Count);

            Assert.IsInstanceOfType(dic["menu"], typeof(Dictionary<string, object>));

            Assert.AreEqual(2, ((Dictionary<string, object>)dic["menu"]).Count);

        }

        [TestMethod]
        public void EmptyArray()
        {
            JsonParser parser = new JsonParser();

            object items = ((Dictionary<string,object>) parser.Parse(@"{ ""array"" : [] }"))["array"];

            Assert.IsInstanceOfType(items,typeof(object[]));
            Assert.AreEqual(0,((object[])items).Length);
        }

        [TestMethod]
        public void TestEscapes()
        {
            JsonParser parser = new JsonParser();

            object item = ((Dictionary<string, object>)parser.Parse(@"{ ""obj"" : ""\n"" }"))["obj"];

            Assert.IsInstanceOfType(item,typeof(string));
            Assert.AreEqual("\n", item);

            item = ((Dictionary<string, object>)parser.Parse(@"{ ""obj"" : ""\t"" }"))["obj"];

            Assert.IsInstanceOfType(item,typeof(string));
            Assert.AreEqual("\t", item);

            item = ((Dictionary<string, object>)parser.Parse(@"{ ""obj"" : ""\\"" }"))["obj"];

            Assert.IsInstanceOfType(item,typeof(string));
            Assert.AreEqual(@"\", item);

            item = ((Dictionary<string, object>)parser.Parse(@"{ ""obj"" : ""\u00aa"" }"))["obj"];

            Assert.IsInstanceOfType(item,typeof(string));
            Assert.AreEqual("\u00aa", item);

        }
    }
}
