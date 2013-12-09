using System;
using System.Linq;
using System.Text;

namespace Vici.Core
{
    public class SafeStringDictionary<T> : SafeDictionary<string,T>
	{
		public SafeStringDictionary()
		{
		}

		public SafeStringDictionary(bool ignoreCase) : base(ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal)
		{
		}
	}
}
