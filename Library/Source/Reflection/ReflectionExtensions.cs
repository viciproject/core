using System.Collections.Generic;
using System;
using System.Reflection;

namespace Vici.Core
{
	public static class ReflectionExtensions
	{
        public static TypeInspector Inspector(this Type type)
        {
            return new TypeInspector(type);
        }

	    public static MemberInspector Inspector(this MemberInfo memberInfo)
	    {
	        return new MemberInspector(memberInfo);
	    }

	    public static PropertyInspector Inspector(this PropertyInfo propertyInfo)
	    {
	        return new PropertyInspector(propertyInfo);
	    }

	    public static AssemblyInspector Inspector(this Assembly assembly)
	    {
	        return new AssemblyInspector(assembly);
	    }




    }
}