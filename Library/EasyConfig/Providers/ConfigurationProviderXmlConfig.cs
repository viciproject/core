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
using System.IO;
using System.Linq;
using System.Xml.Linq;
#if !WINDOWS_PHONE && !NETFX_CORE
using System.Xml.XPath;
#endif

namespace Vici.Core.Config
{
    //TODO: change implementation for WP7
#if !WINDOWS_PHONE && !NETFX_CORE
    public class ConfigurationProviderXmlConfig : IConfigurationProvider
    {
        private XDocument _xDoc;
        private readonly string _filePath;

        public ConfigurationProviderXmlConfig(string filePath)
        {
            

            if (!File.Exists(filePath))
                throw new FileNotFoundException(string.Format("Cofiguration file {0} could not be found",filePath));

            _filePath = filePath;
        }

        public long Version()
        {
            return new FileInfo(_filePath).LastWriteTimeUtc.Ticks;
        }

        public bool CanSave
        {
            get { return false; }
        }

        public string GetValue(string key, string environment)
        {
            if (_xDoc == null)
                ReloadConfigFile();

            var xElement = _xDoc.XPathSelectElement("//" + key.Replace('.', '/'));

            return xElement != null ? xElement.Value : null;
        }

        public IEnumerable<KeyValuePair<string, string>> EnumerateValues(string key, string environment)
        {
            if (_xDoc == null)
                ReloadConfigFile();

            var xElement = _xDoc.XPathSelectElement("//" + key.Replace('.', '/'));

            if (xElement == null)
                return Enumerable.Empty<KeyValuePair<string,string>>();

            return xElement.Elements().Select(e => new KeyValuePair<string, string>(e.Name.ToString(),e.Value));
        }

        public void SetValue(string key, string value, string environment)
        {
            throw new NotSupportedException();
        }

        private void ReloadConfigFile()
        {
            try
            {
                _xDoc = XDocument.Load(_filePath);
            }
            catch (Exception err)
            {
                throw new FormatException("The XML of the configuration file could not be loaded.", err);
            }
        }
    }
#endif
}