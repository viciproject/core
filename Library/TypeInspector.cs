using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Vici.Core
{
#if !NETFX_CORE
    public static class TypeInfoMocker { public static Type GetTypeInfo(this Type type) { return type; } }
#endif
    public class TypeInspector
    {
        private Type _t;

        public TypeInspector(Type type)
        {
            _t = type;
        }

        public bool IsGenericType
        {
#if NETFX_CORE
            get { return _t.GetTypeInfo().IsGenericType; }
#else
            get { return _t.IsGenericType; }
#endif
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
#if NETFX_CORE
            get { return _t.GetTypeInfo().IsPrimitive; }
#else
            get { return _t.IsPrimitive; }
#endif
        }

        public bool IsValueType
        {
            get { return _t.GetTypeInfo().IsValueType; }
        }

        public Type BaseType
        {
            get { return _t.GetTypeInfo().BaseType; }
        }

        public bool IsEnum
        {
            get { return _t.GetTypeInfo().IsEnum; }
        }

        public object DefaultValue()
        {
            if (CanBeNull)
                return null;

            return Activator.CreateInstance(_t);
        }

        public MethodInfo GetMethod(string name, Type[] types)
        {
            return _t.GetMethod(name, types);
        }

        public bool HasAttribute<T>(bool inherit) where T : Attribute
        {
            return _t.GetTypeInfo().IsDefined(typeof(T), inherit);
        }

        public T GetAttribute<T>(bool inherit) where T : Attribute
        {
            T[] attributes = (T[]) _t.GetCustomAttributes(typeof(T), inherit);

            return attributes.Length > 0 ? attributes[0] : null;
        }

        public T[] GetAttributes<T>(bool inherit) where T : Attribute
        {
            return (T[])_t.GetCustomAttributes(typeof(T), inherit);
        }

        public bool IsAssignableFrom(Type type)
        {
            return _t.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        public ConstructorInfo[] GetConstructors()
        {
            return _t.GetConstructors();
        }

        public MemberInfo[] GetMember(string propertyName)
        {
            return _t.GetMember(propertyName);
        }

        public PropertyInfo GetIndexer(Type[] types)
        {
            return _t.GetProperty("Item", new[] { typeof(string) });
        }

        public object[] GetCustomAttributes(Type type, bool inherit)
        {
            return _t.GetCustomAttributes(type, inherit);
        }

        public MethodInfo GetPropertyGetter(string propertyName, Type[] parameterTypes)
        {
             return _t.GetMethod("get_" + propertyName, parameterTypes);
        }

        public PropertyInfo GetProperty(string propName)
        {
            return _t.GetProperty(propName);
        }

        public FieldInfo GetField(string fieldName)
        {
            return _t.GetField(fieldName);
        }

        public bool ImplementsOrInherits(Type type)
        {
            if (type.GetTypeInfo().IsGenericTypeDefinition && type.GetTypeInfo().IsInterface)
            {
                return _t.FindInterfaces((t, criteria) => (IsGenericType && _t.GetTypeInfo().GetGenericTypeDefinition() == type), null).Any();
            }

            return type.GetTypeInfo().IsAssignableFrom(_t.GetTypeInfo());
        }

        public bool ImplementsOrInherits<T>()
        {
            return ImplementsOrInherits(typeof (T));
        }

        public MethodInfo GetMethod(string methodName, BindingFlags bindingFlags, Binder binder, Type[] parameterTypes, ParameterModifier[] modifiers)
        {
            return _t.GetMethod(methodName, bindingFlags, binder, parameterTypes, modifiers);
        }

        public Type[] GetGenericArguments()
        {
            return _t.GetGenericArguments();
        }

        public FieldInfo[] GetFields(BindingFlags bindingFlags)
        {
            return _t.GetFields(bindingFlags);
        }

        public PropertyInfo[] GetProperties(BindingFlags bindingFlags)
        {
            return _t.GetProperties(bindingFlags);
        }

        public Type[] GetInterfaces()
        {
            return _t.GetInterfaces();
        }
    }

    public static class TypeInspectorExtension
    {
        public static TypeInspector Inspector(this Type type)
        {
            return new TypeInspector(type);
        }
    }

}