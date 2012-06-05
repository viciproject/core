using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;
using Vici.Core.Parser;

namespace Vici.Core
{
#if !NETFX_CORE
    public static class TypeInfoMocker
    {
        public static Type GetTypeInfo(this Type type) { return type; }
    }
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
            get { return _t.GetTypeInfo().IsGenericType; }
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
            get { return _t.GetTypeInfo().IsPrimitive; }
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
#if NETFX_CORE        
            return _t.GetTypeInfo().GetCustomAttributes<T>(inherit).FirstOrDefault();
#else
			return (T) _t.GetCustomAttributes(typeof(T),inherit).FirstOrDefault();
#endif            
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
            return WalkAndFindMultiple(t => t.GetTypeInfo().DeclaredMembers.Where(m => m.Name == propertyName));
#else
            return _t.GetMember(propertyName);
#endif
        }

        public PropertyInfo GetIndexer(Type[] types)
        {
#if NETFX_CORE
            return WalkAndFindSingle(t => t.GetTypeInfo().DeclaredProperties.FirstOrDefault(pi => pi.Name == "Item" && LazyBinder.ParametersMatch(types, pi.GetIndexParameters())));
#else            
            return _t.GetProperty("Item", new[] { typeof(string) });
#endif
        }

        public T[] GetCustomAttributes<T>(bool inherit) where T:Attribute
        {
#if NETFX_CORE
            return _t.GetTypeInfo().GetCustomAttributes<T>(inherit).ToArray();;
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
#if NETFX_CORE
                return _t.GetTypeInfo().ImplementedInterfaces.Any(t => (t.GetTypeInfo().IsGenericType && t.GetTypeInfo().GetGenericTypeDefinition() == type));
#else
                return _t.FindInterfaces((t, criteria) => (t.GetTypeInfo().IsGenericType && t.GetTypeInfo().GetGenericTypeDefinition() == type), null).Any();
#endif

            }

            return type.GetTypeInfo().IsAssignableFrom(_t.GetTypeInfo());
        }

        public bool ImplementsOrInherits<T>()
        {
            return ImplementsOrInherits(typeof (T));
        }

        public MethodInfo GetMethod(string methodName, BindingFlags bindingFlags, Type[] parameterTypes, ParameterModifier[] modifiers)
        {
#if NETFX_CORE
            Binder binder = new Binder();

            return (MethodInfo) WalkAndFindSingle(t => binder.BestMatch(t.GetTypeInfo().GetDeclaredMethods(methodName),parameterTypes));
#else
            return _t.GetMethod(methodName, bindingFlags, LazyBinder.Default, parameterTypes, modifiers);
#endif
        }

        public Type[] GetGenericArguments()
        {
#if NETFX_CORE
            return _t.GenericTypeArguments;
#else
            return _t.GetGenericArguments();
#endif
        }

#if NETFX_CORE
        private bool MatchBindingFlags(FieldInfo fieldInfo, BindingFlags flags)
        {
            if ((flags & BindingFlags.Static) == 0 && fieldInfo.IsStatic)
                return false;

            if ((flags & BindingFlags.Instance) == 0 && !fieldInfo.IsStatic)
                return false;

            if ((flags & BindingFlags.Public) != 0 && !fieldInfo.IsPublic)
                return false;

            if ((flags & BindingFlags.NonPublic) != 0 && !fieldInfo.IsPrivate)
                return false;

            return true;
        }

        private bool MatchBindingFlags(PropertyInfo propertyInfo, BindingFlags flags)
        {
            if ((flags & BindingFlags.Static) == 0 && propertyInfo.GetMethod.IsStatic)
                return false;

            if ((flags & BindingFlags.Instance) == 0 && !propertyInfo.GetMethod.IsStatic)
                return false;

            return true;
        }
#endif

        public FieldInfo[] GetFields(BindingFlags bindingFlags)
        {
#if NETFX_CORE
            if ((bindingFlags & BindingFlags.DeclaredOnly) != 0)
                return _t.GetTypeInfo().DeclaredFields.Where(fi => MatchBindingFlags(fi, bindingFlags)).ToArray();
            else
                return WalkAndFindMultiple(t => t.GetTypeInfo().DeclaredFields.Where(fi => MatchBindingFlags(fi, bindingFlags)));
#else
            return _t.GetFields(bindingFlags);
#endif
        }

        public PropertyInfo[] GetProperties(BindingFlags bindingFlags)
        {
#if NETFX_CORE
            if ((bindingFlags & BindingFlags.DeclaredOnly) != 0)
                return _t.GetTypeInfo().DeclaredProperties.Where(pi => MatchBindingFlags(pi, bindingFlags)).ToArray();
            else
                return WalkAndFindMultiple(t => t.GetTypeInfo().DeclaredProperties.Where(pi => MatchBindingFlags(pi, bindingFlags)));
#else
            return _t.GetProperties(bindingFlags);
#endif
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