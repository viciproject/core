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
            return _m.Invoke(o, BindingFlags.Default, LazyBinder.Default, parameters, null);
        }
    }

    public static class MethodInspectorExtension
    {
        public static MethodInspector Inspector(this MethodBase method)
        {
            return new MethodInspector(method);
        }
    }

}