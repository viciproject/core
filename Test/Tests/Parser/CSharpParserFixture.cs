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

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vici.Core.Parser;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Vici.Core.Test
{
    [TestClass]
    public class CSharpParserFixture
    {
        private readonly ParserContext _context = new CSharpContext();
        private readonly CScriptParser _parser = new CScriptParser();

        private class DataClass
        {
            public DataClass()
            {
            }

            public DataClass(int int1)
            {
                Int1 = int1;
            }


            public DataClass(string string1, int int1)
            {
                String1 = string1;
                Int1 = int1;
            }

            public string String1;
            public int Int1;

            public int Method0() { return 2; }
            public int Method1(int x) { return x * 2; }
            public int Method2(int x, int y) { return x + y; }

            public static int Static1 = 500;
            public static int Static2 { get { return 501; } }

            public int this[int i] { get { return i * 2; } }
            public int this[int x, int y] { get { return x + y; } }

        }

        [TestInitialize]
        public void SetupFixture()
        {
            DataClass dataObject = new DataClass();

            dataObject.String1 = "blabla1";
            dataObject.Int1 = 123;

            _context.AddType("Math", typeof(Math));
            _context.Set("Data", dataObject);
            _context.AddType("DataClass", typeof(DataClass));
            _context.Set("Func", new Converter<int, int>(Func));
            _context.AddFunction("Max", typeof(Math), "Max");
            _context.AddFunction("fmt", typeof(String), "Format");
            _context.Set("Value10", 10, typeof(int));
            _context.Set("NullableValue5", 5, typeof(int?));
            _context.Set("NullableValueNull", null, typeof(int?));
            _context.Set("MyArray", new int[] { 1, 2, 4, 8, 16 });
            _context.Set("MyArray2", new int[,] { { 1, 2 }, { 2, 4 }, { 4, 8 }, { 8, 16 }, { 16, 32 } });

            _context.Set("f", new Func<int,int>(i => i*2));

            _parser.DefaultContext = _context;
        }

        private static int Func(int i)
        {
            return i * 5;
        }

        [TestMethod]
        public void ComplexExpressions()
        {
            Assert.AreEqual(435, _parser.Evaluate<int>("Math.Max(Data.Method2(Data.Int1+10,300),Data.Method1(Data.Int1))+(\"x\" + 5).Length"));

            Assert.AreEqual(17, _parser.Evaluate<int>("Data.Method2(Data.Method2(3,4),Data.Method1(5))"));
            Assert.AreEqual(100, _parser.Evaluate<int>("Max(Max(100,5),Func(10))"));
            Assert.AreEqual(1000, _parser.Evaluate<int>("Max(Max(100,5),Func(200))"));
        }

        [TestMethod]
        public void DefaultValueExpression()
        {
            IParserContext context = new FlexContext();

            context.Set("a","");
            context.Set("b", "z");


            Assert.AreEqual("x", _parser.Evaluate<string>("a ?: \"x\"",context));
            Assert.AreEqual("z", _parser.Evaluate<string>("b ?: \"x\"", context));
        }

        [TestMethod]
        public void StringExpressions()
        {
            Assert.AreEqual("ab", _parser.Evaluate<string>("string.Concat(\"a\",\"b\")"));
            Assert.AreEqual("ab", _parser.Evaluate<string>("\"a\" + \"b\")"));
        }

        [TestMethod]
        public void StringComparisons()
        {
            StringComparison saved = _context.StringComparison;

            _context.StringComparison = StringComparison.Ordinal;

            Assert.IsTrue(_parser.Evaluate<bool>("\"a\" == \"a\""));
            Assert.IsFalse(_parser.Evaluate<bool>("\"a\" == \"b\""));
            Assert.IsTrue(_parser.Evaluate<bool>("\"a\" != \"b\""));
            Assert.IsFalse(_parser.Evaluate<bool>("\"a\" != \"a\""));

            Assert.IsFalse(_parser.Evaluate<bool>("\"a\" == \"A\""));
            Assert.IsFalse(_parser.Evaluate<bool>("\"a\" == \"B\""));
            Assert.IsTrue(_parser.Evaluate<bool>("\"a\" != \"B\""));
            Assert.IsTrue(_parser.Evaluate<bool>("\"a\" != \"B\""));

            _context.StringComparison = StringComparison.OrdinalIgnoreCase;

            Assert.IsTrue(_parser.Evaluate<bool>("\"a\" == \"A\""));
            Assert.IsFalse(_parser.Evaluate<bool>("\"a\" != \"A\""));

            _context.StringComparison = saved;
        }


        [TestMethod]
        public void MemberMethods()
        {
            Assert.AreEqual(2, _parser.Evaluate<int>("Data.Method0()"));
            Assert.AreEqual(2, _parser.Evaluate<int>("Math.Max(1,2)"));
            Assert.AreEqual(21, _parser.Evaluate<int>("Data.Method0() + Data.Method1(5) + Data.Method2(5,4)"));
        }

        [TestMethod]
        public void CharLiterals()
        {
            Assert.AreEqual('x', _parser.Evaluate<char>("'x'"));
            Assert.AreEqual('\n', _parser.Evaluate<char>("'\\n'"));
            Assert.AreEqual("Test\n", _parser.Evaluate<string>("\"Test\" + '\\n'"));
            Assert.AreEqual('\'', _parser.Evaluate<char>("'\\''"));
            Assert.AreEqual('\x45', _parser.Evaluate<char>("'\\x45'"));
            Assert.AreEqual('\x4545', _parser.Evaluate<char>("'\\x4545'"));
        }

        [TestMethod]
        public void TypeCast()
        {
            Assert.IsInstanceOfType(_parser.EvaluateToObject("(int)5L"),typeof(int) );
        }

        [TestMethod]
        public void StringLiterals()
        {
            Assert.AreEqual("xyz", _parser.Evaluate<string>("\"xyz\""));
            Assert.AreEqual("\n", _parser.Evaluate<string>(@"""\n"""));
            Assert.AreEqual("\f", _parser.Evaluate<string>(@"""\f"""));
            Assert.AreEqual("\"", _parser.Evaluate<string>(@"""\"""""));
            Assert.AreEqual("\x45r\n", _parser.Evaluate<string>(@"""\x45r\n"""));
            Assert.AreEqual("\x45b\n", _parser.Evaluate<string>(@"""\x45b\n"""));
            Assert.AreEqual("\x45bf\n", _parser.Evaluate<string>(@"""\x45bf\n"""));
            Assert.AreEqual("\x45bff\n", _parser.Evaluate<string>(@"""\x45bff\n"""));
        }

        [TestMethod]
        public void NumericLiterals()
        {
            Assert.IsInstanceOfType(_parser.EvaluateToObject("1"), typeof(int));
            Assert.AreEqual(1, _parser.Evaluate<int>("1"));

            Assert.IsInstanceOfType(_parser.EvaluateToObject("10000000000"), typeof(long));
            Assert.AreEqual(10000000000, _parser.Evaluate<long>("10000000000"));

            Assert.IsInstanceOfType(_parser.EvaluateToObject("1m"), typeof(decimal));
            Assert.AreEqual(1m, _parser.Evaluate<decimal>("1m"));

            Assert.IsInstanceOfType(_parser.EvaluateToObject("1L"), typeof(long));
            Assert.AreEqual(1L, _parser.Evaluate<long>("1L"));

            Assert.IsInstanceOfType(_parser.EvaluateToObject("1UL"), typeof(ulong));
            Assert.AreEqual(1UL, _parser.Evaluate<ulong>("1UL"));

            Assert.IsInstanceOfType(_parser.EvaluateToObject("1.0"), typeof(double));
            Assert.AreEqual(1L, _parser.Evaluate<double>("1.0"));

            Assert.IsInstanceOfType(_parser.EvaluateToObject("1.0f"), typeof(float));
            Assert.AreEqual(1L, _parser.Evaluate<float>("1.0f"));

        }

        [TestMethod]
        public void ObjectCreation()
        {
            Assert.IsInstanceOfType(_parser.Evaluate<object>("new DataClass(5)"), typeof(DataClass));

            Assert.AreEqual(5, _parser.Evaluate<int>("(new DataClass(5)).Int1"));
            Assert.AreEqual(5, _parser.Evaluate<int>("new DataClass(5).Int1"));
            Assert.AreEqual(5, _parser.Evaluate<int>("Math.Max(new DataClass(3+2).Int1,3)"));
        }

        [TestMethod]
        public void Delegates()
        {
            Assert.AreEqual(10, _parser.Evaluate<int>("Func(2)"));
            Assert.AreEqual(5, _parser.Evaluate<int>("Max(4,5)"));
        }

        [TestMethod]
        public void Typeof()
        {
            Assert.AreEqual(typeof(int), _parser.Evaluate<Type>("typeof(int)"));
        }

        [TestMethod]
        public void OperatorPrecedence()
        {
            Assert.AreEqual(2, _parser.Evaluate<int>("(5-4)*2"));
            Assert.AreEqual(13, _parser.Evaluate<int>("5+4*2"));
            Assert.AreEqual(22, _parser.Evaluate<int>("5*4+2"));
            Assert.AreEqual(18, _parser.Evaluate<int>("(5+4)*2"));
        }

        [TestMethod]
        public void UnaryNot()
        {
            Assert.AreEqual(false, _parser.Evaluate<bool>("!(1==1)"));
            Assert.AreEqual(true, _parser.Evaluate<bool>("!false"));
            Assert.AreEqual(true, _parser.Evaluate<bool>("!!true"));
        }

        [TestMethod]
        public void UnaryMinus()
        {
            Assert.AreEqual(-2, _parser.Evaluate<int>("-2"));
            Assert.AreEqual(3, _parser.Evaluate<int>("5+-2"));
            Assert.AreEqual(-1, _parser.Evaluate<int>("-(3-2)"));
        }

        [TestMethod]
        public void BitwiseComplement()
        {
            Assert.AreEqual(~2, _parser.Evaluate<int>("~2"));
            Assert.AreEqual(5 + ~2, _parser.Evaluate<int>("5+~2"));
            Assert.AreEqual(~(3 - 2), _parser.Evaluate<int>("~(3 - 2)"));
        }

        [TestMethod]
        public void StaticFields()
        {
            Assert.AreEqual(500, _parser.Evaluate<int>("DataClass.Static1"));
            Assert.AreEqual(501, _parser.Evaluate<int>("DataClass.Static2"));
        }

        [TestMethod]
        public void NullableLifting()
        {
            Assert.AreEqual(15, _parser.Evaluate<int?>("Value10 + NullableValue5"));
            Assert.IsInstanceOfType(_parser.Evaluate<int?>("Value10 + NullableValue5"), typeof(int?));
            Assert.AreEqual(null, _parser.Evaluate<int?>("Value10 + NullableValueNull"));

        }

        [TestMethod]
        public void Indexing()
        {
            Assert.AreEqual(30, _parser.Evaluate<int>("Data[Func(5),5]"));
        }

        [TestMethod]
        public void ArrayIndexing()
        {
            Assert.AreEqual(8, _parser.Evaluate<int>("MyArray[3]"));
            Assert.AreEqual(8, _parser.Evaluate<int>("MyArray2[2,1]"));

            Assert.AreEqual(16, _parser.Evaluate<int>("MyArray[Data.Method0()+2]"));
            Assert.AreEqual(8, _parser.Evaluate<int>("MyArray2[Data.Method0()+1,0]"));

        }

        [TestMethod]
        public void Ternary()
        {

            Assert.AreEqual(1, _parser.Evaluate<int>("true ? 1:2"));
            Assert.AreEqual(2, _parser.Evaluate<int>("false ? 1:2"));

            _context.Set("a", 1);

            Assert.AreEqual(1, _parser.Evaluate<int>("a==1 ? 1 : 2"));
            Assert.AreEqual(2, _parser.Evaluate<int>("a!=1 ? 1 : 2"));
            Assert.AreEqual("1", _parser.Evaluate<string>("a==1 ? \"1\" : \"2\""));
            Assert.AreEqual("2", _parser.Evaluate<string>("a!=1 ? \"1\" : \"2\""));
            Assert.AreEqual(1, _parser.Evaluate<int>("a==1 ? 1 : a==2 ? 2 : a==3 ? 3 : 4"));

            Assert.AreEqual("x", _parser.Evaluate<string>("a==1 ? \"x\" : a==2 ? \"y\" : a==3 ? \"z\" : \"error\""));
            _context.Set("a", 2);
            Assert.AreEqual("y", _parser.Evaluate<string>("a==1 ? \"x\" : a==2 ? \"y\" : a==3 ? \"z\" : \"error\""));
            _context.Set("a", 3);
            Assert.AreEqual("z", _parser.Evaluate<string>("a==1 ? \"x\" : a==2 ? \"y\" : a==3 ? \"z\" : \"error\""));
            _context.Set("a", 56443);
            Assert.AreEqual("error", _parser.Evaluate<string>("a==1 ? \"x\" : a==2 ? \"y\" : a==3 ? \"z\" : \"error\""));

        }

        [TestMethod]
        public void Comparisons()
        {
            Assert.IsTrue(_parser.Evaluate<bool>("1==1"));
            Assert.IsTrue(_parser.Evaluate<bool>("2>=1"));
            Assert.IsTrue(_parser.Evaluate<bool>("2>1"));
            Assert.IsTrue(_parser.Evaluate<bool>("1<2"));
            Assert.IsFalse(_parser.Evaluate<bool>("2<1"));

            _context.Set("NullString", null, typeof(string));
            _context.Set("ShortValue", 4, typeof(short));

            Assert.IsTrue(_parser.Evaluate<bool>("NullString == null"));
            Assert.IsTrue(_parser.Evaluate<bool>("NullableValue5 == 5"));
            Assert.IsTrue(_parser.Evaluate<bool>("ShortValue == 4"));
        }

        [TestMethod]
        public void AsOperator()
        {
            Assert.AreEqual("x", _parser.Evaluate<string>("\"x\" as string"));
            Assert.AreEqual(null, _parser.Evaluate<string>("5 as string"));
        }

        [TestMethod]
        public void IsOperator()
        {
            Assert.IsTrue(_parser.Evaluate<bool>("\"x\" is string"));
            Assert.IsFalse(_parser.Evaluate<bool>("5 is string"));
            Assert.IsFalse(_parser.Evaluate<bool>("null is string"));
        }

        [TestMethod]
        public void NullValueOperator()
        {
            Assert.AreEqual(10, _parser.Evaluate<int>("NullableValueNull ?? 10"));
            Assert.AreEqual(5, _parser.Evaluate<int>("NullableValue5 ?? 10"));
        }

        [TestMethod]
        public void Assignment()
        {
            _context.AssignmentPermissions = AssignmentPermissions.NewVariable;

            Assert.AreEqual(5, _parser.Evaluate<int>("aaa = 5"));

            Assert.IsTrue(_parser.Evaluate<bool>("aaa == 5"));

            _context.AssignmentPermissions = AssignmentPermissions.ExistingVariable;

            Assert.AreEqual(100, _parser.Evaluate<int>("aaa = 100"));
            Assert.IsTrue(_parser.Evaluate<bool>("aaa == 100"));

            _context.AssignmentPermissions = AssignmentPermissions.Variable;

            Assert.AreEqual(200, _parser.Evaluate<int>("aaa = bbb = 100*2"));
            Assert.IsTrue(_parser.Evaluate<bool>("aaa == 200"));
            Assert.IsTrue(_parser.Evaluate<bool>("bbb == 200"));

        }

        [TestMethod]
        //[ExpectedException(typeof(IllegalAssignmentException))]
        public void PropertyAssignmentNotAllowed()
        {
            try
            {
                _context.AssignmentPermissions = AssignmentPermissions.None;

                Assert.AreEqual(123, _parser.Evaluate<int>("Data.Int1 = 123"));

                Assert.Fail();
            }
            catch (IllegalAssignmentException)
            {
                
                
            }
        }


        [TestMethod]
        public void PropertyAssignment()
        {
            _context.AssignmentPermissions = AssignmentPermissions.Property;

            Assert.AreEqual(123, _parser.Evaluate<int>("Data.Int1 = 123"));

            Assert.AreEqual(123,_parser.Evaluate<int>("Data.Int1"));
        }

        public class XElement
        {
            public string Attribute(XName xName)
            {
                return "attr[" + xName + "]";
            }
        }

        public class XName
        {
            private string _name;

            public static implicit operator XName(string s)
            {
                XName xName = new XName();

                xName._name = s;

                return xName;
            }

            public override string ToString()
            {
                return _name;
            }
        }


        [TestMethod]
        public void CustomImplicitConversions()
        {
            XElement xEl = new XElement();

            _context.Set("xEl", xEl);

            Assert.AreEqual("attr[Test]", _parser.Evaluate<string>("xEl.Attribute(\"Test\")"));
        }

        [TestMethod]
        public void CustomOperators()
        {
            _context.Set("date1", DateTime.Now);
            _context.Set("date2",DateTime.Now.AddHours(1));

            Assert.IsTrue(_parser.Evaluate<bool>("date1 < date2"));
            Assert.IsFalse(_parser.Evaluate<bool>("date1 > date2"));
            Assert.IsFalse(_parser.Evaluate<bool>("date1 == date2"));
            Assert.AreEqual(1,(int)_parser.Evaluate<TimeSpan>("date2 - date1").TotalHours);

        }

        [TestMethod]
        public void ExpressionTree()
        {
            IParserContext context = new ParserContext();

            ExpressionWithContext expr = new ExpressionWithContext(context);

            expr.Expression = Exp.Add(TokenPosition.Unknown, Exp.Value(TokenPosition.Unknown, 4), Exp.Value(TokenPosition.Unknown, 5));

            Assert.AreEqual(9, expr.Evaluate().Value);
            Assert.AreEqual(typeof(int), expr.Evaluate().Type);

            expr.Expression = Exp.Add(TokenPosition.Unknown, Exp.Add(TokenPosition.Unknown, Exp.Value(TokenPosition.Unknown, 4), Exp.Value(TokenPosition.Unknown, (long)5)), Exp.Value(TokenPosition.Unknown, 6));

            Assert.AreEqual(15L, expr.Evaluate().Value);
            Assert.AreEqual(typeof(long), expr.Evaluate().Type);

            expr.Expression = Exp.Op(TokenPosition.Unknown, "<<", Exp.Value(TokenPosition.Unknown, (long)4), Exp.Value(TokenPosition.Unknown, 2));

            Assert.AreEqual(16L, expr.Evaluate().Value);
        }

        [TestMethod]
        public void NumRange()
        {

            Converter<IEnumerable<int>, int> sum = delegate(IEnumerable<int> items)
                                                       {
                                                           int total = 0;
                                                           foreach (int n in items)
                                                               total += n;
                                                           return total;
                                                       };

            //IParserContext context = new ParserContext();

            _context.Set("sum",sum);

            Assert.IsInstanceOfType(_parser.EvaluateToObject("1...3"), typeof(IEnumerable<int>));
            Assert.AreEqual(6,_parser.Evaluate<int>("sum(1 ... 3)"));
        }

        [TestMethod]
        public void DynObject()
        {
            DynamicObject dynObj = new DynamicObject();

            dynObj.Apply(new DataClass(5));

            CSharpContext context = new CSharpContext(dynObj);

            Assert.AreEqual(2, _parser.Evaluate<int>("Method0()",context));
            Assert.AreEqual(21, _parser.Evaluate<int>("Method0() + Method1(5) + Method2(Int1,4)", context));


            
        }

        private static ParserContext SetupFalsyContext(ParserContextBehavior behavior)
        {
            ParserContext context = new ParserContext(behavior);

            context.Set<object>("NullValue", null);
            context.Set<object>("RandomObject",new object());
            context.Set("EmptyString", "");
            context.Set("NonEmptyString", "x");

            return context;
        }

        [TestMethod]
        //[ExpectedException(typeof(NullReferenceException))]
        public void NotFalsyNull()
        {
            try
            {
                ParserContext context = SetupFalsyContext(ParserContextBehavior.Default);

                _parser.Evaluate<bool>("!!NullValue", context);

                Assert.Fail();
            }
            catch(NullReferenceException)
            {
            }
        }

        [TestMethod]
        //[ExpectedException(typeof(ArgumentException))]
        public void NotFalsyEmptyString()
        {
            try
            {
                ParserContext context = SetupFalsyContext(ParserContextBehavior.Default);

                _parser.Evaluate<bool>("!!EmptyString", context);

                Assert.Fail();
            }
            catch(ArgumentException)
            {
            }
        }

        [TestMethod]
        //[ExpectedException(typeof(ArgumentException))]
        public void NotFalsyString()
        {
            try
            {
                ParserContext context = SetupFalsyContext(ParserContextBehavior.Default);

                _parser.Evaluate<bool>("!!NonEmptyString", context);

                Assert.Fail();
            }
            catch(ArgumentException)
            {
                
            }
        }

        [TestMethod]
        public void FalsyEmptyString()
        {
            ParserContext context = SetupFalsyContext(ParserContextBehavior.EmptyStringIsFalse);

            Assert.IsFalse(_parser.Evaluate<bool>("!!EmptyString",context));
        }

        [TestMethod]
        public void FalsyString()
        {
            ParserContext context = SetupFalsyContext(ParserContextBehavior.NonEmptyStringIsTrue);

            Assert.IsTrue(_parser.Evaluate<bool>("!!NonEmptyString", context));
        }

        [TestMethod]
        public void FalsyNull()
        {
            ParserContext context = SetupFalsyContext(ParserContextBehavior.NullIsFalse);

            Assert.IsFalse(_parser.Evaluate<bool>("!!NullValue", context));
        }

        [TestMethod]
        public void FalsyNotNull()
        {
            ParserContext context = SetupFalsyContext(ParserContextBehavior.NotNullIsTrue);

            Assert.IsTrue(_parser.Evaluate<bool>("!!RandomObject", context));
        }

        [TestMethod]
        public void Sequence()
        {
            CSharpContext context = new CSharpContext();

            string output = "";

            context.Set("f", new Action<int>(delegate(int i) { output += i; }));

            
            _parser.Evaluate("f(1);f(2);",context);

            Assert.AreEqual("12",output);
            
            output = "";

            _parser.Evaluate("foreach (x in [1...9]) f(x);", context);

            Assert.AreEqual("123456789", output);

            output = "";
            _parser.Evaluate("foreach (x in [1...9]) { f(x); f(x-1); }", context);

            Assert.AreEqual("102132435465768798", output);
        }

        [TestMethod]
        public void SequenceWithReturn()
        {
            CSharpContext context = new CSharpContext();

            string output = "";

            context.Set("f", new Action<int>(delegate(int i) { output += i; }));

            Assert.AreEqual(5,_parser.Evaluate<int>("f(1);return 5;f(2);", context));

            Assert.AreEqual("1", output);
        }

        [TestMethod]
        public void ForEach()
        {
            CSharpContext context = new CSharpContext();

            string output = "";

            context.Set("f", new Action<int>(delegate(int i) { output += i; }));

            

            _parser.Evaluate("foreach (x in [1...9]) f(x);", context);

            Assert.AreEqual("123456789", output);

            output = "";
            _parser.Evaluate("foreach (x in [1...3]) { f(x); foreach(y in [1...x]) f(y); }", context);

            Assert.AreEqual("112123123", output);
        }

        [TestMethod]
        public void If()
        {
            CSharpContext context = new CSharpContext(ParserContextBehavior.Easy);

            string output = "";

            context.Set("f", new Action<int>(delegate(int i) { output += i; }));

            _parser.Evaluate("if(1==1) f(1);f(2)", context);

            Assert.AreEqual("12", output);

            output = "";

            _parser.Evaluate("if(0) f(1);f(2)", context);

            Assert.AreEqual("2", output);

            output = "";
        }

        [TestMethod]
        public void IfWithReturn()
        {
            CSharpContext context = new CSharpContext(ParserContextBehavior.Easy);

            string output = "";

            context.Set("f", new Action<int>(delegate(int i) { output += i; }));

            Assert.AreEqual(5,_parser.Evaluate<int>("if(1==1) { f(1); return 5; } f(2);", context));

            Assert.AreEqual("1", output);
        }

        [TestMethod]
        public void IfElse()
        {
            CSharpContext context = new CSharpContext(ParserContextBehavior.Easy);

            string output = "";

            context.Set("f", new Action<int>(delegate(int i) { output += i; }));

            _parser.Evaluate("if(1==1) f(1); else f(3); f(2)", context);

            Assert.AreEqual("12", output);

            output = "";

            _parser.Evaluate("if(1==0) f(1); else f(3); f(2)", context);

            Assert.AreEqual("32", output);

            output = "";

            _parser.Evaluate("if(1==0) f(1); else if(1==1) f(3); f(2)", context);

            Assert.AreEqual("32", output);

            output = "";

        }

        [TestMethod]
        public void ComplexScript1()
        {
            CSharpContext context = new CSharpContext(ParserContextBehavior.Easy);

            string output = "";

            context.AssignmentPermissions = AssignmentPermissions.All;

            context.Set("f", new Action<int>(delegate(int i) { output += i; }));
            

            int[] array = new int[50];

            context.Set("array", array);

            for (int i = 0; i < array.Length; i++)
                array[i] = i + 1;

            Random rnd = new Random();

            for (int i=0;i<array.Length-1;i++)
            {
                int idx = rnd.Next(i + 1, array.Length - 1);

                int tmp = array[idx];
                array[idx] = array[i];
                array[i] = tmp;
            }

            string script =
                @"

numSwaps = 0;

foreach (i in [0...array.Length-2])
{
   foreach (j in [i+1...array.Length-1])
   {
        if (array[i] > array[j])
        {
             tmp = array[i];
             array[i] = array[j];
             array[j] = tmp;
 
             numSwaps = numSwaps + 1;
        }
   }
}

return numSwaps;
";

            _parser.Evaluate(script, context);

            object o;
            Type t;

            context.Get("array", out o, out t);

            array = (int[]) o;


            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
            Assert.AreEqual(3, array[2]);
            Assert.AreEqual(4, array[3]);
            Assert.AreEqual(5, array[4]);
            Assert.AreEqual(6, array[5]);
            Assert.AreEqual(7, array[6]);
            Assert.AreEqual(8, array[7]);
            Assert.AreEqual(9, array[8]);
        }

        [TestMethod]
        public void FunctionDefinition()
        {
            CSharpContext context = new CSharpContext();

            string output = "";

            context.Set("f", new Action<int>(delegate(int i) { output += i; }));


            _parser.Evaluate("function x(a,b) { f(a); f(b); } x(1,2);", context);

            Assert.AreEqual("12",output);
        }

        [TestMethod]
        public void FunctionDefinition2()
        {
            CSharpContext context = new CSharpContext();

            string output = "";

            context.Set("f", new Action<int>(delegate(int i) { output += i; }));

            string script = @"
function max(a,b)
{
    return a > b ? a : b;
}

f(max(1,2));
f(max(2,1));
";


            _parser.Evaluate(script, context);

            Assert.AreEqual("22", output);
        }


        [TestMethod]
        public void FunctionDefinition3()
        {
            CSharpContext context = new CSharpContext();

            string output = "";

            context.Set("f", new Action<string>(delegate(string i) { output += i; }));

            string libraryScript = @"
function plural(n,a,b,c,d)
{
    if (n == 1)
        return a;

    if (n == 2 || n == 0)
        return b;

    if (n == 3)
        return c;

    return d;
}
";

            string script = @"
f(plural(0,""a"",""b"",""c"",""d""));
f(plural(1,""a"",""b"",""c"",""d""));
f(plural(2,""a"",""b"",""c"",""d""));
f(plural(3,""a"",""b"",""c"",""d""));
f(plural(4,""a"",""b"",""c"",""d""));
";

            var library = _parser.Parse(libraryScript);

            library.Evaluate(context);

            _parser.Evaluate(script, context);

            Assert.AreEqual("babcd", output);
        }



    }
}