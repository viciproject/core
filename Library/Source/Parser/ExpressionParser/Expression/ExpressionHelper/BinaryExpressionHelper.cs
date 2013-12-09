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

namespace Vici.Core.Parser
{
    internal class BinaryExpressionHelper
    {
        public static object CalcUInt32_Int32(string op, object p1, object p2, StringComparison stringComparison, Expression expr)
        {
            uint? v1 = (uint?)p1;
            int? v2 = (int?)p2;

            switch (op)
            {
                case ">>": return v1 >> v2;
                case "<<": return v1 << v2;
            }

            throw new IllegalOperandsException("Operator " + op + " not supported for UInt32 and Int32", expr);
        }

        public static object CalcInt64_Int32(string op, object p1, object p2, StringComparison stringComparison, Expression expr)
        {
            long? v1 = (long?)p1;
            int? v2 = (int?)p2;

            switch (op)
            {
                case ">>": return v1 >> v2;
                case "<<": return v1 << v2;
            }

            throw new ArithmeticException();
        }

        public static object CalcUInt64_Int32(string op, object p1, object p2, StringComparison stringComparison, Expression expr)
        {
            ulong? v1 = (ulong?)p1;
            int? v2 = (int?)p2;

            switch (op)
            {
                case ">>": return v1 >> v2;
                case "<<": return v1 << v2;
            }

            throw new ArithmeticException();
        }

        public static object CalcBool(string op, object p1, object p2, StringComparison stringComparison, Expression expr)
        {
            bool? v1 = (bool?)p1;
            bool? v2 = (bool?)p2;

            switch (op)
            {
                case "^": return v1 ^ v2;
                case "&": return v1 & v2;
                case "|": return v1 | v2;
                case "==": return v1 == v2;
                case "!=": return v1 != v2;
            }

            throw new ArithmeticException();
        }

        public static object CalcInt32(string op, object p1, object p2, StringComparison stringComparison, Expression expr)
        {
            int? v1 = (int?) p1;
            int? v2 = (int?) p2;

            switch (op)
            {
                case "+": return v1 + v2;
                case "-": return v1 - v2;
                case "/": return v1 / v2;
                case "*": return v1 * v2;
                case "%": return v1 % v2;
                case "^": return v1 ^ v2;
                case "&": return v1 & v2;
                case "|": return v1 | v2;
                case ">>": return v1 >> v2;
                case "<<": return v1 << v2;
                case "==": return v1 == v2;
                case "!=": return v1 != v2;
                case "<=": return v1 <= v2;
                case ">=": return v1 >= v2;
                case "<": return v1 < v2;
                case ">": return v1 > v2;
            }

            throw new ArithmeticException();
        }

        public static object CalcUInt32(string op, object p1, object p2, StringComparison stringComparison, Expression expr)
        {
            uint? v1 = (uint?)p1;
            uint? v2 = (uint?)p2;

            switch (op)
            {
                case "+": return v1 + v2;
                case "-": return v1 - v2;
                case "/": return v1 / v2;
                case "*": return v1 * v2;
                case "%": return v1 % v2;
                case "^": return v1 ^ v2;
                case "&": return v1 & v2;
                case "|": return v1 | v2;
                case "==": return v1 == v2;
                case "!=": return v1 != v2;
                case "<=": return v1 <= v2;
                case ">=": return v1 >= v2;
                case "<": return v1 < v2;
                case ">": return v1 > v2;
            }

            throw new ArithmeticException();
        }

        public static object CalcInt64(string op, object p1, object p2, StringComparison stringComparison, Expression expr)
        {
            long? v1 = (long?)p1;
            long? v2 = (long?)p2;

            switch (op)
            {
                case "+": return v1 + v2;
                case "-": return v1 - v2;
                case "/": return v1 / v2;
                case "*": return v1 * v2;
                case "%": return v1 % v2;
                case "^": return v1 ^ v2;
                case "&": return v1 & v2;
                case "|": return v1 | v2;
                case "==": return v1 == v2;
                case "!=": return v1 != v2;
                case "<=": return v1 <= v2;
                case ">=": return v1 >= v2;
                case "<": return v1 < v2;
                case ">": return v1 > v2;
            }

            throw new ArithmeticException();
        }

