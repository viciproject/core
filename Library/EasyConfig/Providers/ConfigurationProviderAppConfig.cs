#region License
//=============================================================================
// Vici Core - Productivity Library for .NET 3.5 
//
// Copyright (c) 2008-2011 Philippe Leybaert
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
using System.Configuration;

namespace Vici.Core.Config
{
#if !MONOTOUCH && !WINDOWS_PHONE
    public class ConfigurationProviderAppConfig : IConfigurationProvider
    {
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
                value = ConfigurationManager.AppSettings[environment + '.' + key];

            return value ?? ConfigurationManager.AppSettings[key];
        }

        public IEnumerable<KeyValuePair<string,string>> EnumerateValues(string key, string environment)
        {
            if (!string.IsNullOrEmpty(environment))
                key = environment + '.' + key;

            key += '.';

            return 
                from string s in ConfigurationManager.AppSettings where s.StartsWith(key) 
                select new KeyValuePair<string, string>(s.Substring(key.Length),ConfigurationManager.AppSettings[s]);
        }


        public void SetValue(string key, string value, string environment)
        {
            throw new NotSupportedException();
        }
    }
#endif
}