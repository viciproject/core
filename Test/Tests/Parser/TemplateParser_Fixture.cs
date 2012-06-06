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
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vici.Core.Parser;

namespace Vici.Core.Test
{
    public class VelocityTemplateParser : TemplateParser<Parser.Config.Velocity> { }
	public class CurlyTemplateParser : TemplateParser<Parser.Config.DoubleCurly> { }
    public class ProMeshTemplateParser : TemplateParser<Parser.Config.ProMesh> {}

    [TestClass]
    public class TemplateParser_Fixture
    {
        readonly TemplateParser curlyParser = new CurlyTemplateParser();
        readonly TemplateParser velocityParser = new VelocityTemplateParser();
        readonly TemplateParser xmlParser = new TemplateParser<Parser.Config.Xml>();
        readonly TemplateParser promeshParser = new ProMeshTemplateParser();

        readonly CSharpContext context = new CSharpContext();

        private class TestClass
        {
            public int Int1;
            public string String1;

            public TestClass(int int1, string string1)
            {
                Int1 = int1;
                String1 = string1;
            }

            public static string Format(string s, object x)
            {
                return string.Format(s, x);
            }
        }

        [TestInitialize]
        public void Setup()
        {
            context.Set("doubleValue", 20.5);
            context.Set("intList", new int[] { 3, 4, 5 });
            context.Set("objList" , new TestClass[] { new TestClass(1,"X1"), new TestClass(5,"X5") });
            context.Set("intVar",5);
            //context.AddType("String",typeof(string));
            context.AddType("TestClass",typeof(TestClass));
        }

        [TestMethod]
        public void SmartExpression1()
        {
            Assert.AreEqual("5", velocityParser.Render("$intVar", context));
        }

        [TestMethod]
        public void SmartExpression2()
        {
            Assert.AreEqual("5 B", velocityParser.Render("$intVar B", context));
        }

        [TestMethod]
        public void SmartExpression3()
        {
            Assert.AreEqual("X 5 Y", velocityParser.Render("$TestClass.Format(\"X {0} Y\",intVar)", context));
        }

        [TestMethod]
        public void SmartExpression4()
        {
            Assert.AreEqual("X 5 \"Y", velocityParser.Render("$TestClass.Format(\"X {0} \\\"Y\",intVar)", context));
        }


        [TestMethod]
        public void TestForeachInts_Curly()
        {
            string inputCurly = @"{{foreach x in intList}}{{x}}/{{x*2}}/{{end}}..";

            string s = curlyParser.Render(inputCurly, context);

            Assert.AreEqual("3/6/4/8/5/10/..", s);
        }

        [TestMethod]
        public void TestForeachInts_ProMesh()
        {
            string input = @"<!--{{foreach x in intList}}-->{{x}}/{{x*2}}/<!--{{end}}-->..";

            string s = promeshParser.Render(input, context);

            Assert.AreEqual("3/6/4/8/5/10/..", s);
        }

        [TestMethod]
        public void TestForeachInts_Xml()
        {
            string input = @"<foreach var='x' in='intList'>${x}/${x*2}/</foreach>..";

            string s = xmlParser.Render(input, context);

            Assert.AreEqual("3/6/4/8/5/10/..", s);
        }

        [TestMethod]
        public void TestForeachInts_Velocity()
        {
            string input = @"#foreach(x in intList)${x}/${x*2}/#end..";

            string s = velocityParser.Render(input, context);

            Assert.AreEqual("3/6/4/8/5/10/..", s);

            input = @"#{foreach} (x in intList)${x}/${x*2}/#{end}..";

            s = velocityParser.Render(input, context);

            Assert.AreEqual("3/6/4/8/5/10/..", s);
        }

        [TestMethod]
        public void TestForeachIntsTwice_Velocity()
        {
            string input = @"#foreach(x in intList)${x}/${x*2}/#end.. #foreach(x in intList)${x}/${x*2}/#end";

            string s = velocityParser.Render(input, context);

            Assert.AreEqual("3/6/4/8/5/10/.. 3/6/4/8/5/10/", s);
        }

        [TestMethod]
        public void TestForeachIntsTwice_ProMesh()
        {
            string input = @"<!--{{foreach x in intList}}-->{{x}}/{{x*2}}/<!--{{end}}-->.. <!--{{foreach x in intList}}-->{{x}}/{{x*2}}/<!--{{end}}-->";

            string s = promeshParser.Render(input, context);

            Assert.AreEqual("3/6/4/8/5/10/.. 3/6/4/8/5/10/", s);
        }

        [TestMethod]
        public void TestDSDFSDFSDF()
        {
            IParserContext vars = new FlexContext();

            List<string> items = new List<string>();
            items.Add("A");
            items.Add("B");

            vars.Set("Items",items);

            string input = @"#foreach (item in Items)${item}#end#foreach (item in Items)${item}#end";

            string s = velocityParser.Render(input, vars);

            Assert.AreEqual("ABAB",s);
        }


        [TestMethod]
        public void TestIf_Curly()
        {
            IParserContext newContext = context.CreateLocal();

            string inputCurly = @"{{if A == 5}}X{{else}}Y{{end}}";

            newContext.Set("A",5);

            string s = curlyParser.Render(inputCurly, newContext);

            Assert.AreEqual("X", s);

            newContext.Set("A", 6);

            s = curlyParser.Render(inputCurly, newContext);

            Assert.AreEqual("Y", s);

        }