        public static object CalcUInt64(string op, object p1, object p2, StringComparison stringComparison, Expression expr)
        {
            ulong? v1 = (ulong?)p1;
            ulong? v2 = (ulong?)p2;

            switch (op)
            {
                case "+": return v1 + v2;
                case "-": return v1 - v2;
                case "/": return v1 / v2;
                case "*": return v1 * v2;
                case "%": return v1 % v2;
                case "^": return v1 ^ v2;
                case "&": return v1 & v2;
                case "|": return v1 | v2;
                case "==": return v1 == v2;
                case "!=": return v1 != v2;
                case "<=": return v1 <= v2;
                case ">=": return v1 >= v2;
                case "<": return v1 < v2;
                case ">": return v1 > v2;
            }

            throw new ArithmeticException();
        }

        public static object CalcFloat(string op, object p1, object p2, StringComparison stringComparison, Expression expr)
        {
            float? v1 = (float?)p1;
            float? v2 = (float?)p2;

            switch (op)
            {
                case "+": return v1 + v2;
                case "-": return v1 - v2;
                case "/": return v1 / v2;
                case "*": return v1 * v2;
                case "%": return v1 % v2;
                case "==": return v1 == v2;
                case "!=": return v1 != v2;
                case "<=": return v1 <= v2;
                case ">=": return v1 >= v2;
                case "<": return v1 < v2;
                case ">": return v1 > v2;
            }

            throw new ArithmeticException();
        }

        public static object CalcDouble(string op, object p1, object p2, StringComparison stringComparison, Expression expr)
        {
            double? v1 = (double?)p1;
            double? v2 = (double?)p2;

            switch (op)
            {
                case "+": return v1 + v2;
                case "-": return v1 - v2;
                case "/": return v1 / v2;
                case "*": return v1 * v2;
                case "%": return v1 % v2;
                case "==": return v1 == v2;
                case "!=": return v1 != v2;
                case "<=": return v1 <= v2;
                case ">=": return v1 >= v2;
                case "<": return v1 < v2;
                case ">": return v1 > v2;
            }

            throw new ArithmeticException();
        }

        public static object CalcDecimal(string op, object p1, object p2, StringComparison stringComparison, Expression expr)
        {
            decimal? v1 = (decimal?)p1;
            decimal? v2 = (decimal?)p2;

            switch (op)
            {
                case "+": return v1 + v2;
                case "-": return v1 - v2;
                case "/": return v1 / v2;
                case "*": return v1 * v2;
                case "%": return v1 % v2;
                case "==": return v1 == v2;
                case "!=": return v1 != v2;
                case "<=": return v1 <= v2;
                case ">=": return v1 >= v2;
                case "<": return v1 < v2;
                case ">": return v1 > v2;
            }

            throw new ArithmeticException();
        }

        public static object CalcString(string op, object p1, object p2, StringComparison stringComparison, Expression expr)
        {
            string v1 = (string)p1;
            string v2 = (string)p2;

            if (op == "+")
                return (v1 + v2);

            if (op != "==" && op != "!=")
                throw new ArithmeticException("Illegal operator [" + op + "] for strings");

            bool equals = (op == "==");

            if (v1 == null && v2 == null)
                return equals;

            if (v1 == null || v2 == null)
                return !equals;

            return v1.Equals(v2, stringComparison) ? equals : !equals;
        }

        public static object CalcStringObject(string op, object p1, object p2, StringComparison stringComparison, Expression expr)
        {
            string v1 = (string)p1;
            object v2 = p2;

            switch (op)
            {
                case "+": return v1 + v2;
            }

            throw new ArithmeticException();
        }

        public static object CalcObjectString(string op, object p1, object p2, StringComparison stringComparison, Expression expr)
        {
            object v1 = p1;
            string v2 = (string) p2;

            switch (op)
            {
                case "+": return v1 + v2;
            }

            throw new ArithmeticException();
        }

        public static object CalcObject(string op, object p1, object p2, StringComparison stringComparison, Expression expr)
        {
            if (op != "==" && op != "!=")
                throw new ArithmeticException();

            bool isEqual;

            if (p1 == null || p2 == null)
                isEqual = object.ReferenceEquals(p1,p2);
            else
                isEqual = p1.Equals(p2);

            return isEqual == (op == "==");
        }


    }
}