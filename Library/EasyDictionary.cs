using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vici.Core
{
    public class EasyDictionary<TK,TV> : Dictionary<TK,TV>
    {
        private bool _returnDefaultIfNotExists = true;

        public EasyDictionary()
        {
        }

        public EasyDictionary(IEqualityComparer<TK> comparer) : base(comparer)
        {
        }

        public new TV this[TK key]
        {
            get
            {
                if (!ReturnDefaultIfNotExists)
                    return base[key];

                if (ContainsKey(key))
                    return base[key];

                return DefaultValue;
            }
            set
            {
                base[key] = value;
            }
        }

        public bool ReturnDefaultIfNotExists
        {
            get { return _returnDefaultIfNotExists; }
            set { _returnDefaultIfNotExists = value; }
        }

        public TV DefaultValue { get; set; }
    }

	public class EasyStringDictionary<T> : EasyDictionary<string,T>
	{
		public EasyStringDictionary()
		{
		}

		public EasyStringDictionary(bool ignoreCase) : base(ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal)
		{
		}
	}
}
