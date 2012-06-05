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
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Vici.Core.Config;

namespace Vici.Core.Test
{
    [TestClass]
    public class ConfigTest
    {
        class StaticConfig
        {
            public static string Test1 { get; set; }
            public static ConfigurableClass1 SubObject1;
            public static Dictionary<string, string> StringValues;
            public static Dictionary<string, int> IntValues;
        }

        [ConfigKey("Cfg")]
        class StaticConfig2
        {
            public static string Test1;
            public static ConfigurableClass1 SubObject1;
        }

        class InstanceConfig
        {
            public string Test1;
            public ConfigurableClass1 SubObject1;
            public Dictionary<string, string> StringValues;
            public Dictionary<string, int> IntValues;
        }

        class ConfigurableClass1 : IConfigObject
        {
            public int TestInt1;
        }

        class InstanceXmlConfig
        {
            public int Prop1;
            public int Prop2;
            public InstanceXmlSubConfig SubGroupProp1;
            public Dictionary<string, string> StringValues;
            public Dictionary<string, int> IntValues;
        }

        class InstanceXmlSubConfig : IConfigObject
        {
            public string SubProp2;
            public string SubProp3 = "test";
            public string SubProp4 = "test";
        }


        [TestMethod]
        public void TestStaticClassWithoutKey()
        {
            InstanceConfig config = new InstanceConfig();

            ConfigManager configManager = new ConfigManager();

            configManager.Register(config);
            configManager.Register<StaticConfig>();
            configManager.Register<StaticConfig2>();

            configManager.RegisterProvider(new ConfigurationProviderAppConfig());
            configManager.Update();

            Assert.AreEqual("xx1",StaticConfig.Test1);
            Assert.AreEqual(99, StaticConfig.SubObject1.TestInt1);

            Assert.AreEqual("xx2", StaticConfig2.Test1);
            Assert.AreEqual(88, StaticConfig2.SubObject1.TestInt1);

            Assert.AreEqual("xx1", config.Test1);
            Assert.AreEqual(99, config.SubObject1.TestInt1);

            Assert.IsNotNull(StaticConfig.IntValues);
            Assert.IsNotNull(StaticConfig.StringValues);

            Assert.AreEqual("x", StaticConfig.StringValues["ValX"]);
            Assert.AreEqual("y", StaticConfig.StringValues["ValY"]);
            Assert.AreEqual("z", StaticConfig.StringValues["ValZ"]);
            
            Assert.AreEqual(3, StaticConfig.StringValues.Count);

            Assert.AreEqual(1, StaticConfig.IntValues["Val1"]);
            Assert.AreEqual(2, StaticConfig.IntValues["Val2"]);
            Assert.AreEqual(3, StaticConfig.IntValues["Val3"]);

            Assert.AreEqual(3, StaticConfig.IntValues.Count);
        }

        [TestMethod]
        public void TestStaticClassWithKey()
        {
            ConfigManager configManager = new ConfigManager();

            configManager.Register<StaticConfig2>();
            configManager.RegisterProvider(new ConfigurationProviderAppConfig());
            configManager.Update();

            Assert.AreEqual("xx2", StaticConfig2.Test1);
            Assert.AreEqual(88, StaticConfig2.SubObject1.TestInt1);

    }

        [TestMethod]
        public void TestInstanceClassWithoutKey()
        {
            InstanceConfig config = new InstanceConfig();

            ConfigManager configManager = new ConfigManager();

            configManager.Register(config);
            configManager.RegisterProvider(new ConfigurationProviderAppConfig());
            configManager.Update();

            Assert.AreEqual("xx1", config.Test1);
            Assert.AreEqual(99, config.SubObject1.TestInt1);

            Assert.IsNotNull(config.IntValues);
            Assert.IsNotNull(config.StringValues);

            Assert.AreEqual("x", config.StringValues["ValX"]);
            Assert.AreEqual("y", config.StringValues["ValY"]);
            Assert.AreEqual("z", config.StringValues["ValZ"]);

            Assert.AreEqual(3, config.StringValues.Count);

            Assert.AreEqual(1, config.IntValues["Val1"]);
            Assert.AreEqual(2, config.IntValues["Val2"]);
            Assert.AreEqual(3, config.IntValues["Val3"]);

            Assert.AreEqual(3, config.IntValues.Count);

        }

        [TestMethod]
        public void TestXmlConfigFile()
        {
            InstanceXmlConfig config = new InstanceXmlConfig();

            ConfigManager configManager = new ConfigManager();

            configManager.Register(config);
            configManager.RegisterProvider(new ConfigurationProviderXmlConfig(Environment.CurrentDirectory + "\\Config.xml"));
            configManager.Update();

            Assert.AreEqual(21, config.Prop1);
            Assert.AreEqual(0, config.Prop2);
            Assert.AreEqual("22", config.SubGroupProp1.SubProp2);
            Assert.AreEqual("test", config.SubGroupProp1.SubProp3);
        }

        [TestMethod]
        public void TestDefaults()
        {
            InstanceXmlConfig config = new InstanceXmlConfig();

            ConfigManager configManager = new ConfigManager();

            configManager.Register(config);
            configManager.RegisterProvider(new ConfigurationProviderXmlConfig(Environment.CurrentDirectory + "\\Config.xml"));
            configManager.Update();
           
            Assert.AreEqual("test", config.SubGroupProp1.SubProp3);
            Assert.AreEqual("yeah", config.SubGroupProp1.SubProp4);
        }

    }
}