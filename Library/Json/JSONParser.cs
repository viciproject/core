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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Vici.Core.Parser;

namespace Vici.Core.Json
{
    public class JsonParser
    {
        private Token[] _tokens;
        private int _currentToken;

        public T Parse<T>(string json) where T:class, new()
        {
            Tokenize(json);

            _currentToken = 0;

            return (T) ParseObject(typeof(T));
        }

        public object Parse(string json)
        {
            Tokenize(json);

            _currentToken = 0;

            return ParseObject(typeof (object));
        }

        private void Tokenize(string json)
        {
            Tokenizer tokenizer = new JSONTokenizer();

            List<Token> tokens = new List<Token>(tokenizer.Tokenize(json, TokenPosition.Unknown));

            tokens.RemoveAll(token => token.TokenMatcher is WhiteSpaceMatcher);

            _tokens = tokens.ToArray();
        }

        private Token CurrentToken()
        {
            return _currentToken < _tokens.Length ? _tokens[_currentToken] : null;
        }

        private void NextToken()
        {
            _currentToken++;
        }

        private object ParseObject(Type objectType)
        {
            if (CurrentToken().TokenMatcher is NullTokenMatcher)
            {
                NextToken();

                return null;
            }

            if (!(CurrentToken().TokenMatcher is ObjectStartTokenMatcher))
                throw new Exception("Expected {");

            object obj;
            bool isDictionary = false;

            if (objectType == typeof(object))
            {
                obj = new Dictionary<string, object>();

                isDictionary = true;
            }
            else
                obj = Activator.CreateInstance(objectType);

            NextToken();

            for (; ; )
            {
                if (!(CurrentToken().TokenMatcher is StringTokenMatcher))
                    throw new Exception("Expected property name");

                string propName = CurrentToken().Text.Substring(1, CurrentToken().Text.Length - 2);

                NextToken();

                if (!(CurrentToken().TokenMatcher is ColonTokenMatcher))
                    throw new Exception("Expected colon");

                NextToken();

                if (!isDictionary)
                {
                    PropertyInfo property = objectType.Inspector().GetProperty(propName);
                    FieldInfo field = objectType.Inspector().GetField(propName);

                    if (property != null || field != null)
                    {
                        Type fieldType = (property != null) ? property.PropertyType : field.FieldType;

                        object fieldvalue = ParseValue(fieldType);

                        if (property != null)
                            property.SetValue(obj, fieldvalue, null);
                        else
                            field.SetValue(obj, fieldvalue);
                    }
                    else
                    {
                        ParseValue(typeof (object));
                    }
                }
                else
                {
                    ((Dictionary<string, object>) obj)[propName] = ParseValue(typeof (object));
                }

                if (!(CurrentToken().TokenMatcher is CommaTokenMatcher))
                    break;

                NextToken();
            }

            if (!(CurrentToken().TokenMatcher is ObjectEndTokenMatcher))
                throw new Exception("Expected }");

            NextToken();

            return obj;
        }

        private static bool IsArray(Type type)
        {
            return type.Inspector().ImplementsOrInherits<IList>() 
                    ||
                   type.Inspector().ImplementsOrInherits(typeof (IList<>));
        }

        private object ParseValue(Type type)
        {
            if (type == typeof(object))
            {
                if (CurrentToken().TokenMatcher is StringTokenMatcher)
                    type = typeof(string);
                else if (CurrentToken().TokenMatcher is IntegerTokenMatcher)
                    type = typeof(Int64);
                else if (CurrentToken().TokenMatcher is FloatTokenMatcher)
                    type = typeof(double);
                else if (CurrentToken().TokenMatcher is TrueTokenMatcher || CurrentToken().TokenMatcher is FalseTokenMatcher)
                    type = typeof(bool);
                else if (CurrentToken().TokenMatcher is ArrayStartTokenMatcher)
                    type = typeof(object[]);
                else if (CurrentToken().TokenMatcher is ObjectStartTokenMatcher || CurrentToken().TokenMatcher is NullTokenMatcher)
                    type = typeof (object);
                else 
                    throw new Exception("Unexpected token " + CurrentToken());
            }

            if (type == typeof(string))
                return ParseString();

            if (type == typeof(bool))
            {
                bool value = CurrentToken().TokenMatcher is TrueTokenMatcher;

                NextToken();

                return value;
            }

            if (type == typeof(int) || type == typeof(short) || type == typeof(long) || type == typeof(double) || type == typeof(float) || type == typeof(decimal))
                return ParseNumber(type);
            
            if (IsArray(type))
                return ParseArray(type);
            
            return ParseObject(type);
        }

        private object ParseNumber(Type type)
        {
            if (!(CurrentToken().TokenMatcher is IntegerTokenMatcher) && !(CurrentToken().TokenMatcher is FloatTokenMatcher))
                throw new Exception("Number expected");

            object n;

            if (CurrentToken().TokenMatcher is IntegerTokenMatcher)
            {
                n = Int64.Parse(CurrentToken().Text, NumberFormatInfo.InvariantInfo);
            }
            else
            {
                n = Double.Parse(CurrentToken().Text, NumberFormatInfo.InvariantInfo);
            }

            _currentToken++;

            return Convert.ChangeType(n, type, null);
        }

        

        private object ParseString()
        {
            if (!(CurrentToken().TokenMatcher is StringTokenMatcher))
                throw new Exception("Expected string");

            string s = CurrentToken().Text.Substring(1,CurrentToken().Text.Length-2);

            //TODO: parse dates

            _currentToken++;

            return s;
        }

        private object ParseArray(Type type)
        {
            Type elementType = null;

            if (type.IsArray)
            {
                elementType = type.GetElementType();
            }
            
            if (!(CurrentToken().TokenMatcher is ArrayStartTokenMatcher))
                throw new Exception("Expected [");

            _currentToken++;

            List<object> list = new List<object>();

            for(;;)
            {
                if (CurrentToken().TokenMatcher is ArrayEndTokenMatcher)
                    break;

                list.Add(ParseValue(elementType));

                if (!(CurrentToken().TokenMatcher is CommaTokenMatcher))
                    break;

                _currentToken++;
            }

            if (!(CurrentToken().TokenMatcher is ArrayEndTokenMatcher))
                throw new Exception("Expected ]");

            _currentToken++;

            Array array = Array.CreateInstance(elementType, list.Count);

            for (int i = 0; i < array.Length; i++ )
                array.SetValue(list[i],i);

            return array;
        }
    }
}
