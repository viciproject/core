#region License
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
#endregion

using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vici.Core.Parser;

namespace Vici.Core.Test
{
    [TestClass]
    public class DynamicObjectTest
    {
        [TestMethod]
        public void TestPropertyOfLinkedObject()
        {
            var obj = new {Test = "XXX"};

            DynamicObject viewData = new DynamicObject(obj);

            object value;
            Type type;

            Assert.AreEqual("XXX", viewData["Test"]);
            Assert.IsTrue(viewData.TryGetValue("Test", out value, out type));

            Assert.IsInstanceOfType(value, typeof(string));
            Assert.AreEqual(typeof(string), type);
            Assert.AreEqual("XXX",value);

            Assert.IsFalse(viewData.TryGetValue("Test2", out value, out type));

        }

        [TestMethod]
        public void TestPropertyOfMultipleLinkedObject()
        {
            var obj1 = new { Test = "XXX" };
            var obj2 = new { Value = 15.5m };

            DynamicObject viewData = new DynamicObject(obj1,obj2);

            object value;
            Type type;

            Assert.AreEqual("XXX", viewData["Test"]);
            Assert.AreEqual(15.5m, viewData["Value"]);
            Assert.IsTrue(viewData.TryGetValue("Test", out value, out type));

            Assert.IsInstanceOfType(value, typeof (string));
            Assert.AreEqual(typeof(string), type);
            Assert.AreEqual("XXX", value);

            Assert.IsTrue(viewData.TryGetValue("Value", out value, out type));

            Assert.IsInstanceOfType(value, typeof(decimal));
            Assert.AreEqual(typeof(decimal), type);
            Assert.AreEqual(15.5m, value);

            Assert.IsFalse(viewData.TryGetValue("Test2", out value, out type));
            Assert.IsFalse(viewData.TryGetValue("Value2", out value, out type));

        }

        [TestMethod]
        public void TestPropertyOfDictionaryEntry()
        {
            DynamicObject viewData = new DynamicObject();

            viewData["Test"] = "XXX";

            object value;
            Type type;

            Assert.AreEqual("XXX", viewData["Test"]);
            Assert.IsTrue(viewData.TryGetValue("Test", out value, out type));

            Assert.IsInstanceOfType(value, typeof(string));
            Assert.AreEqual(typeof(string), type);
            Assert.AreEqual("XXX", value);

            Assert.IsFalse(viewData.TryGetValue("Test2", out value, out type));

        }

        [TestMethod]
        public void TestApply()
        {
            var obj1 = new { Test = "XXX" };
            var obj2 = new { Value = 15.5m };

            DynamicObject viewData1 = new DynamicObject(obj1);
            DynamicObject viewData2 = new DynamicObject(obj2);

            DynamicObject viewData = new DynamicObject();

            viewData.Apply(viewData1);
            viewData.Apply(viewData2);

            object value;
            Type type;


            Assert.AreEqual("XXX", viewData["Test"]);
            Assert.AreEqual(15.5m, viewData["Value"]);
            Assert.IsTrue(viewData.TryGetValue("Test", out value, out type));

            Assert.IsInstanceOfType(value, typeof(string));
            Assert.AreEqual(typeof(string), type);
            Assert.AreEqual("XXX", value);

            Assert.IsTrue(viewData.TryGetValue("Value", out value, out type));

            Assert.IsInstanceOfType(value, typeof(decimal));
            Assert.AreEqual(typeof(decimal), type);
            Assert.AreEqual(15.5m, value);

            Assert.IsFalse(viewData.TryGetValue("Test2", out value, out type));
            Assert.IsFalse(viewData.TryGetValue("Value2", out value, out type));

        }

    }
}