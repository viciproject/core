using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vici.Core.Parser;

namespace Vici.Core.Test
{
    [TestClass]
    public class ScriptParser_Fixture
    {
        readonly CSharpContext context = new CSharpContext();

        [TestInitialize]
        public void Setup()
        {
            context.Set("doubleValue", 20.5);
            context.Set("intList", new int[] { 3, 4, 5 });
            context.Set("intVar", 5);
        }
        /*
        [TestMethod]
        public void SmartExpression1()
        {
            ScriptParser parser = new ScriptParser();

            string script = @"

            foreach (i in [1...3])
                out(i.ToString());
            }
";

            ScriptResult scriptResult = parser.Execute(script, context);

            Assert.AreEqual("x", scriptResult.OutputText);
        }
*/
    }
}