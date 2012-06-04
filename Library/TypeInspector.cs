using System.Collections;
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

#if NETFX_CORE

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

#endif

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
#if NETFX_CORE
            return WalkAndFindSingle(t => t.GetTypeInfo().GetDeclaredMethods(name).FirstOrDefault(mi => types.SequenceEqual(mi.GetParameters().Select(p => p.ParameterType))));
#else
            return _t.GetMethod(name, types);
#endif
        }

        public bool HasAttribute<T>(bool inherit) where T : Attribute
        {
            return _t.GetTypeInfo().IsDefined(typeof(T), inherit);
        }

        public T GetAttribute<T>(bool inherit) where T : Attribute
        {
            T[] attributes = (T[]) _t.GetTypeInfo().GetCustomAttributes(typeof(T), inherit).ToArray();

            return attributes.Length > 0 ? attributes[0] : null;
        }

        public T[] GetAttributes<T>(bool inherit) where T : Attribute
        {
            return (T[])_t.GetTypeInfo().GetCustomAttributes(typeof(T), inherit).ToArray();
        }

        public bool IsAssignableFrom(Type type)
        {
            return _t.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        public ConstructorInfo[] GetConstructors()
        {
#if NETFX_CORE
            return _t.GetTypeInfo().DeclaredConstructors.ToArray();
#else
            return _t.GetConstructors();
#endif
        }

        public MemberInfo[] GetMember(string propertyName)
        {
#if NETFX_CORE
            return WalkAndFindMultiple(t => t.GetTypeInfo().DeclaredMembers);
#else
            return _t.GetMember(propertyName);
#endif
        }

        public PropertyInfo GetIndexer(Type[] types)
        {
            

            return _t.GetProperty("Item", new[] { typeof(string) });
        }

        public object[] GetCustomAttributes(Type type, bool inherit)
        {
#if NETFX_CORE
            return _t.GetTypeInfo().GetCustomAttributes(type, inherit).ToArray();
#else
            return _t.GetCustomAttributes(type, inherit);
#endif
        }

        public MethodInfo GetPropertyGetter(string propertyName, Type[] parameterTypes)
        {
            return GetMethod("get_" + propertyName, parameterTypes);
        }

        public PropertyInfo GetProperty(string propName)
        {
#if NETFX_CORE
            return WalkAndFindSingle(t => t.GetTypeInfo().GetDeclaredProperty(propName));
#else
            return _t.GetProperty(propName);
#endif
        }

        public FieldInfo GetField(string fieldName)
        {
#if NETFX_CORE
            return WalkAndFindSingle(t => t.GetTypeInfo().GetDeclaredField(fieldName));
#else
            return _t.GetField(fieldName);
#endif
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
#if NETFX_CORE
            return _t.GenericTypeArguments;
#else
            return _t.GetGenericArguments();
#endif
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
#if NETFX_CORE
            return _t.GetTypeInfo().ImplementedInterfaces.ToArray();
#else
            return _t.GetInterfaces();
#endif
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