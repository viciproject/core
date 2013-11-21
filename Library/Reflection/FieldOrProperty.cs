//=============================================================================
// Vici Parser - Generic expression and template parser for .NET 
//
// Copyright (c) 2008-2009 Philippe Leybaert
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
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Vici.Core
{
    public delegate object Getter();
    public delegate T Getter<T>();
    public delegate void Setter(object o);
    public delegate void Setter<T>(T o);

    public class FieldOrProperty
    {
        private readonly MemberInfo _memberInfo;
        private readonly object _targetObject = null;

        public FieldOrProperty(Type type, string name, bool includePrivate, object targetObject)
            : this(type, name, includePrivate)
        {
            _targetObject = targetObject;
        }

        public FieldOrProperty(Type type, string name, bool includePrivate)
        {
            MemberInfo[] members;

			//members = from member in type.GetTypeInfo().DeclaredMembers where member.



            if (includePrivate)
                members = type.GetMember(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
            else
                members = type.GetMember(name);

            if (members.Length > 0 && !(members[0] is FieldInfo) && !(members[0] is PropertyInfo))
            {
                throw new AmbiguousMatchException();
            }

            MemberInfo member = null;

            if (members.Length > 1) // Can happen with types created by Reflection.Emit
            {
                for (int i = 0; i < members.Length; i++)
                    if (members[i].DeclaringType == type)
                    {
                        member = members[i];
                        break;
                    }

                if (member == null)
                    member = members[0];
            }
            else
            {
                member = members[0];
            }

            _memberInfo = member;
        }

        public FieldOrProperty(MemberInfo memberInfo)
        {
            _memberInfo = memberInfo;
        }

        public FieldOrProperty(MemberInfo memberInfo, object targetObject)
            : this(memberInfo)
        {
            _targetObject = targetObject;
        }

        public FieldOrProperty OfInstance(object targetObject)
        {
            return new FieldOrProperty(_memberInfo, targetObject);
        }

        public object Value
        {
            get { return GetValue(_targetObject); }
            set { SetValue(_targetObject, value); }
        }

        public string Name
        {
            get { return _memberInfo.Name; }
        }

        public Type Type
        {
            get
            {
                return (_memberInfo is FieldInfo) ? ((FieldInfo)_memberInfo).FieldType :
                        (_memberInfo is PropertyInfo) ? ((PropertyInfo)_memberInfo).PropertyType :
                        null;
            }
        }

        public bool IsField { get { return _memberInfo is FieldInfo; } }
        public bool IsProperty { get { return _memberInfo is PropertyInfo; } }

        private FieldInfo AsField { get { return _memberInfo as FieldInfo; } }
        private PropertyInfo AsProperty { get { return _memberInfo as PropertyInfo; } }

        public Setter SetterDelegate()
        {
            return delegate(object value) { Value = value; };
        }

        public Setter<T> SetterDelegate<T>()
        {
            return delegate(T value) { Value = value; };
        }

        public Getter GetterDelegate()
        {
            return delegate { return Value; };
        }

        public Getter<T> GetterDelegate<T>()
        {
            return delegate { return (T)Value; };
        }

        public object GetValue(object targetObject)
        {
            if (IsField)
                return AsField.GetValue(targetObject);

            return AsProperty.GetValue(targetObject, null);
        }

        public void SetValue(object targetObject, object value)
        {
            if (IsField)
                AsField.SetValue(targetObject, value);
            else
                AsProperty.SetValue(targetObject, value, null);
        }

        public bool IsStatic
        {
            get
            {
                if (IsField)
                    return AsField.IsStatic;

                return (AsProperty.CanRead && AsProperty.GetGetMethod(true).IsStatic) || (AsProperty.CanWrite && AsProperty.GetSetMethod(true).IsStatic);
            }
        }
    }
}