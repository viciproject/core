//=============================================================================
// Vici Core - Productivity Library for .NET 3.5 
//
// Copyright (c) 2008-2011 Philippe Leybaert
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
using Vici.Core.Parser;
using NUnit.Framework;

namespace Vici.Core.Test
{
    [TestFixture]
    public class CSharpParserFixture
    {
        private readonly ParserContext _context = new CSharpContext();
        private readonly CSharpParser _parser = new CSharpParser();

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

        [SetUp]
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

            _parser.DefaultContext = _context;
        }

        private static int Func(int i)
        {
            return i * 5;
        }

        [Test]
        public void ComplexExpressions()
        {
            Assert.AreEqual(435, _parser.Evaluate<int>("Math.Max(Data.Method2(Data.Int1+10,300),Data.Method1(Data.Int1))+(\"x\" + 5).Length"));

            Assert.AreEqual(17, _parser.Evaluate<int>("Data.Method2(Data.Method2(3,4),Data.Method1(5))"));
            Assert.AreEqual(100, _parser.Evaluate<int>("Max(Max(100,5),Func(10))"));
            Assert.AreEqual(1000, _parser.Evaluate<int>("Max(Max(100,5),Func(200))"));
        }

        [Test]
        public void DefaultValueExpression()
        {
            IParserContext context = new FlexContext();

            context.Set("a","");
            context.Set("b", "z");


            Assert.AreEqual("x", _parser.Evaluate<string>("a ?: \"x\"",context));
            Assert.AreEqual("z", _parser.Evaluate<string>("b ?: \"x\"", context));
        }

        [Test]
        public void StringExpressions()
        {
            Assert.AreEqual("ab", _parser.Evaluate<string>("string.Concat(\"a\",\"b\")"));
            Assert.AreEqual("ab", _parser.Evaluate<string>("\"a\" + \"b\")"));
        }

        [Test]
        public void StringComparisons()
        {
            StringComparison saved = _context.StringComparison;

            _context.StringComparison = StringComparison.InvariantCulture;

            Assert.IsTrue(_parser.Evaluate<bool>("\"a\" == \"a\""));
            Assert.IsFalse(_parser.Evaluate<bool>("\"a\" == \"b\""));
            Assert.IsTrue(_parser.Evaluate<bool>("\"a\" != \"b\""));
            Assert.IsFalse(_parser.Evaluate<bool>("\"a\" != \"a\""));

            Assert.IsFalse(_parser.Evaluate<bool>("\"a\" == \"A\""));
            Assert.IsFalse(_parser.Evaluate<bool>("\"a\" == \"B\""));
            Assert.IsTrue(_parser.Evaluate<bool>("\"a\" != \"B\""));
            Assert.IsTrue(_parser.Evaluate<bool>("\"a\" != \"B\""));

            _context.StringComparison = StringComparison.InvariantCultureIgnoreCase;

            Assert.IsTrue(_parser.Evaluate<bool>("\"a\" == \"A\""));
            Assert.IsFalse(_parser.Evaluate<bool>("\"a\" != \"A\""));

            _context.StringComparison = saved;
        }


        [Test]
        public void MemberMethods()
        {
            Assert.AreEqual(2, _parser.Evaluate<int>("Data.Method0()"));
            Assert.AreEqual(2, _parser.Evaluate<int>("Math.Max(1,2)"));
            Assert.AreEqual(21, _parser.Evaluate<int>("Data.Method0() + Data.Method1(5) + Data.Method2(5,4)"));
        }

        [Test]
        public void CharLiterals()
        {
            Assert.AreEqual('x', _parser.Evaluate<char>("'x'"));
            Assert.AreEqual('\n', _parser.Evaluate<char>("'\\n'"));
            Assert.AreEqual("Test\n", _parser.Evaluate<string>("\"Test\" + '\\n'"));
            Assert.AreEqual('\'', _parser.Evaluate<char>("'\\''"));
            Assert.AreEqual('\x45', _parser.Evaluate<char>("'\\x45'"));
            Assert.AreEqual('\x4545', _parser.Evaluate<char>("'\\x4545'"));
        }

        [Test]
        public void TypeCast()
        {
            CSharpTokenizer tokenizer = new CSharpTokenizer();

            Assert.IsInstanceOfType(typeof(int), _parser.EvaluateToObject("(int)5L"));
        }

        [Test]
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

