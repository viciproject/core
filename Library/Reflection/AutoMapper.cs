using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Vici.Core
{
	public class AutoMapper
	{
		public bool IncludePrivate { get; set; }
		public bool IncludeInherited { get; set; }
		public IEqualityComparer<string> Comparer { private get; set; }

		public AutoMapper(bool includePrivate = false, bool includeInherited = false, bool ignoreCase = false)
		{
			IncludePrivate = includePrivate;
			IncludeInherited = includeInherited;
			IgnoreCase = ignoreCase;
		}

		public static AutoMapper Mapper(bool includePrivate = false, bool includeInherited = false, bool ignoreCase = false)
		{
			return new AutoMapper {
				IncludePrivate = includePrivate,
				IncludeInherited = includeInherited,
				IgnoreCase = ignoreCase
			};
		}

		public bool IgnoreCase
		{
			set 
			{
				if (value)
					Comparer = StringComparer.OrdinalIgnoreCase;
				else
					Comparer = StringComparer.Ordinal;
			}
		}


		public void FillObject<T>(Func<string,Tuple<object,bool>> valueProvider)
		{
			FillObject(typeof(T), valueProvider);

		    Converter<string, int> x;
		}
        
		public void FillObject(object o, Func<string,Tuple<object,bool>> valueProvider)
		{
			Type type = o.GetType();

			if (o is Type)
			{
				type = (Type)o;
				o = null;
			}

			var fields = GetFields(type, o == null);

			HashSet<string> mappedNames = new HashSet<string>(Comparer);

			foreach (var field in fields)
			{
				if (mappedNames.Contains(field.Name))
					continue;

				var result = valueProvider(field.Name);

				if (result.Item2)
				{
					field.SetValue(o, result.Item1);
					mappedNames.Add(field.Name);
				}
			}
		}


		public void FillObject<T>(Dictionary<string,object> values)
		{
			FillObject(typeof(T), values.Select(kvp => Tuple.Create(kvp.Key,kvp.Value)));
		}

		public void FillObject(object o, Dictionary<string,object> values)
		{
			FillObject(o, name => values.ContainsKey(name) ? Tuple.Create((object)null,false) : Tuple.Create(values[name],true));
		}

		public void FillObject<T>(IEnumerable<Tuple<string,object>> values)
		{
			FillObject(typeof(T), values);
		}

		public void FillObject(object o, IEnumerable<Tuple<string,object>> values)
		{
			var type = ResolveObjectType(ref o);

			var fields = GetFields(type, o == null);

			foreach (var item in values)
			{
				var name = item.Item1;

				foreach (var field in fields)
					if (Comparer.Equals(name, field.Name))
					{
						field.SetValue(o, item.Item2.Convert(field.FieldType));
						break;
					}
			}
		}

		//---- Private helpers

		private Type ResolveObjectType(ref object o)
		{
			Type type = o.GetType();

			if (o is Type)
			{
				type = (Type)o;
				o = null;
			}

			return type;
		}

		private FieldOrPropertyInfo[] GetFields(Type type, bool staticMembers)
		{
			BindingFlags bindingFlags = BindingFlags.Public;

			if (!staticMembers)
				bindingFlags |= BindingFlags.Instance;
			else
				bindingFlags |= BindingFlags.Static;

			if (IncludePrivate)
				bindingFlags |= BindingFlags.NonPublic;

			if (!IncludeInherited)
				bindingFlags |= BindingFlags.DeclaredOnly;

			return type.Inspector().GetFieldsAndProperties(bindingFlags);
		}

	}
}

