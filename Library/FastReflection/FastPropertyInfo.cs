#region License
//=============================================================================
// Vici Core - Productivity Library for .NET 3.5 
//
// Copyright (c) 2008-2010 Philippe Leybaert
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
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Vici.Core
{
    public class FastPropertyInfo : MemberInfo
    {
        private interface IValueAccessor
        {
            object Get(object targetObject);
            void Set(object targetObject, object value);
        }

        private IValueAccessor _accessor;
        private static readonly Dictionary<Type, OpCode> _typeMap;
        private readonly PropertyInfo _propertyInfo;
        private readonly Type _targetType;

        static FastPropertyInfo()
        {
            _typeMap = new Dictionary<Type, OpCode>();
            _typeMap[typeof (sbyte)] = OpCodes.Ldind_I1;
            _typeMap[typeof (byte)] = OpCodes.Ldind_U1;
            _typeMap[typeof (char)] = OpCodes.Ldind_U2;
            _typeMap[typeof (short)] = OpCodes.Ldind_I2;
            _typeMap[typeof (ushort)] = OpCodes.Ldind_U2;
            _typeMap[typeof (int)] = OpCodes.Ldind_I4;
            _typeMap[typeof (uint)] = OpCodes.Ldind_U4;
            _typeMap[typeof (long)] = OpCodes.Ldind_I8;
            _typeMap[typeof (ulong)] = OpCodes.Ldind_I8;
            _typeMap[typeof (bool)] = OpCodes.Ldind_I1;
            _typeMap[typeof (double)] = OpCodes.Ldind_R8;
            _typeMap[typeof (float)] = OpCodes.Ldind_R4;
        }

        public FastPropertyInfo(Type targetType, string propertyName)
        {
            _targetType = targetType;

            _propertyInfo = targetType.GetProperty(propertyName);
        }

        public object GetValue(object target, object[] indexes)
        {
            if (indexes != null)
            {
                throw new NotSupportedException();
            }
            
            if (_accessor == null)
            {
                Init();
            }
            
            return _accessor.Get(target);
        }

        public void SetValue(object target, object value, object[] indexes)
        {
            if (indexes != null && indexes.Length > 0)
            {
                throw new NotSupportedException();
            }
            
            if (_accessor == null)
            {
                Init();
            }
            
            _accessor.Set(target, value);
        }

        public PropertyAttributes Attributes
        {
            get { return _propertyInfo.Attributes; }
        }

        public bool CanRead
        {
            get { return _propertyInfo.CanRead; }
        }

        public bool CanWrite
        {
            get { return _propertyInfo.CanWrite; }
        }

        public override Type DeclaringType
        {
            get { return _propertyInfo.DeclaringType; }
        }

        public override MemberTypes MemberType
        {
            get { return _propertyInfo.MemberType; }
        }

        public MethodInfo[] GetAccessors()
        {
            return _propertyInfo.GetAccessors();
        }

        public MethodInfo[] GetAccessors(bool nonPublic)
        {
            return _propertyInfo.GetAccessors(nonPublic);
        }

        private void Init()
        {
            Assembly assembly = EmitAssembly();
            
            _accessor = assembly.CreateInstance("PropertyAccessor") as IValueAccessor;
            
            if (_accessor == null)
            {
                throw new Exception("Unable to create property accessor.");
            }
        }

        private Assembly EmitAssembly()
        {
            AssemblyName assemblyName = new AssemblyName();
            
            assemblyName.Name = "PropertyAccessorAssembly";
            
            AssemblyBuilder newAssembly = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder newModule = newAssembly.DefineDynamicModule("Module");
            TypeBuilder myType = newModule.DefineType("PropertyAccessor", TypeAttributes.Public);
            
            myType.AddInterfaceImplementation(typeof (IValueAccessor));
            myType.DefineDefaultConstructor(MethodAttributes.Public);
            
            MethodBuilder getMethod = myType.DefineMethod("Get", MethodAttributes.Public | MethodAttributes.Virtual, typeof (object), new Type[] {typeof (object)});
            ILGenerator getIL = getMethod.GetILGenerator();
            MethodInfo targetGetMethod = _propertyInfo.GetGetMethod();
            
            if (targetGetMethod != null)
            {
                getIL.DeclareLocal(typeof (object));
                getIL.Emit(OpCodes.Ldarg_1);
                getIL.Emit(OpCodes.Castclass, _targetType);
                getIL.EmitCall(OpCodes.Call, targetGetMethod, null);
                
                if (targetGetMethod.ReturnType.IsValueType)
                    getIL.Emit(OpCodes.Box, targetGetMethod.ReturnType);
                
                getIL.Emit(OpCodes.Stloc_0);
                getIL.Emit(OpCodes.Ldloc_0);
            }
            else
            {
                getIL.ThrowException(typeof (MissingMethodException));
            }
            
            getIL.Emit(OpCodes.Ret);
            
            Type[] setParamTypes = new[] {typeof (object), typeof (object)};
            
            MethodBuilder setMethod = myType.DefineMethod("Set", MethodAttributes.Public | MethodAttributes.Virtual, null, setParamTypes);
            ILGenerator setIL = setMethod.GetILGenerator();
            MethodInfo targetSetMethod = _propertyInfo.GetSetMethod();
            
            if (targetSetMethod != null)
            {
                Type paramType = targetSetMethod.GetParameters()[0].ParameterType;
                
                setIL.DeclareLocal(paramType);
                
                setIL.Emit(OpCodes.Ldarg_1);
                setIL.Emit(OpCodes.Castclass, _targetType); //Cast to the source type
                setIL.Emit(OpCodes.Ldarg_2); //Load the second argument (value object)
                
                if (paramType.IsValueType)
                {
                    setIL.Emit(OpCodes.Unbox, paramType);
                    if (_typeMap.ContainsKey(paramType))
                    {
                        setIL.Emit(_typeMap[paramType]);
                    }
                    else
                    {
                        setIL.Emit(OpCodes.Ldobj, paramType);
                    }
                }
                else
                {
                    setIL.Emit(OpCodes.Castclass, paramType);
                }

                setIL.EmitCall(OpCodes.Callvirt, targetSetMethod, null);
            }
            else
            {
                setIL.ThrowException(typeof (MissingMethodException));
            }
            
            setIL.Emit(OpCodes.Ret);
            
            myType.CreateType();

            return newAssembly;
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _propertyInfo.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _propertyInfo.GetCustomAttributes(attributeType, inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _propertyInfo.IsDefined(attributeType, inherit);
        }

        public override string Name
        {
            get { return _propertyInfo.Name; }
        }

        public override Type ReflectedType
        {
            get { return _propertyInfo.ReflectedType; }
        }

        public MethodInfo GetGetMethod()
        {
            return _propertyInfo.GetGetMethod();
        }

        public MethodInfo GetGetMethod(bool nonPublic)
        {
            return _propertyInfo.GetGetMethod(nonPublic);
        }
    }
}