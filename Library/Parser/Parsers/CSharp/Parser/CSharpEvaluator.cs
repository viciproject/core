#region License
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
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Vici.Core.Parser
{
    public static class CSharpEvaluator
    {
        private static readonly NumberFormatInfo _numberFormat;

        static CSharpEvaluator()
        {
            _numberFormat = new NumberFormatInfo
                                {
                                    NumberDecimalSeparator = ".",
                                    NumberGroupSeparator = ",",
                                    NumberGroupSizes = new[] {3}
                                };
        }

        public static Expression TypeCast(string token, TokenPosition position, Expression[] terms)
        {
            return new TypeCastExpression(position, new VariableExpression(position, token.Substring(1, token.Length - 2).Trim()), terms[0]);
        }

        public static Expression IsAsOperator(string token, TokenPosition position, Expression[] terms)
        {
            if (token == "as")
                return new AsExpression(position, terms[0], terms[1]);

            if (token == "is")
                return new IsExpression(position, terms[0], terms[1]);

            return null;
        }

        public static Expression Ternary(string token, TokenPosition position, Expression[] terms)
        {
            return new ConditionalExpression(position, terms[0], terms[1], terms[2]);
        }

        private static char UnEscape(string s, TokenPosition position)
        {
            if (s.Length == 1)
                return s[0];

            if (s.Length == 2)
            {
                switch (s[1])
                {
                    case '\\':
                    case '\"':
                    case '\'':
                        return s[1];
                    case '0':
                        return (char)0;
                    case 'a':
                        return '\a';
                    case 'b':
                        return '\b';
                    case 'f':
                        return '\f';
                    case 'n':
                        return '\n';
                    case 'r':
                        return '\r';
                    case 't':
                        return '\t';
                    case 'v':
                        return '\v';
                    default:
                        throw new UnknownTokenException(position,s);
                }
            }
            else
            {
                return (char)Convert.ToUInt16(s.Substring(2), 16);
            }
        }

        public static Expression TypeOf(string token, TokenPosition position, Expression[] terms)
        {
            return new TypeOfExpression(position);
        }

        public static Expression CharLiteral(string token, TokenPosition position, Expression[] terms)
        {
            return Exp.Value(position, UnEscape(token.Substring(1, token.Length - 2), position));
        }

        public static Expression Number(string token, TokenPosition position, Expression[] terms)
        {
            string s = token;

            Type type = null;

            if (!char.IsDigit(s[s.Length - 1]))
            {
                string suffix = "" + char.ToUpper(s[s.Length - 1]);

                s = s.Remove(s.Length - 1);

                if (!char.IsDigit(s[s.Length - 1]))
                {
                    suffix = char.ToUpper(s[s.Length - 1]) + suffix;

                    s = s.Remove(s.Length - 1);
                }

                switch (suffix)
                {
                    case "M":
                        type = typeof(decimal);
                        break;
                    case "D":
                        type = typeof(double);
                        break;
                    case "F":
                        type = typeof(float);
                        break;
                    case "L":
                        type = typeof(long);
                        break;
                    case "U":
                        type = typeof(uint);
                        break;
                    case "LU":
                    case "UL":
                        type = typeof(ulong);
                        break;
                }
            }

            if (type != null)
                return new ValueExpression(position, Convert.ChangeType(s, type, _numberFormat), type);

            if (s.LastIndexOf('.') >= 0)
            {
                return Exp.Value(position, Convert.ToDouble(s, _numberFormat));
            }
            else
            {
                long n = Convert.ToInt64(s);

                if (n > Int32.MaxValue || n < Int32.MinValue)
                    return Exp.Value(position, n);
                else
                    return Exp.Value(position, (int)n);
            }
        }

        public static Expression VarName(string token, TokenPosition position, Expression[] terms)
        {
            return new VariableExpression(position, token);
        }

        public static Expression Function(string token, TokenPosition position, Expression[] terms)
        {
            Expression[] parameters = new Expression[terms.Length - 1];

            Array.Copy(terms, 1, parameters, 0, parameters.Length);

            if (token == "[")
            {
                return new IndexExpression(position, terms[0], parameters);
            }
            else
            {
                return new CallExpression(position, terms[0], parameters);
            }
        }

        public static Expression Coalesce(string token, TokenPosition position, Expression[] terms)
        {
            return new CoalesceExpression(position, terms[0], terms[1]);
        }

        public static Expression DefaultValueOperator(string token, TokenPosition position, Expression[] terms)
        {
            return new DefaultValueExpression(position, terms[0], terms[1]);
        }

        public static Expression ShortcutOperator(string token, TokenPosition position, Expression[] terms)
        {
            if (token == "&&")
                return new AndAlsoExpression(position, terms[0], terms[1]);

            if (token == "||")
                return new OrElseExpression(position, terms[0], terms[1]);

            return null;
        }

        public static Expression Unary(string token, TokenPosition position, Expression[] terms)
        {
            if (token == "!")
                return new NegationExpression(position, terms[0]);

            if (token == "-")
                return new UnaryMinusExpression(position, terms[0]);

            if (token == "~")
                return new BitwiseComplementExpression(position, terms[0]);

            return null;
        }

        public static Expression Operator(string token, TokenPosition position, Expression[] terms)
        {
            return Exp.Op(position, token, terms[0], terms[1]);
        }

        public static Expression Assignment(string token, TokenPosition position, Expression[] terms)
        {
            return new AssignmentExpression(position, terms[0], terms[1]);
        }

        public static Expression StringLiteral(string token, TokenPosition position, Expression[] terms)
        {
            string s = token.Substring(1, token.Length - 2);

            if (s.IndexOf('\\') < 0)
                return Exp.Value(position, s);

            string output = "";

            bool inEscape = false;
            string hexString = null;

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                if (inEscape)
                {
                    if (c == 'x')
                    {
                        hexString = "";
                        continue;
                    }

                    if (hexString == null && (c != 'x' || c != 'X'))
                    {
                        output += UnEscape("\\" + c, position);
                        inEscape = false;
                        continue;
                    }

                    if (hexString == null)
                    {
                        inEscape = false;
                    }
                    else
                    {
                        if (((char.ToLower(c) < 'a' || char.ToLower(c) > 'f') && (c < '0' || c > '9')) || hexString.Length == 4)
                        {
                            output += UnEscape("\\x" + hexString, position);
                            inEscape = false;
                            hexString = null;
                        }
                        else
                        {
                            hexString += c;
                            continue;
                        }
                    }
                }

                if (c != '\\')
                {
                    output += c;

                    continue;
                }

                inEscape = true;
            }

            return Exp.Value(position, output);
        }

        public static Expression DotOperator(string token, TokenPosition position, Expression[] terms)
        {
            VariableExpression varExpression = terms[1] as VariableExpression;

            if (varExpression == null)
                throw new UnknownPropertyException("Unkown member " + terms[1], terms[1]);

            return new FieldExpression(position, terms[0], varExpression.VarName);
        }

        public static Expression Constructor(string token, TokenPosition position, Expression[] terms)
        {
            string className = token.Substring(3).Trim();

            return new ConstructorExpression(position, new VariableExpression(position, className), terms);
        }

        public static Expression NumericRange(string token, TokenPosition position, Expression[] terms)
        {
            return new RangeExpression(position, terms[0], terms[1]);
        }
    }
}