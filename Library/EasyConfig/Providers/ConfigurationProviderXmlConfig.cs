#region License

//=============================================================================
// Vici Core - Productivity Library for .NET 3.5 
//
// Copyright (c) 2008-2012 Philippe Leybaert
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
using System.Linq;
using System.Xml.Linq;

namespace Vici.Core.Config
{
    public class ConfigurationProviderXmlConfig : IConfigurationProvider
    {
        private Dictionary<string, string> _settings = new Dictionary<string, string>();

        public ConfigurationProviderXmlConfig(XDocument xDoc)
        {
            _settings.Clear();

            foreach (var xElement in xDoc.Root.Elements())
            {
                LoadElement(xElement,"");
            }
        }

        private void LoadElement(XElement xElement, string baseKey)
        {
            if (baseKey.Length > 0)
                baseKey += '.';

            foreach (var x in xElement.Elements())
                LoadElement(x, baseKey + xElement.Name);

            _settings[baseKey + xElement.Name] = xElement.Value;
        }

        public long Version()
        {
            return 1;
        }

        public bool CanSave
        {
            get { return false; }
        }

        public string GetValue(string key, string environment)
        {
            string value = null;

            if (!string.IsNullOrEmpty(environment))
                value = GetValue(environment + '.' + key, null);

            if (value != null)
                return value;

            return _settings.TryGetValue(key, out value) ? value : null;
        }

        public IEnumerable<KeyValuePair<string, string>> EnumerateValues(string key, string environment)
        {
            if (!string.IsNullOrEmpty(environment))
                key = environment + '.' + key;

            key += '.';

            return
                from s in _settings.Keys
                where s.StartsWith(key)
                select new KeyValuePair<string, string>(s.Substring(key.Length), _settings[s]);
        }

        public void SetValue(string key, string value, string environment)
        {
            throw new NotSupportedException();
        }
    }

}