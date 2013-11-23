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

        private T WalkAndFindSingle<T>(Func<Type,T> f)
        {
            Type t = _t;

            while (t != null)
            {
                T result = f(t);

                if (result != null)
                    return result;

                t = t.GetTypeInfo().BaseType;
            }

            return default(T);
        }

        private T[] WalkAndFindMultiple<T>(Func<Type, IEnumerable<T>> f) where T : class
        {
            var list = new List<T>();

            Type t = _t;

            while (t != null)
            {
                IEnumerable<T> result = f(t);

                if (result != null)
                    list.AddRange(result);

                t = t.GetTypeInfo().BaseType;
            }

            return list.ToArray();
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

        public bool IsSubclassOf(Type type)
        {
            return WalkAndFindSingle(t => t.GetTypeInfo().BaseType == type);
        }

        public object DefaultValue()
        {
            if (CanBeNull)
                return null;

            return Activator.CreateInstance(_t);
        }

        public MethodInfo GetMethod(string name, Type[] types)
        {
            return WalkAndFindSingle(t => t.GetTypeInfo().GetDeclaredMethods(name).FirstOrDefault(mi => types.SequenceEqual(mi.GetParameters().Select(p => p.ParameterType))));
        }

        public MethodInfo GetMethod(string name, BindingFlags bindingFlags)
        {
            return WalkAndFindSingle(t => t.GetTypeInfo().GetDeclaredMethods(name).FirstOrDefault(mi => mi.Inspector().MatchBindingFlags(bindingFlags)));
        }

        public bool HasAttribute<T>(bool inherit) where T : Attribute
        {
            return _typeInfo.IsDefined(typeof(T), inherit);
        }

        public T GetAttribute<T>(bool inherit) where T : Attribute
        {
            return _typeInfo.GetCustomAttributes<T>(inherit).FirstOrDefault();
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
            return _typeInfo.DeclaredConstructors.ToArray();
        }

        public MemberInfo[] GetMember(string propertyName)
        {
            return WalkAndFindMultiple(t => t.GetTypeInfo().DeclaredMembers.Where(m => m.Name == propertyName));
        }

        public PropertyInfo GetIndexer(Type[] types)
        {
            return WalkAndFindSingle(t => t.GetTypeInfo().DeclaredProperties.FirstOrDefault(pi => pi.Name == "Item" && LazyBinder.MatchParameters(types, pi.GetIndexParameters())));
        }

        public T[] GetCustomAttributes<T>(bool inherit) where T:Attribute
        {
            return _typeInfo.GetCustomAttributes<T>(inherit).ToArray();
        }

        public MethodInfo GetPropertyGetter(string propertyName, Type[] parameterTypes)
        {
            return GetMethod("get_" + propertyName, parameterTypes);
        }

        public PropertyInfo GetProperty(string propName)
        {
            return WalkAndFindSingle(t => t.GetTypeInfo().GetDeclaredProperty(propName));
        }

        public FieldInfo GetField(string fieldName)
        {
            return WalkAndFindSingle(t => t.GetTypeInfo().GetDeclaredField(fieldName));
        }

        public bool ImplementsOrInherits(Type type)
        {
            if (type.GetTypeInfo().IsGenericTypeDefinition && type.GetTypeInfo().IsInterface)
            {
                return _typeInfo.ImplementedInterfaces.Any(t => (t.GetTypeInfo().IsGenericType && t.GetTypeInfo().GetGenericTypeDefinition() == type));
            }

            return type.GetTypeInfo().IsAssignableFrom(_typeInfo);
        }

        public bool ImplementsOrInherits<T>()
        {
            return ImplementsOrInherits(typeof (T));
        }

        public MethodInfo GetMethod(string methodName, BindingFlags bindingFlags, Type[] parameterTypes)
        {
            return WalkAndFindSingle(t => LazyBinder.SelectBestMethod(t.GetTypeInfo().GetDeclaredMethods(methodName),parameterTypes,bindingFlags));
        }

        public Type[] GetGenericArguments()
        {
            return _t.GenericTypeArguments;
        }


        public FieldInfo[] GetFields(BindingFlags bindingFlags)
        {
            if ((bindingFlags & BindingFlags.DeclaredOnly) != 0)
				return _typeInfo.DeclaredFields.Where(fi => fi.Inspector().MatchBindingFlags(bindingFlags)).ToArray();

            return WalkAndFindMultiple(t => t.GetTypeInfo().DeclaredFields.Where(fi => fi.Inspector().MatchBindingFlags(bindingFlags)));
        }

        public PropertyInfo[] GetProperties(BindingFlags bindingFlags)
        {
            if ((bindingFlags & BindingFlags.DeclaredOnly) != 0)
                return _typeInfo.DeclaredProperties.Where(pi => pi.Inspector().MatchBindingFlags(bindingFlags)).ToArray();

            return WalkAndFindMultiple(t => t.GetTypeInfo().DeclaredProperties.Where(pi => pi.Inspector().MatchBindingFlags(bindingFlags)));
        }

        public Type[] GetInterfaces()
        {
            return _typeInfo.ImplementedInterfaces.ToArray();
        }

		public FieldOrPropertyInfo[] GetFieldsAndProperties(BindingFlags bindingFlags)
		{
			MemberInfo[] members;

			if ((bindingFlags & BindingFlags.DeclaredOnly) != 0)
                members = _typeInfo.DeclaredFields.Where(fi => fi.Inspector().MatchBindingFlags(bindingFlags)).Union<MemberInfo>(_typeInfo.DeclaredProperties.Where(pi => pi.Inspector().MatchBindingFlags(bindingFlags))).ToArray();
			else
                members = WalkAndFindMultiple(t => t.GetTypeInfo().DeclaredFields.Where(fi => fi.Inspector().MatchBindingFlags(bindingFlags)).Union<MemberInfo>(t.GetTypeInfo().DeclaredProperties.Where(pi => pi.Inspector().MatchBindingFlags(bindingFlags))));

			return members.Select(m => new FieldOrPropertyInfo(m)).ToArray();
		}
    }


}