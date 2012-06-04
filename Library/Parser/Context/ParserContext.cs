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

namespace Vici.Core.Parser
{
    public class ParserContext : IParserContext
    {
        private readonly Dictionary<string, object> _variables;
        private readonly Dictionary<string, Type> _types;
        private readonly object _rootObject;

        private readonly IParserContext _parentContext;

        public ParserContextBehavior Behavior { get; private set; }

        private AssignmentPermissions _assignmentPermissions = AssignmentPermissions.None;
        private StringComparison _stringComparison = StringComparison.Ordinal;
        private IFormatProvider _formatProvider = NumberFormatInfo.InvariantInfo;
        
        public ParserContext(ParserContextBehavior behavior)
        {
            Behavior = behavior;

            if ((behavior & ParserContextBehavior.CaseInsensitiveVariables) == ParserContextBehavior.CaseInsensitiveVariables)
            {
                _variables = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                _types = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                _variables = new Dictionary<string, object>();
                _types = new Dictionary<string, Type>();

            }
        }

        public ParserContext() : this(ParserContextBehavior.Default)
        {
        }

        public ParserContext(object rootObject, ParserContextBehavior behavior) : this(behavior)
        {
            _rootObject = rootObject;
        }

        public ParserContext(object rootObject) : this(rootObject, ParserContextBehavior.Default)
        {
        }

        protected ParserContext(ParserContext parentContext) : this(parentContext.Behavior)
        {
            _parentContext = parentContext;

            _assignmentPermissions = parentContext._assignmentPermissions;
            _stringComparison = parentContext._stringComparison;
            _formatProvider = parentContext._formatProvider;
        }

        public virtual IParserContext CreateLocal()
        {
            return new ParserContext(this);
        }

        private bool TestBehavior(ParserContextBehavior behavior)
        {
            return ((Behavior & behavior) == behavior);
        }

//        public bool ReturnNullWhenNullReference
//        {
//            get { return _returnNullWhenNullReference; }
//            set { _returnNullWhenNullReference = value; }
//        }
//
//        public bool NullIsFalse
//        {
//            get { return _nullIsFalse; }
//            set { _nullIsFalse = value; }
//        }
//
//        public bool NotNullIsTrue
//        {
//            get { return _notNullIsTrue; }
//            set { _notNullIsTrue = value; }
//        }
//
//        public bool EmptyStringIsFalse
//        {
//            get { return _emptyStringIsFalse; }
//            set { _emptyStringIsFalse = value; }
//        }
//
//        public bool NonEmptyStringIsTrue
//        {
//            get { return _nonEmptyStringIsTrue; }
//            set { _nonEmptyStringIsTrue = value; }
//        }

//        public bool CaseSensitiveVariables
//        {
//            get { return _caseSensitiveVariables; }
//            set
//            {
//                if (value != _caseSensitiveVariables)
//                {
//                    if (value)
//                    {
//                        _variables = new Dictionary<string, object>();
//                        _types = new Dictionary<string, Type>();
//                    }
//                    else
//                    {
//                        _variables = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
//                        _types = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);
//                        
//                    }
//                    _caseSensitiveVariables = value;
//                }
//            }
//        }

        public AssignmentPermissions AssignmentPermissions
        {
            get { return _assignmentPermissions; }
            set { _assignmentPermissions = value; }
        }

        public StringComparison StringComparison
        {
            get { return _stringComparison; }
            set { _stringComparison = value; }
        }

//        public bool EmptyCollectionIsFalse
//        {
//            get { return _emptyCollectionIsFalse; }
//            set { _emptyCollectionIsFalse = value; }
//        }
//
//        public bool NotZeroIsTrue
//        {
//            get { return _notZeroIsTrue; }
//            set { _notZeroIsTrue = value; }
//        }
//
        public IFormatProvider FormatProvider
        {
            get { return _formatProvider; }
            set { _formatProvider = value; }
        }

        public void SetLocal<T>(string name, T data)
        {
            SetLocal(name, data, typeof (T));
        }