        [Test]
        public void NumericLiterals()
        {
            Assert.IsInstanceOfType(typeof(int), _parser.EvaluateToObject("1"));
            Assert.AreEqual(1, _parser.Evaluate<int>("1"));

            Assert.IsInstanceOfType(typeof(long), _parser.EvaluateToObject("10000000000"));
            Assert.AreEqual(10000000000, _parser.Evaluate<long>("10000000000"));

            Assert.IsInstanceOfType(typeof(decimal), _parser.EvaluateToObject("1m"));
            Assert.AreEqual(1m, _parser.Evaluate<decimal>("1m"));

            Assert.IsInstanceOfType(typeof(long), _parser.EvaluateToObject("1L"));
            Assert.AreEqual(1L, _parser.Evaluate<long>("1L"));

            Assert.IsInstanceOfType(typeof(ulong), _parser.EvaluateToObject("1UL"));
            Assert.AreEqual(1UL, _parser.Evaluate<ulong>("1UL"));

            Assert.IsInstanceOfType(typeof(double), _parser.EvaluateToObject("1.0"));
            Assert.AreEqual(1L, _parser.Evaluate<double>("1.0"));

            Assert.IsInstanceOfType(typeof(float), _parser.EvaluateToObject("1.0f"));
            Assert.AreEqual(1L, _parser.Evaluate<float>("1.0f"));

        }

        [Test]
        public void ObjectCreation()
        {
            Assert.IsInstanceOfType(typeof(DataClass), _parser.Evaluate<object>("new DataClass(5)"));

            Assert.AreEqual(5, _parser.Evaluate<int>("(new DataClass(5)).Int1"));
            Assert.AreEqual(5, _parser.Evaluate<int>("new DataClass(5).Int1"));
            Assert.AreEqual(5, _parser.Evaluate<int>("Math.Max(new DataClass(3+2).Int1,3)"));
        }

        [Test]
        public void Delegates()
        {
            Assert.AreEqual(10, _parser.Evaluate<int>("Func(2)"));
            Assert.AreEqual(5, _parser.Evaluate<int>("Max(4,5)"));
        }

        [Test]
        public void Typeof()
        {
            Assert.AreEqual(typeof(int), _parser.Evaluate<Type>("typeof(int)"));
        }

        [Test]
        public void OperatorPrecedence()
        {
            Assert.AreEqual(2, _parser.Evaluate<int>("(5-4)*2"));
            Assert.AreEqual(13, _parser.Evaluate<int>("5+4*2"));
            Assert.AreEqual(22, _parser.Evaluate<int>("5*4+2"));
            Assert.AreEqual(18, _parser.Evaluate<int>("(5+4)*2"));
        }

        [Test]
        public void UnaryNot()
        {
            Assert.AreEqual(false, _parser.Evaluate<bool>("!(1==1)"));
            Assert.AreEqual(true, _parser.Evaluate<bool>("!false"));
            Assert.AreEqual(true, _parser.Evaluate<bool>("!!true"));
        }

        [Test]
        public void UnaryMinus()
        {
            Assert.AreEqual(-2, _parser.Evaluate<int>("-2"));
            Assert.AreEqual(3, _parser.Evaluate<int>("5+-2"));
            Assert.AreEqual(-1, _parser.Evaluate<int>("-(3-2)"));
        }

        [Test]
        public void BitwiseComplement()
        {
            Assert.AreEqual(~2, _parser.Evaluate<int>("~2"));
            Assert.AreEqual(5 + ~2, _parser.Evaluate<int>("5+~2"));
            Assert.AreEqual(~(3 - 2), _parser.Evaluate<int>("~(3 - 2)"));
        }

        [Test]
        public void StaticFields()
        {
            Assert.AreEqual(500, _parser.Evaluate<int>("DataClass.Static1"));
            Assert.AreEqual(501, _parser.Evaluate<int>("DataClass.Static2"));
        }

        [Test]
        public void NullableLifting()
        {
            Assert.AreEqual(15, _parser.Evaluate<int?>("Value10 + NullableValue5"));
            Assert.IsInstanceOfType(typeof(int?), _parser.Evaluate<int?>("Value10 + NullableValue5"));
            Assert.AreEqual(null, _parser.Evaluate<int?>("Value10 + NullableValueNull"));

        }

        [Test]
        public void Indexing()
        {
            Assert.AreEqual(30, _parser.Evaluate<int>("Data[Func(5),5]"));
        }

        [Test]
        public void ArrayIndexing()
        {
            Assert.AreEqual(8, _parser.Evaluate<int>("MyArray[3]"));
            Assert.AreEqual(8, _parser.Evaluate<int>("MyArray2[2,1]"));

            Assert.AreEqual(16, _parser.Evaluate<int>("MyArray[Data.Method0()+2]"));
            Assert.AreEqual(8, _parser.Evaluate<int>("MyArray2[Data.Method0()+1,0]"));

        }

        [Test]
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

        [Test]
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

        [Test]
        public void AsOperator()
        {
            Assert.AreEqual("x", _parser.Evaluate<string>("\"x\" as string"));
            Assert.AreEqual(null, _parser.Evaluate<string>("5 as string"));
        }

