using System;
using System.Linq;
using System.Reflection;

namespace Vici.Core
{
    public class MemberInspector
    {
        private MemberInfo _memberInfo;

        public MemberInspector(MemberInfo memberInfo)
        {
            _memberInfo = memberInfo;
        }

        public bool HasAttribute(Type attributeType, bool inherit)
        {
            return _memberInfo.IsDefined(attributeType, inherit);
        }

        public bool HasAttribute<T>(bool inherit) where T : Attribute
        {
            return _memberInfo.IsDefined(typeof(T), inherit);
        }

        public Attribute[] GetAttributes(Type attributeType, bool inherit)
        {
            return (Attribute[]) _memberInfo.GetCustomAttributes(attributeType, inherit);
        }

        public Attribute GetAttribute(Type attributeType, bool inherit)
        {
            return GetAttributes(attributeType, inherit).FirstOrDefault();
        }

        public T GetAttribute<T>(bool inherit) where T : Attribute
        {
            return (T) GetAttribute(typeof (T), inherit);
        }

        public T[] GetAttributes<T>(bool inherit) where T : Attribute
        {
            return (T[])GetAttributes(typeof(T), inherit);
        }

        public bool IsStatic
        {
            get
            {
                if (_memberInfo is PropertyInfo)
                    return (_memberInfo as PropertyInfo).GetMethod.IsStatic;
                if (_memberInfo is FieldInfo)
                    return (_memberInfo as FieldInfo).IsStatic;
                if (_memberInfo is MethodBase)
                    return (_memberInfo as MethodBase).IsStatic;
                return false;
            }
        }

        public bool IsPublic
        {
            get
            {
                if (_memberInfo is PropertyInfo)
                    return (_memberInfo as PropertyInfo).GetMethod.IsPublic;
                if (_memberInfo is FieldInfo)
                    return (_memberInfo as FieldInfo).IsPublic;
                if (_memberInfo is MethodBase)
                    return (_memberInfo as MethodBase).IsPublic;
                return false;
            }
        }

        public bool IsWritePublic
        {
            get
            {
                if (_memberInfo is PropertyInfo)
                {
                    var propertyInfo = (_memberInfo as PropertyInfo);

                    return propertyInfo.SetMethod != null && propertyInfo.SetMethod.IsPublic;
                }

                return IsPublic;
            }
            
        }

        internal bool MatchBindingFlags(BindingFlags flags)
        {
            if (flags == BindingFlags.Default)
                return true;

            if ((flags & BindingFlags.Static) == 0 && IsStatic)
                return false;

            if ((flags & BindingFlags.Instance) == 0 && !IsStatic)
                return false;

            if ((flags & BindingFlags.Public) != 0 && !IsPublic)
                return false;

            if ((flags & BindingFlags.NonPublic) != 0 && IsPublic)
                return false;

            return true;
        }


    }
}