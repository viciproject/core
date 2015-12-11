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
using System.IO;
using System.Reflection;
using System.Text;
using Vici.Core.Parser;

namespace Vici.Core.Json
{
    public class JsonParser
    {
        public static JsonObject Parse(string json)
        {
            return new JsonParser()._Parse(json);
        }

        public static JsonObject Parse(Stream stream)
        {
            return new JsonParser()._Parse(stream);
        }

        public static T Parse<T>(string json) where T : class,new()
        {
            return new JsonParser()._Parse<T>(json);
        }

        private Token[] _tokens;
        private int _currentToken;

        private T _Parse<T>(string json) where T:class, new()
        {
            Tokenize(json);

            _currentToken = 0;

            return ParseObject(typeof(T)).As<T>();
        }

        private JsonObject _Parse(string json)
        {
            Tokenize(json);

            _currentToken = 0;

            return ParseValue();
        }

        private JsonObject _Parse(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return _Parse(reader.ReadToEnd());
            }
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

        private JsonObject ParseObject(Type objectType = null)
        {
            if (CurrentToken().TokenMatcher is NullTokenMatcher)
            {
                NextToken();

                return new JsonObject();
            }

            if (!(CurrentToken().TokenMatcher is ObjectStartTokenMatcher))
                throw new Exception("Expected {");

            object obj;
            bool isDictionary = false;

            if (objectType == null)
            {
                obj = new Dictionary<string, JsonObject>();

                isDictionary = true;
            }
            else
                obj = Activator.CreateInstance(objectType);

            NextToken();

            for (; ; )
            {
                if ((CurrentToken().TokenMatcher is ObjectEndTokenMatcher))
                    break;

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

                        object fieldvalue = ParseValue(fieldType).As(fieldType);

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
                    ((Dictionary<string, JsonObject>) obj)[propName] = ParseValue();
                }

                if (!(CurrentToken().TokenMatcher is CommaTokenMatcher))
                    break;

                NextToken();
            }

            if (!(CurrentToken().TokenMatcher is ObjectEndTokenMatcher))
                throw new Exception("Expected }");

            NextToken();

            return new JsonObject(obj);
        }

        private static bool IsArray(Type type)
        {
            return type != null && (
                type.Inspector().ImplementsOrInherits<IList>() 
                    ||
                   type.Inspector().ImplementsOrInherits(typeof (IList<>)));
        }

        private JsonObject ParseValue(Type type = null)
        {
            bool isArray = false;

            if (type == null)
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
                    isArray = true;
                else if (CurrentToken().TokenMatcher is ObjectStartTokenMatcher ||
                         CurrentToken().TokenMatcher is NullTokenMatcher)
                {}
                else 
                    throw new Exception("Unexpected token " + CurrentToken());
            }

            if (isArray || IsArray(type))
                return ParseArray(type);

            if (type == null)
                return ParseObject();

            if (type == typeof(string))
                return ParseString();

            if (type == typeof(bool))
            {
                bool value = CurrentToken().TokenMatcher is TrueTokenMatcher;

                NextToken();

                return new JsonObject(value);
            }

            if (type == typeof(int) || type == typeof(short) || type == typeof(long) || type == typeof(double) || type == typeof(float) || type == typeof(decimal))
                return ParseNumber(type);
                        
            return ParseObject(type);
        }

        private JsonObject ParseNumber(Type type)
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

            return new JsonObject(Convert.ChangeType(n, type, null));
        }

        

        private JsonObject ParseString()
        {
            if (!(CurrentToken().TokenMatcher is StringTokenMatcher))
                throw new Exception("Expected string");

            string s = CurrentToken().Text.Substring(1,CurrentToken().Text.Length-2);

            //TODO: parse dates

            _currentToken++;

            return new JsonObject(s);
        }

        private JsonObject ParseArray(Type type)
        {
            Type elementType = null;

            if (type != null && type.IsArray)
                elementType = type.GetElementType();
            
            if (!(CurrentToken().TokenMatcher is ArrayStartTokenMatcher))
                throw new Exception("Expected [");

            _currentToken++;

            var list = new List<object>();

            for(;;)
            {
                if (CurrentToken().TokenMatcher is ArrayEndTokenMatcher)
                    break;

                if (elementType == null)
                    list.Add(ParseValue());
                else
                    list.Add(ParseValue().As(elementType));

                if (!(CurrentToken().TokenMatcher is CommaTokenMatcher))
                    break;

                _currentToken++;
            }

            if (!(CurrentToken().TokenMatcher is ArrayEndTokenMatcher))
                throw new Exception("Expected ]");

            _currentToken++;

            Array array = Array.CreateInstance(elementType ?? typeof(JsonObject), list.Count);

            for (int i = 0; i < array.Length; i++ )
                array.SetValue(list[i],i);

            return new JsonObject(array);
        }
    }
}