        [Test]
        public void IsOperator()
        {
            Assert.IsTrue(_parser.Evaluate<bool>("\"x\" is string"));
            Assert.IsFalse(_parser.Evaluate<bool>("5 is string"));
            Assert.IsFalse(_parser.Evaluate<bool>("null is string"));
        }

        [Test]
        public void NullValueOperator()
        {
            Assert.AreEqual(10, _parser.Evaluate<int>("NullableValueNull ?? 10"));
            Assert.AreEqual(5, _parser.Evaluate<int>("NullableValue5 ?? 10"));
        }

        [Test]
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

        [Test]
        [ExpectedException(typeof(IllegalAssignmentException))]
        public void PropertyAssignmentNotAllowed()
        {
            _context.AssignmentPermissions = AssignmentPermissions.None;

            Assert.AreEqual(123, _parser.Evaluate<int>("Data.Int1 = 123"));
        }


        [Test]
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


        [Test]
        public void CustomImplicitConversions()
        {
            XElement xEl = new XElement();

            _context.Set("xEl", xEl);

            Assert.AreEqual("attr[Test]", _parser.Evaluate<string>("xEl.Attribute(\"Test\")"));
        }

        [Test]
        public void CustomOperators()
        {
            _context.Set("date1", DateTime.Now);
            _context.Set("date2",DateTime.Now.AddHours(1));

            Assert.IsTrue(_parser.Evaluate<bool>("date1 < date2"));
            Assert.IsFalse(_parser.Evaluate<bool>("date1 > date2"));
            Assert.IsFalse(_parser.Evaluate<bool>("date1 == date2"));
            Assert.AreEqual(1,(int)_parser.Evaluate<TimeSpan>("date2 - date1").TotalHours);

        }

        [Test]
        public void ExpressionTree()
        {
            IParserContext context = new ParserContext();

            ExpressionWithContext expr = new ExpressionWithContext(context);

            expr.Expression = Exp.Add(TokenPosition.Unknown, Exp.Value(TokenPosition.Unknown, 4), Exp.Value(TokenPosition.Unknown, 5));

            Assert.AreEqual(9, expr.Evaluate().Value);
            Assert.AreEqual(typeof(int), expr.Evaluate().Type);

            expr.Expression = Exp.Add(TokenPosition.Unknown, Exp.Add(TokenPosition.Unknown, Exp.Value(TokenPosition.Unknown, 4), Exp.Value(TokenPosition.Unknown, (long)5)), Exp.Value(TokenPosition.Unknown, 6));

            Assert.AreEqual(15, expr.Evaluate().Value);
            Assert.AreEqual(typeof(long), expr.Evaluate().Type);

            expr.Expression = Exp.Op(TokenPosition.Unknown, "<<", Exp.Value(TokenPosition.Unknown, (long)4), Exp.Value(TokenPosition.Unknown, 2));

            Assert.AreEqual(16, expr.Evaluate().Value);
        }

        [Test]
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
            
            Assert.IsInstanceOfType(typeof(IEnumerable<int>),_parser.EvaluateToObject("1...3"));
            Assert.AreEqual(6,_parser.Evaluate<int>("sum(1 ... 3)"));
        }

        [Test]
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

        [Test]
        [ExpectedException(typeof(NullReferenceException))]
        public void NotFalsyNull()
        {
            ParserContext context = SetupFalsyContext(ParserContextBehavior.Default);

            _parser.Evaluate<bool>("!!NullValue", context);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void NotFalsyEmptyString()
        {
            ParserContext context = SetupFalsyContext(ParserContextBehavior.Default);

            _parser.Evaluate<bool>("!!EmptyString", context);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void NotFalsyString()
        {
            ParserContext context = SetupFalsyContext(ParserContextBehavior.Default);

            _parser.Evaluate<bool>("!!NonEmptyString", context);
        }

        [Test]
        public void FalsyEmptyString()
        {
            ParserContext context = SetupFalsyContext(ParserContextBehavior.EmptyStringIsFalse);

            Assert.IsFalse(_parser.Evaluate<bool>("!!EmptyString",context));
        }

        [Test]
        public void FalsyString()
        {
            ParserContext context = SetupFalsyContext(ParserContextBehavior.NonEmptyStringIsTrue);

            Assert.IsTrue(_parser.Evaluate<bool>("!!NonEmptyString", context));
        }

        [Test]
        public void FalsyNull()
        {
            ParserContext context = SetupFalsyContext(ParserContextBehavior.NullIsFalse);

            Assert.IsFalse(_parser.Evaluate<bool>("!!NullValue", context));
        }

        [Test]
        public void FalsyNotNull()
        {
            ParserContext context = SetupFalsyContext(ParserContextBehavior.NotNullIsTrue);

            Assert.IsTrue(_parser.Evaluate<bool>("!!RandomObject", context));
        }

    }
}