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
using System.Reflection;

namespace Vici.Core.Config
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

        public object[] GetCustomAttributes(Type type, bool inherit)
        {
#if NETFX_CORE
            return new[] {_memberInfo.GetCustomAttributes(type, inherit)};
#else
            return _memberInfo.GetCustomAttributes(type, inherit);
#endif
        }

        public bool IsDefined(Type type, bool b)
        {
            return _memberInfo.IsDefined(type, b);
        }
    }
}