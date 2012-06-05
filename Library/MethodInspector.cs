using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;
using Vici.Core.Parser;

namespace Vici.Core
{
    public class MethodInspector
    {
        private MethodBase _m;

        public MethodInspector(MethodBase method)
        {
            _m = method;
        }

        public object Invoke(object o, object[] parameters)
        {
#if NETFX_CORE
            var parameterTypes = _m.GetParameters();

            object[] newParameters = new object[parameters.Length];

            for (int i = 0; i < parameters.Length;i++ )
            {
                newParameters[i] = parameters[i].Convert(parameterTypes[i].ParameterType);
            }

            return _m.Invoke(o, newParameters);
#else
            return _m.Invoke(o, BindingFlags.Default, LazyBinder.Default, parameters, null);
#endif
        }
    }

    public static class MethodInspectorExtension
    {
        public static MethodInspector Inspector(this MethodBase method)
        {
            return new MethodInspector(method);
        }
    }

    public class MissingMethodException : Exception
    {
        public MissingMethodException(string name)
        {
            
        }
    }

}