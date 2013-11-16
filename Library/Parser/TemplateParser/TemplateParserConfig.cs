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
using System.IO;

namespace Vici.Core.Parser
{
    public abstract class TemplateParserConfig<T> : TemplateParserConfig where T:TemplateTokenizer,new()
    {
        private readonly T _tokenizer = new T();

        public override Tokenizer<TemplateToken> Tokenizer
        {
            get { return _tokenizer; }
        }
    }

    public abstract class TemplateParserConfig
    {
        public abstract Tokenizer<TemplateToken> Tokenizer { get; }

        protected virtual bool OnEvalIf(ExpressionParser parser, TemplateToken templateToken, IParserContext context)
        {
            return context.ToBoolean(parser.Evaluate(templateToken.Text, context, templateToken.TokenPosition).Value);
        }

        protected virtual string OnEvalExpression(ExpressionParser parser, TemplateToken templateToken, IParserContext context)
        {
            int quoteIdx = templateToken.Text.LastIndexOf('`');

            string expr = quoteIdx >= 0 ? templateToken.Text.Substring(0, quoteIdx).Trim() : templateToken.Text;

            object value = parser.Evaluate(expr, context, templateToken.TokenPosition).Value;

            if (value == null)
                return null;

            if (templateToken.TokenType == TemplateTokenType.Expression && quoteIdx >= 0)
                return context.Format("{0:" + templateToken.Text.Substring(quoteIdx+1).Trim() + "}", value);

            if (value is string)
                return (string) value;

            return value.ToString();
        }

        protected virtual string OnEvalMacroDefinition(ExpressionParser parser, TemplateToken templateToken)
        {
            return templateToken.Text;
        }

        protected virtual string OnEvalMacroCall(ExpressionParser parser, TemplateToken templateToken, IParserContext context, out Dictionary<string, IValueWithType> parameters)
        {
            TemplateToken.ParameterizedExpression pExpr = templateToken.ExtractParameters();

            parameters = new Dictionary<string, IValueWithType>(StringComparer.OrdinalIgnoreCase);

            foreach (KeyValuePair<string, string> var in pExpr.Parameters)
            {
                parameters[var.Key] = parser.Evaluate(var.Value, context);
            }

            return pExpr.MainExpression;
        }

        protected virtual IEnumerable OnEvalForeach(ExpressionParser parser, ForeachTemplateToken templateToken, IParserContext context)
        {
            return parser.Evaluate(templateToken.Expression, context, templateToken.TokenPosition).Value as IEnumerable;
        }

        protected virtual string OnEvalText(string text)
        {
            return text;
        }

        protected virtual void OnEvalIteration(string iteratorName, int rowNum, object obj, IParserContext localContext)
        {
            localContext.SetLocal(iteratorName + "@row", rowNum);
            localContext.SetLocal(iteratorName + "@index", rowNum-1);
            localContext.SetLocal(iteratorName + "@odd", rowNum % 2 == 1);
            localContext.SetLocal(iteratorName + "@even", rowNum % 2 == 0);
            localContext.SetLocal(iteratorName + "@oddeven", rowNum % 2 == 0 ? "even" : "odd");
            localContext.SetLocal(iteratorName + "@ODDEVEN", rowNum % 2 == 0 ? "EVEN" : "ODD");
            localContext.SetLocal(iteratorName + "@OddEven", rowNum % 2 == 0 ? "Even" : "Odd");
        }

        internal IEnumerable EvalForeach(ExpressionParser parser, ForeachTemplateToken templateToken, IParserContext context)
        {
            return OnEvalForeach(parser, templateToken, context);
        }

        internal bool EvalIf(ExpressionParser parser, TemplateToken templateToken, IParserContext context)
        {
            return OnEvalIf(parser, templateToken, context);
        }

        internal string EvalExpression(ExpressionParser parser, TemplateToken templateToken, IParserContext context)
        {
            return OnEvalExpression(parser, templateToken, context);
        }

        internal string EvalMacroCall(ExpressionParser parser, TemplateToken templateToken, IParserContext context, out Dictionary<string, IValueWithType> parameters)
        {
            return OnEvalMacroCall(parser, templateToken, context, out parameters);
        }

        internal string EvalMacroDefinition(ExpressionParser parser, TemplateToken templateToken)
        {
            return OnEvalMacroDefinition(parser, templateToken);
        }

        internal string EvalText(string text)
        {
            return OnEvalText(text);
        }

        internal void EvalIteration(string iteratorName, int rowNum, object obj, IParserContext localContext)
        {
            OnEvalIteration(iteratorName, rowNum, obj, localContext);
        }

        internal CompiledTemplate EvalParseFile(ExpressionParser parser, TemplateParser templateParser, string fileName, TemplateToken token, IParserContext context, out Dictionary<string, IValueWithType> parameters)
        {
            return OnEvalParseFile(parser, templateParser, fileName, token, context, out parameters);
        }

        protected virtual CompiledTemplate OnEvalParseFile(ExpressionParser parser, TemplateParser templateParser, string fileName, TemplateToken token, IParserContext context, out Dictionary<string, IValueWithType> parameters)
        {
            TemplateToken.ParameterizedExpression pExpr = token.ExtractParameters();

            parameters = new Dictionary<string, IValueWithType>(StringComparer.OrdinalIgnoreCase);

            foreach (KeyValuePair<string, string> var in pExpr.Parameters)
            {
                parameters[var.Key] = parser.Evaluate(var.Value, context);
            }

#if NETFX_CORE || PCL
            return null; // TODO: use isolated storage
#else
            string includeFile = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(pExpr.MainExpression), fileName));

            return templateParser.ParseFile(includeFile);
#endif
        }

        internal string EvalIncludeFile(ExpressionParser parser, string fileName, TemplateToken token, IParserContext context)
        {
            return OnEvalIncludeFile(parser, fileName, token, context);
        }

        protected virtual string OnEvalIncludeFile(ExpressionParser parser, string fileName, TemplateToken token, IParserContext context)
        {
#if NETFX_CORE || PCL
            return ""; // TODO: use isolated storage
#else
            string includeFile = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(fileName), fileName));

            return CompatibilityLayer.File.ReadAllText(includeFile);
#endif
        }
    }
}