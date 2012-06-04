using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Vici.Core
{
    public class TypeInspector
    {
        private Type _t;

        public TypeInspector(Type type)
        {
            _t = type;
        }

        public bool IsNullable
        {
            get { return (_t.IsGenericType && _t.GetGenericTypeDefinition() == typeof (Nullable<>)); }
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
            get { return _t.IsPrimitive; }
        }

        public bool IsValueType
        {
            get { return _t.IsValueType; }
        }

        public Type BaseType
        {
            get { return _t.BaseType; }
        }

        public bool IsEnum
        {
            get { return _t.IsEnum; }
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
            return _t.IsDefined(typeof(T), inherit);
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
            return _t.IsAssignableFrom(type);
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
            if (type.IsGenericTypeDefinition && type.IsInterface)
            {
                return _t.FindInterfaces((t, criteria) => (t.IsGenericType && t.GetGenericTypeDefinition() == type), null).Any();
            }

            return type.IsAssignableFrom(_t);
        }

        public bool ImplementsOrInherits<T>()
        {
            return ImplementsOrInherits(typeof (T));
        }

        public MethodInfo GetMethod(string methodName, BindingFlags bindingFlags, Binder binder, Type[] parameterTypes, ParameterModifier[] modifiers)
        {
            return _t.GetMethod(methodName, bindingFlags, binder, parameterTypes, modifiers);
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