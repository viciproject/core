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
using System.Linq;
using System.Reflection;

namespace Vici.Core
{
    public class FieldOrPropertyInfo
    {
        private MemberInfo _memberInfo;
        public Type FieldType;

        public FieldOrPropertyInfo(MemberInfo memberInfo)
        {
            _memberInfo = memberInfo;

            FieldType = (_memberInfo is FieldInfo) ? ((FieldInfo)_memberInfo).FieldType : ((PropertyInfo)_memberInfo).PropertyType;
        }

        public string Name
        {
            get { return _memberInfo.Name; }
        }

        public object GetValue(object o)
        {
            return _memberInfo is FieldInfo ? ((FieldInfo)_memberInfo).GetValue(o) : ((PropertyInfo)_memberInfo).GetValue(o, null);
        }

        public void SetValue(object o, object value)
        {
            if (_memberInfo is FieldInfo)
                ((FieldInfo) _memberInfo).SetValue(o, value);
            else
                ((PropertyInfo)_memberInfo).SetValue(o, value, null);
        }

		public bool IsField { get { return _memberInfo is FieldInfo; } }
		public bool IsProperty { get { return _memberInfo is PropertyInfo; } }

		private FieldInfo AsField { get { return _memberInfo as FieldInfo; } }
		private PropertyInfo AsProperty { get { return _memberInfo as PropertyInfo; } }

		public Action<object,object> Setter()
		{
			return delegate(object target, object value) { SetValue(target,value); };
		}

		public Action<object> Setter(object target)
		{
			return delegate(object value) { SetValue(target,value); };
		}

		public Action<object,T> Setter<T>()
		{
			return delegate(object target, T value) { SetValue(target,value); };
		}

		public Action<T> Setter<T>(object target)
		{
			return delegate(T value) { SetValue(target,value); };
		}

		public Func<object,object> Getter()
		{
			return delegate(object target) { return GetValue(target); };
		}

		public Func<object,T> Getter<T>()
		{
			return delegate(object target) { return (T) GetValue(target); };
		}

		public Func<object> Getter(object target)
		{
			return delegate() { return GetValue(target); };
		}

		public Func<T> Getter<T>(object target)
		{
			return delegate() { return (T) GetValue(target); };
		}

		public bool IsPrivate
		{
			get 
			{ 
				if (IsField)
					return AsField.IsPrivate;

				var method = AsProperty.GetMethod ?? AsProperty.SetMethod;

				return method.IsPrivate;
			}
		}

		public bool IsStatic
		{
			get
			{
				if (IsField)
					return AsField.IsStatic;

				var method = AsProperty.GetMethod ?? AsProperty.SetMethod;

				return method.IsStatic;
			}
		}

        public Attribute[] GetCustomAttributes(Type type, bool inherit)
        {
#if PCL
            return _memberInfo.GetCustomAttributes(type, inherit).ToArray();
#else
            return (Attribute[]) _memberInfo.GetCustomAttributes(type, inherit);
#endif
        }

        public T[] GetCustomAttributes<T>(bool inherit) where T:Attribute
        {
#if PCL
            return _memberInfo.GetCustomAttributes<T>(inherit).ToArray();
#else
            return (T[]) _memberInfo.GetCustomAttributes(typeof(T), inherit);
#endif
        }

        public bool IsDefined(Type type, bool b)
        {
            return _memberInfo.IsDefined(type, b);
        }
    }
}