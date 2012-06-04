using System.Linq;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Vici.Core
{
    public class MethodInspector
    {
        private MethodBase _m;

        public MethodInspector(MethodBase method)
        {
            _m = method;
        }

        public Type ReflectedType
        {
            get { return _m.ReflectedType; }
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