        public void SetLocal(string name, IValueWithType data)
        {
            SetLocal(name, data.Value, data.Type);
        }

        public void SetLocal(string name, object data, Type type)
        {
            _variables[name] = data;
            _types[name] = type;
        }

        public void Set(string name, object data, Type type)
        {
            if (_parentContext != null && _parentContext.Exists(name))
                _parentContext.Set(name, data, type);

            SetLocal(name, data, type);
        }

        public void Set<T>(string name, T data)
        {
            Set(name, data, typeof (T));
        }

        public void Set(string name, IValueWithType data)
        {
            Set(name, data.Value, data.Type);
        }

        public void AddType(string name, Type type)
        {
            Set(name, ContextFactory.CreateType(type));
        }

        public void AddFunction(string name, Type type, string methodName)
        {
            Set(name, ContextFactory.CreateFunction(type, methodName));
        }

        public void AddFunction(string name, Type type, string methodName, object targetObject)
        {
            Set(name, ContextFactory.CreateFunction(type, methodName, targetObject));
        }

        public void AddFunction(string name, MethodInfo methodInfo)
        {
            Set(name, ContextFactory.CreateFunction(methodInfo));
        }

        public void AddFunction(string name, MethodInfo methodInfo, object targetObject)
        {
            Set(name, ContextFactory.CreateFunction(methodInfo, targetObject));
        }

        public virtual bool Exists(string varName)
        {
            if (_variables.ContainsKey(varName))
                return true;

            if (_rootObject != null && PropertyHelper.Exists(_rootObject, varName))
                    return true;
            
            if (_parentContext == null || !_parentContext.Exists(varName))
                return false;

            return true;
        }

        public virtual bool Get(string varName, out object value, out Type type)
        {
            if (_variables.ContainsKey(varName))
            {
                value = _variables[varName];
                type = _types[varName];
            }
            else if (_rootObject != null && PropertyHelper.TryGetValue(_rootObject,varName,out value, out type))
            {
                return true;
            }
            else
            {
                if (_parentContext == null || !_parentContext.Get(varName, out value, out type))
                {
                    value = null;
                    type = typeof(object);

                    return false;
                }
            }

            if (type == typeof(object) && value != null)
                type = value.GetType();

            return true;
        }

        public bool ToBoolean(object value)
        {
            if (value != null)
            {
                if (value is bool)
                    return ((bool) value);

                if (TestBehavior(ParserContextBehavior.ZeroIsFalse))
                {
                    if (value is int || value is uint || value is short || value is ushort || value is long || value is ulong || value is byte || value is sbyte)
                        return Convert.ToInt64(value) != 0;

                    if (value is decimal)
                        return (decimal) value != 0m;

                    if (value is float || value is double)
                        return Convert.ToDouble(value) == 0.0;
                }

                if (TestBehavior(ParserContextBehavior.EmptyCollectionIsFalse))
                {
                    if (value is ICollection)
                        return ((ICollection) value).Count > 0;

                    if (value is IEnumerable)
                    {
                        IEnumerator enumerator = ((IEnumerable) value).GetEnumerator();

                        if (enumerator.MoveNext())
                            return true;

                        return false;
                    }
                }

                if (TestBehavior(ParserContextBehavior.NonEmptyStringIsTrue) && (value is string) && ((string) value).Length > 0)
                    return true;

                if (TestBehavior(ParserContextBehavior.EmptyStringIsFalse) && (value is string) && ((string) value).Length == 0)
                    return false;

                if (TestBehavior(ParserContextBehavior.NotNullIsTrue))
                    return true;
            }
            else
            {
                if (TestBehavior(ParserContextBehavior.NullIsFalse))
                    return false;
            }

            if (_parentContext != null)
                return _parentContext.ToBoolean(value);

            if (value == null)
                throw new NullReferenceException();
            else
                throw new ArgumentException("Type " + value.GetType().Name + " cannot be evaluated as boolean");
        }

        public string Format(string formatString, params object[] parameters)
        {
            return string.Format(FormatProvider, formatString, parameters);
        }
    }
}