        [TestMethod]
        public void TestIf_ProMesh()
        {
            IParserContext newContext = context.CreateLocal();

            string input = @"<!--{{ if A == 5 }}-->X<!--{{ else }}-->Y<!--{{ end }}";

            newContext.Set("A", 5);

            string s = promeshParser.Render(input, newContext);

            Assert.AreEqual("X", s);

            newContext.Set("A", 6);

            s = promeshParser.Render(input, newContext);

            Assert.AreEqual("Y", s);

            input = @"<!--$[ if A == 5 ]-->X<!--$[ else ]-->Y<!--$[ end ]";

            newContext.Set("A", 5);

            s = promeshParser.Render(input, newContext);

            Assert.AreEqual("X", s);

            newContext.Set("A", 6);

            s = promeshParser.Render(input, newContext);

            Assert.AreEqual("Y", s);

        }

        [TestMethod]
        public void TestEscapes_Xml()
        {
            IParserContext vars = new FlexContext();

            string input = "$X";

            vars.Set("X","A>B");

            string s = xmlParser.Render(input, vars);

            Assert.AreEqual("A&gt;B",s);
        }

        [TestMethod]
        public void TestIf_Xml()
        {
            IParserContext newContext = context.CreateLocal();

            string inputString = @"<?xml?>$A<if condition='A == 5'>X<else/>Y</if>";

            newContext.Set("A", 5);

            string s = xmlParser.Render(inputString, newContext);

            Assert.AreEqual("<?xml?>5X", s);

            newContext.Set("A", 6);

            s = xmlParser.Render(inputString, newContext);

            Assert.AreEqual("<?xml?>6Y", s);
        }

        [TestMethod]
        public void TestIf2_Xml()
        {
            IParserContext newContext = context.CreateLocal();

            string inputString = @"<?xml?>$A<if condition='B > 5'>X<else/>Y</if>";

            newContext.Set("A", 5);
            newContext.Set("B", 4);

            string s = xmlParser.Render(inputString, newContext);

            Assert.AreEqual("<?xml?>5Y", s);

            newContext.Set("B", 6);

            s = xmlParser.Render(inputString, newContext);

            Assert.AreEqual("<?xml?>5X", s);
        }

        [TestMethod]
        public void TestForeachObjs_Curly()
        {
            string inputCurly = @"{{foreach x in objList}}{{x.Int1}}/{{x.String1}}/{{end}}";

            string s = curlyParser.Render(inputCurly, context);

            Assert.AreEqual("1/X1/5/X5/", s);
        }

        [TestMethod]
        public void TestNumericRange_Curly()
        {
            string inputCurly = @"{{ foreach x in (1...5) }}{{x}}/{{end}}";

            string s = curlyParser.Render(inputCurly, context);

            Assert.AreEqual("1/2/3/4/5/", s);
        }

        [TestMethod]
        public void TestNumericRangeReverse_Curly()
        {
            string inputCurly = @"{{ foreach x in [5...1] }}{{x}}/{{end}}";

            string s = curlyParser.Render(inputCurly, context);

            Assert.AreEqual("5/4/3/2/1/", s);
        }

        [TestMethod]
        public void Test_FormattedExpression_Curly()
        {
            string inputCurly = @"{{doubleValue ` 0.00}}";

            string s = curlyParser.Render(inputCurly, context);

            Assert.AreEqual("20.50", s);
        }

        [TestMethod]
        public void Test_FormattedExpression_ProMesh()
        {
            string input = @"{{doubleValue ` 0.00}}";

            string s = promeshParser.Render(input, context);

            Assert.AreEqual("20.50", s);
        }

        [TestMethod]
        public void Test_Macro1_Curly()
        {
            string inputCurly = @"{{macro TestMacro}}{{x*2}}{{end}}{{call TestMacro @x=5}},{{call TestMacro @x=7}}";

            string s = curlyParser.Render(inputCurly, context);

            Assert.AreEqual("10,14", s);
        }

        [TestMethod]
        public void Test_Macro1_ProMesh()
        {
            string input = @"<!--{{macro TestMacro}}-->{{x*2}}<!--{{end}}--><!--{{call TestMacro @x=5}}-->,<!--{{ call TestMacro @x=7 }}-->";

            string s = promeshParser.Render(input, context);

            Assert.AreEqual("10,14", s);
        }

        [TestMethod]
        public void Test_Macro2_Curly()
        {
            string input = @"{{macro TestMacro}}{{x*2}}({{call AnotherMacro @y=x}}){{end}}{{call TestMacro @x=5}},{{call TestMacro @x=7}}{{macro AnotherMacro}}{{y*5}}{{end}}";

            string s = curlyParser.Render(input, context);

            Assert.AreEqual("10(25),14(35)", s);
        }

        [TestMethod]
        public void Test_Empty_Curly()
        {
            string inputCurly = @"";

            string s = curlyParser.Render(inputCurly, context);

            Assert.AreEqual("", s);
        }

        [TestMethod]
        public void Test_Empty_ProMesh()
        {
            string inputString = @"";

            string s = promeshParser.Render(inputString, context);

            Assert.AreEqual("", s);
        }


    }
}
