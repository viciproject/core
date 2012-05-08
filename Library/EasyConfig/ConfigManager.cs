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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Vici.Core.Config
{
    public class ConfigManager
    {
        private string _environment;
        private readonly List<Type> _configTypes = new List<Type>();
        private readonly List<object> _configObjects = new List<object>();
        private List<int> _configVersions = new List<int>();
        private readonly List<IConfigurationProvider> _configProviders = new List<IConfigurationProvider>();

        public static ConfigManager Default { get; private set; }

        static ConfigManager()
        {
            Default = new ConfigManager();
        }

        public void Register<T>()
        {
            _configTypes.Add(typeof (T));
        }

        public void Register(Type type)
        {
            _configTypes.Add(type);
        }

        public void Register(object obj)
        {
            _configObjects.Add(obj);
        }

        public void RegisterProvider(IConfigurationProvider provider)
        {
            _configProviders.Add(provider);
            _configVersions.Add(0);
        }

        public string Environment
        {
            get
            {
                return _environment;
            }
            set
            {
                _environment = value;

                // easy way to set all entries to 0
                _configVersions = new List<int>(_configProviders.Count);
                
                Update();
            }
        }

        public bool Update()
        {
            bool updated = false;

            for (int i = 0; i < _configProviders.Count; i++)
            {
                updated |= (_configProviders[i].Version() != _configVersions[i]);
            }

            if (updated)
            {
                _configTypes.ForEach(Fill);
                _configObjects.ForEach(Fill);
            }

            return updated;
        }

        private void Fill(Type type)
        {
            ConfigKeyAttribute[] attributes = (ConfigKeyAttribute[]) type.GetCustomAttributes(typeof (ConfigKeyAttribute), false);
            
            Fill(type, null, attributes.Length > 0 ? attributes[0].BaseKey : null);
        }

        private void Fill(object obj)
        {
            ConfigKeyAttribute[] attributes = (ConfigKeyAttribute[]) obj.GetType().GetCustomAttributes(typeof (ConfigKeyAttribute), false);
            
            Fill(null, obj, attributes.Length > 0 ? attributes[0].BaseKey : null);
        }

        private void Fill(Type type, object obj, string baseKey)
        {
            if (type == null && obj == null)
                throw new ArgumentNullException();

            if (!string.IsNullOrEmpty(baseKey))
                baseKey = baseKey + '.';
            else
                baseKey = "";
            
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.DeclaredOnly;
            
            if (type == null)
            {
                type = obj.GetType();
                bindingFlags |= BindingFlags.Instance;
            }
            else
            {
                bindingFlags |= BindingFlags.Static;
            }

            FieldOrPropertyInfo[] members = type.GetFieldsAndProperties(bindingFlags);
            
            foreach (FieldOrPropertyInfo field in members)
            {
                ConfigKeyAttribute[] attributes = (ConfigKeyAttribute[]) field.GetCustomAttributes(typeof (ConfigKeyAttribute), false);

                string key = field.Name;

                if (attributes.Length > 0 && !string.IsNullOrEmpty(attributes[0].BaseKey))
                    key = attributes[0].BaseKey;

                Type fieldType = field.FieldType;

                bool follow = (attributes.Length > 0 && attributes[0] is ConfigObjectAttribute) || typeof (IConfigObject).IsAssignableFrom(fieldType);
                bool ignore = field.IsDefined(typeof (ConfigIgnoreAttribute), false);
                
                key = baseKey + key;
                
                if (ignore)
                    continue;

                if (follow)
                {

                    object configObject = field.GetValue(obj);

                    if (configObject == null)
                    {
                        configObject = Activator.CreateInstance(fieldType);

                        field.SetValue(obj, configObject);
                    }

                    Fill(null, configObject, key);
                }
                else
                {
                    if (typeof(IDictionary).IsAssignableFrom(field.FieldType))
                    {
                        Type dicInterface = fieldType.GetInterfaces().Where(i => i.GetGenericTypeDefinition() == typeof(IDictionary<,>)).FirstOrDefault();

                        Type targetType = typeof (object);

                        if (dicInterface != null)
                        {
                            targetType = dicInterface.GetGenericArguments()[1];
                        }

                        object configObject = field.GetValue(obj);

                        if (configObject == null)
                        {
                            configObject = Activator.CreateInstance(field.FieldType);

                            field.SetValue(obj, configObject);
                        }

                        IDictionary dic = (IDictionary) configObject;

                        foreach (var item in GetValues(key))
                        {
                            dic[item.Key] = item.Value.To(targetType);
                        }
                    }
                    else
                    {
                        object value = GetValue(key, field.FieldType);

                        if (value != null)
                            field.SetValue(obj, value);
                    }
                }
            }
        }

        public object GetValue(string key, Type type)
        {
            foreach (IConfigurationProvider provider in _configProviders)
            {
                string value = provider.GetValue(key, _environment);

                if (value != null)
                    return value.To(type);
            }

            return null;
        }

        public IEnumerable<KeyValuePair<string,string>> GetValues(string key)
        {
            return _configProviders.SelectMany(provider => provider.EnumerateValues(key, _environment));
        }
    }
}