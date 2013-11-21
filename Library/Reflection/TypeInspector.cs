using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Vici.Core
{
    public class TypeInspector
    {
        private readonly Type _t;
		private readonly TypeInfo _typeInfo;

        public TypeInspector(Type type)
        {
            _t = type;
			_typeInfo = type.GetTypeInfo();
        }

        private T WalkAndFindSingle<T>(Func<Type,T> f) where T:class 
        {
            Type t = _t;

            while (t != null)
            {
                T result = f(t);

                if (result != null)
                    return result;

                t = t.GetTypeInfo().BaseType;
            }

            return null;
        }

        private T[] WalkAndFindMultiple<T>(Func<Type, IEnumerable<T>> f) where T : class
        {
            List<T> array = new List<T>();

            Type t = _t;

            while (t != null)
            {
                IEnumerable<T> result = f(t);

                if (result != null)
                    array.AddRange(result);

                t = t.GetTypeInfo().BaseType;
            }

            return array.ToArray();
        }

        public bool IsGenericType
        {
			get { return _typeInfo.IsGenericType; }
        }

        public bool IsNullable
        {
            get { return (IsGenericType && _t.GetGenericTypeDefinition() == typeof (Nullable<>)); }
        }

        public bool CanBeNull
        {
            get { return !IsValueType || IsNullable; }
        }

        public Type RealType
        {
            get { return Nullable.GetUnderlyingType(_t) ?? _t; }
        }

        public bool IsPrimitive
        {
			get { return _typeInfo.IsPrimitive; }
        }

        public bool IsValueType
        {
			get { return _typeInfo.IsValueType; }
        }

        public Type BaseType
        {
			get { return _typeInfo.BaseType; }
        }

        public bool IsEnum
        {
			get { return _typeInfo.IsEnum; }
        }

        public object DefaultValue()
        {
            if (CanBeNull)
                return null;

            return Activator.CreateInstance(_t);
        }

        public MethodInfo GetMethod(string name, Type[] types)
        {
#if PCL
            return WalkAndFindSingle(t => t.GetTypeInfo().GetDeclaredMethods(name).FirstOrDefault(mi => types.SequenceEqual(mi.GetParameters().Select(p => p.ParameterType))));
#else
            return _t.GetMethod(name, types);
#endif
        }

        public bool HasAttribute<T>(bool inherit) where T : Attribute
        {
            return _typeInfo.IsDefined(typeof(T), inherit);
        }

        public T GetAttribute<T>(bool inherit) where T : Attribute
        {
#if PCL
            return _typeInfo.GetCustomAttributes<T>(inherit).FirstOrDefault();
#else
			return (T) _t.GetCustomAttributes(typeof(T),inherit).FirstOrDefault();
#endif            
        }

        public T[] GetAttributes<T>(bool inherit) where T : Attribute
        {
            return (T[])_typeInfo.GetCustomAttributes(typeof(T), inherit).ToArray();
        }

        public bool IsAssignableFrom(Type type)
        {
            return _typeInfo.IsAssignableFrom(type.GetTypeInfo());
        }

        public ConstructorInfo[] GetConstructors()
        {
#if PCL
            return _typeInfo.DeclaredConstructors.ToArray();
#else
            return _t.GetConstructors();
#endif
        }

        public MemberInfo[] GetMember(string propertyName)
        {
#if PCL
            return WalkAndFindMultiple(t => t.GetTypeInfo().DeclaredMembers.Where(m => m.Name == propertyName));
#else
            return _t.GetMember(propertyName);
#endif
        }

        public PropertyInfo GetIndexer(Type[] types)
        {
#if PCL
            return WalkAndFindSingle(t => t.GetTypeInfo().DeclaredProperties.FirstOrDefault(pi => pi.Name == "Item" && LazyBinder.MatchParameters(types, pi.GetIndexParameters())));
#else            
            return _t.GetProperty("Item", null, types);
#endif
        }

        public T[] GetCustomAttributes<T>(bool inherit) where T:Attribute
        {
#if PCL
            return _typeInfo.GetCustomAttributes<T>(inherit).ToArray();
#else
            return (T[]) _t.GetCustomAttributes(typeof(T), inherit);
#endif
        }

        public MethodInfo GetPropertyGetter(string propertyName, Type[] parameterTypes)
        {
            return GetMethod("get_" + propertyName, parameterTypes);
        }

        public PropertyInfo GetProperty(string propName)
        {
#if PCL
            return WalkAndFindSingle(t => t.GetTypeInfo().GetDeclaredProperty(propName));
#else
            return _t.GetProperty(propName);
#endif
        }

        public FieldInfo GetField(string fieldName)
        {
#if PCL
            return WalkAndFindSingle(t => t.GetTypeInfo().GetDeclaredField(fieldName));
#else
            return _t.GetField(fieldName);
#endif
        }

        public bool ImplementsOrInherits(Type type)
        {
            if (type.GetTypeInfo().IsGenericTypeDefinition && type.GetTypeInfo().IsInterface)
            {
#if PCL
                return _typeInfo.ImplementedInterfaces.Any(t => (t.GetTypeInfo().IsGenericType && t.GetTypeInfo().GetGenericTypeDefinition() == type));
#else
                return _t.FindInterfaces((t, criteria) => (t.GetTypeInfo().IsGenericType && t.GetTypeInfo().GetGenericTypeDefinition() == type), null).Any();
#endif

            }

            return type.GetTypeInfo().IsAssignableFrom(_typeInfo);
        }

        public bool ImplementsOrInherits<T>()
        {
            return ImplementsOrInherits(typeof (T));
        }

        public MethodInfo GetMethod(string methodName, BindingFlags bindingFlags, Type[] parameterTypes)
        {
            return (MethodInfo) WalkAndFindSingle(t => LazyBinder.SelectBestMethod(t.GetTypeInfo().GetDeclaredMethods(methodName),parameterTypes,bindingFlags));
        }

        public Type[] GetGenericArguments()
        {
#if PCL
            return _t.GenericTypeArguments;
#else
            return _t.GetGenericArguments();
#endif
        }


        public FieldInfo[] GetFields(BindingFlags bindingFlags)
        {
#if PCL
            if ((bindingFlags & BindingFlags.DeclaredOnly) != 0)
				return _typeInfo.DeclaredFields.Where(fi => fi.MatchBindingFlags(bindingFlags)).ToArray();
            else
				return WalkAndFindMultiple(t => t.GetTypeInfo().DeclaredFields.Where(fi => fi.MatchBindingFlags(bindingFlags)));
#else
            return _t.GetFields(bindingFlags);
#endif
        }

        public PropertyInfo[] GetProperties(BindingFlags bindingFlags)
        {
#if PCL
            if ((bindingFlags & BindingFlags.DeclaredOnly) != 0)
				return _typeInfo.DeclaredProperties.Where(pi => pi.MatchBindingFlags(bindingFlags)).ToArray();
            else
				return WalkAndFindMultiple(t => t.GetTypeInfo().DeclaredProperties.Where(pi => pi.MatchBindingFlags(bindingFlags)));
#else
            return _t.GetProperties(bindingFlags);
#endif
        }

        public Type[] GetInterfaces()
        {
            return _typeInfo.ImplementedInterfaces.ToArray();
        }

		public FieldOrPropertyInfo[] GetFieldsAndProperties(BindingFlags bindingFlags)
		{
			MemberInfo[] members;

			if ((bindingFlags & BindingFlags.DeclaredOnly) != 0)
				members = _typeInfo.DeclaredFields.Where(fi => fi.MatchBindingFlags(bindingFlags)).Union<MemberInfo>(_typeInfo.DeclaredProperties.Where(pi => pi.MatchBindingFlags(bindingFlags))).ToArray();
			else
				members = WalkAndFindMultiple(t => t.GetTypeInfo().DeclaredFields.Where(fi => fi.MatchBindingFlags(bindingFlags)).Union<MemberInfo>(t.GetTypeInfo().DeclaredProperties.Where(pi => pi.MatchBindingFlags(bindingFlags))));

			return members.Select(m => new FieldOrPropertyInfo(m)).ToArray();
		}
    }


}