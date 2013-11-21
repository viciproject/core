using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Vici.Core
{
	public static class ReflectionExtensions
	{
		public class MemberWrapper
		{
			MemberInfo _memberInfo;

			public MemberWrapper(MemberInfo memberInfo)
			{
				_memberInfo = memberInfo;
			}

			public bool IsStatic
			{
				get
				{ 
					if (_memberInfo is PropertyInfo)
						return (_memberInfo as PropertyInfo).GetMethod.IsStatic;
					if (_memberInfo is FieldInfo)
						return (_memberInfo as FieldInfo).IsStatic;
					if (_memberInfo is MethodInfo)
						return (_memberInfo as MethodInfo).IsStatic;
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
					if (_memberInfo is MethodInfo)
						return (_memberInfo as MethodInfo).IsPublic;
					return false;
				}
			}
		}

        public static TypeInspector Inspector(this Type type)
        {
            return new TypeInspector(type);
        }

		internal static bool MatchBindingFlags(this MemberInfo memberInfo, BindingFlags flags)
		{
			MemberWrapper member = new MemberWrapper(memberInfo);

			if (flags == BindingFlags.Default)
				return true;

			if ((flags & BindingFlags.Static) == 0 && member.IsStatic)
				return false;

			if ((flags & BindingFlags.Instance) == 0 && !member.IsStatic)
				return false;

			if ((flags & BindingFlags.Public) != 0 && !member.IsPublic)
				return false;

			if ((flags & BindingFlags.NonPublic) != 0 && member.IsPublic)
				return false;

			return true;
		}

		public static bool HasAttribute<T>(this MemberInfo type, bool inherit) where T : Attribute
		{
			return type.IsDefined(typeof(T), inherit);
		}

		public static T GetAttribute<T>(this MemberInfo type, bool inherit) where T : Attribute
		{
			T[] attributes = (T[])type.GetCustomAttributes(typeof(T), inherit);

			return attributes.Length > 0 ? attributes[0] : null;
		}

		public static T[] GetAttributes<T>(this MemberInfo type, bool inherit) where T : Attribute
		{
			return (T[])type.GetCustomAttributes(typeof(T), inherit);
		}

    }

}