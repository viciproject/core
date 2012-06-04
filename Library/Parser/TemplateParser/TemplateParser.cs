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
using System.Collections;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Vici.Core.Parser
{
	public class TemplateParser<TConfig> : TemplateParser<TConfig, CSharpParser> where  TConfig:TemplateParserConfig,new()
	{
	}

	public class TemplateParser<TConfig,TParser> : TemplateParser where TParser:ExpressionParser,new() where  TConfig:TemplateParserConfig,new()
	{
		public TemplateParser() : base(new TConfig() , new TParser())
		{
		}
	}

	public class TemplateParser
    {
		private readonly ExpressionParser _parser;
		private readonly TemplateParserConfig _config;

		public TemplateParser(TemplateParserConfig config)
		{
			_config = config;
		}

		public TemplateParser(TemplateParserConfig config, ExpressionParser parser)
		{
			_config = config;
			_parser = parser;
		}

		public ExpressionParser Parser
        {
            get { return _parser; }
        }

    	public TemplateParserConfig Config
    	{
    		get { return _config; }
    	}

#if !NETFX_CORE
        public CompiledTemplate ParseFile(string fileName)
        {
            CompiledTemplate template = Parse(CompatibilityLayer.File.ReadAllText(fileName));

            template.FileName = fileName;

            return template;
        }
#endif
    	public CompiledTemplate Parse(string inputString)
        {
            try
            {
                Stack<Node> nodeStack = new Stack<Node>();

                Node currentNode = new Node();

                CompiledTemplate compiledTemplate = new CompiledTemplate(currentNode); 

                TextNode lastTextNode = null;
                bool checkEmptyLine = false;

                TemplateToken[] tokens = Config.Tokenizer.Tokenize(inputString);

                foreach (TemplateToken token in tokens)
                {
                    if (token.TokenMatcher == null)
                    {
                        string text = token.Text;

                        if (checkEmptyLine)
                            text = CheckEmptyLine(lastTextNode, text);

                        lastTextNode = (TextNode) currentNode.Add(new TextNode(token.TokenPosition, text));

                        checkEmptyLine = false;

                        continue;
                    }

                    checkEmptyLine = token.RemoveEmptyLine && !checkEmptyLine;

                    switch (token.TokenType)
                    {
                        case TemplateTokenType.MacroCall:
                            {
                                currentNode.Add(new MacroCallNode(token));
                            }
                            break;

                        case TemplateTokenType.ParseFile:
                            {
                                currentNode.Add(new ParseFileNode(token));
                            }
                            break;

                        case TemplateTokenType.IncludeFile:
                            {
                                currentNode.Add(new IncludeFileNode(token));
                            }
                            break;

                        case TemplateTokenType.Statement:
                        case TemplateTokenType.Expression:
                            {
                                currentNode.Add(new ExpressionNode(token));
                            }
                            break;

                        case TemplateTokenType.MacroDefinition:
                            {
                                nodeStack.Push(currentNode);

                                currentNode = currentNode.Add(new MacroDefinitionNode(token));

                                string macroName = EvalMacroDefinition(token);

                                compiledTemplate.Macros[macroName] = currentNode;
                            }
                            break;

                        case TemplateTokenType.ForEach:
                            {
                                nodeStack.Push(currentNode);

                                currentNode = currentNode.Add(new ForEachNode((ForeachTemplateToken) token));
                            }
                            break;

                        case TemplateTokenType.If:
                            {
                                nodeStack.Push(currentNode);

                                IfNode ifNode = (IfNode) currentNode.Add(new IfNode(token));

                                ifNode.TrueNode = new Node();

                                nodeStack.Push(ifNode);

                                currentNode = ifNode.TrueNode;
                            }
                            break;

                        case TemplateTokenType.ElseIf:
                            {
                                IfNode ifNode = (IfNode) nodeStack.Peek();

                                currentNode = ifNode.FalseNode = new Node();

                                ifNode = (IfNode) currentNode.Add(new IfNode(token));

                                ifNode.TrueNode = new Node();

                                nodeStack.Push(ifNode);

                                currentNode = ifNode.TrueNode;
                            }
                            break;

                        case TemplateTokenType.Else:
                            {
                                IfNode ifNode = (IfNode) nodeStack.Peek();

                                currentNode = ifNode.FalseNode = new Node();
                            }
                            break;

                        case TemplateTokenType.EndBlock:
                            {
                                while (nodeStack.Peek() is IfNode)
                                {
                                    nodeStack.Pop();
                                }

                                currentNode = nodeStack.Pop();
                            }
                            break;
                    }

                }

                return compiledTemplate;
            }
            catch (Exception ex)
            {
                if (ex is IPositionedException)
                    throw new TemplateParsingException("Error parsing template: " + ex.Message, ex, ((IPositionedException)ex).Position);
                else
                    throw new TemplateParsingException("Error parsing template: " + ex.Message, ex, TokenPosition.Unknown);
            }
        }

    	private static string CheckEmptyLine(TextNode lastTextNode, string text)
    	{
    		string prevText = lastTextNode == null ? "":lastTextNode.Text;

    		Match m1 = Regex.Match(prevText, @"\n[\x20\t]*$",RegexOptions.Singleline);

    		if (m1.Success)
    		{
    			Match m2 = Regex.Match(text, @"^[\x20\t\r]*?\n", RegexOptions.Singleline);

    			if (m2.Success)
    			{
    				if (lastTextNode != null)
    					lastTextNode.Text = lastTextNode.Text.Substring(0,m1.Index+1);

    				text = text.Substring(m2.Length);
    			}
    		}
    		return text;
    	}

#if !NETFX_CORE
        public string RenderFile(string fileName, IParserContext context)
        {
            return Render(ParseFile(fileName), context);
        }
#endif

    	public string Render(CompiledTemplate compiledTemplate, IParserContext context)
        {
            try
            {
                context = context.CreateLocal();

                context.AssignmentPermissions |= AssignmentPermissions.Variable;

                StringBuilder outputBuffer = new StringBuilder();

                Dictionary<string,Node> macros = new Dictionary<string, Node>(compiledTemplate.Macros, StringComparer.OrdinalIgnoreCase);

                BuildOutput(compiledTemplate, macros, compiledTemplate.Tree, outputBuffer, context);

                return outputBuffer.ToString();
            }
            catch (Exception ex)
            {
                if (ex is IPositionedException)
                    throw new TemplateRenderingException("Error rendering template: " + ex.Message, ex, ((IPositionedException)ex).Position);
                else
                    throw new TemplateRenderingException("Error rendering template: " + ex.Message, ex, TokenPosition.Unknown);

            }
        }

        public string Render(string inputString, IParserContext context)
        {
            return Render(Parse(inputString), context);
        }

        private void BuildOutput(CompiledTemplate compiledTemplate, Dictionary<string, Node> macros, Node rootNode, StringBuilder outputBuffer, IParserContext context)
        {
            if (rootNode == null || rootNode.Children == null)
                return;

            foreach (Node node in rootNode.Children)
            {
                if (node is ForEachNode)
                {
                    ForEachNode forEachNode = (ForEachNode)node;

                	IEnumerable list = EvalForeach(forEachNode.TemplateToken, context);

                    if (list != null)
                    {
                        IParserContext localContext = context.CreateLocal();

                        int rowNum = 1;

                        foreach (object listItem in list)
                        {
                            localContext.Set(forEachNode.Iterator, listItem, listItem == null ? typeof(object) : listItem.GetType());

							EvalIteration(forEachNode.Iterator, rowNum, listItem, localContext);

                            BuildOutput(compiledTemplate, macros, forEachNode, outputBuffer, localContext);

                            rowNum++;
                        }
                    }
                }

                else if (node is IfNode)
                {
                    IfNode ifNode = (IfNode)node;

                	bool result = EvalIf(ifNode.TemplateToken, context);

                    BuildOutput(compiledTemplate, macros, result ? ifNode.TrueNode : ifNode.FalseNode, outputBuffer, context);
                }

                else if (node is ExpressionNode)
                {
                    ExpressionNode exprNode = (ExpressionNode)node;

                	string value = EvalExpression(exprNode.TemplateToken, context);

                    if (value != null)
                        outputBuffer.Append(value);
                }

                else if (node is ParseFileNode)
                {
                    ParseFileNode parseFileNode = (ParseFileNode) node;

                    Dictionary<string, IValueWithType> parameters;

                    CompiledTemplate template = EvalParseFile(this, compiledTemplate.FileName, parseFileNode.TemplateToken, context, out parameters);

                    if (template != null)
                    {
                        foreach (KeyValuePair<string, Node> macro in template.Macros)
                            macros[macro.Key] = macro.Value;

                        IParserContext localContext = context.CreateLocal();

                        foreach (KeyValuePair<string, IValueWithType> var in parameters)
                            localContext.Set(var.Key, var.Value.Value, var.Value.Type);

                        BuildOutput(compiledTemplate, macros, template.Tree, outputBuffer, localContext);
                    }
                }

                else if (node is IncludeFileNode)
                {
                    IncludeFileNode includeNode = (IncludeFileNode) node;

                    string value = EvalIncludeFile(compiledTemplate.FileName, includeNode.TemplateToken, context);

                    if (value != null)
                        outputBuffer.Append(value);
                }

                else if (node is MacroCallNode)
                {
                    MacroCallNode macroCallNode = (MacroCallNode) node;

                    Dictionary<string, IValueWithType> parameters;

                    string macroName = EvalMacroCall(macroCallNode.TemplateToken, context, out parameters);


                    Node macro;
                    
                    if (!macros.TryGetValue(macroName, out macro))
                        throw new TemplateRenderingException("Unknown macro " + macroName, macroCallNode.TemplateToken.TokenPosition);

                    IParserContext localContext = context.CreateLocal();

                    foreach (KeyValuePair<string, IValueWithType> var in parameters)
                    {
                        localContext.Set(var.Key, var.Value.Value, var.Value.Type);
                    }

                    BuildOutput(compiledTemplate, macros, macro, outputBuffer, localContext);
                }

                else if (node is TextNode)
                {
                    outputBuffer.Append(EvalText(((TextNode)node).Text));
                }
            }

        }

        private string EvalExpression(TemplateToken templateToken, IParserContext context)
		{
			string returnValue = OnEvalExpression(templateToken, context);

            if (templateToken.TokenType != TemplateTokenType.Statement)
    			return returnValue;

            return "";
		}

		protected virtual string OnEvalExpression(TemplateToken templateToken, IParserContext context)
		{
		    return Config.EvalExpression(Parser, templateToken, context);
		}

        private CompiledTemplate EvalParseFile(TemplateParser templateParser, string fileName, TemplateToken templateToken, IParserContext context, out Dictionary<string, IValueWithType> parameters)
        {
            return OnEvalParseFile(templateParser, fileName, templateToken, context, out parameters);
        }

        protected virtual CompiledTemplate OnEvalParseFile(TemplateParser templateParser, string fileName, TemplateToken templateToken, IParserContext context, out Dictionary<string, IValueWithType> parameters)
        {
            return Config.EvalParseFile(Parser, templateParser, fileName, templateToken, context, out parameters);
        }

        private string EvalIncludeFile(string fileName, TemplateToken templateToken, IParserContext context)
        {
            return OnEvalIncludeFile(fileName, templateToken, context);
        }

        protected virtual string OnEvalIncludeFile(string fileName, TemplateToken templateToken, IParserContext context)
        {
            return Config.EvalIncludeFile(Parser, fileName, templateToken, context);
        }

        private string EvalMacroDefinition(TemplateToken templateToken)
        {
            return OnEvalMacroDefinition(templateToken);
        }

        protected virtual string OnEvalMacroDefinition(TemplateToken templateToken)
        {
            return Config.EvalMacroDefinition(Parser, templateToken);
        }


        private string EvalMacroCall(TemplateToken templateToken, IParserContext context, out Dictionary<string,IValueWithType> parameters)
        {
            return OnEvalMacroCall(templateToken, context, out parameters);
        }

        protected virtual string OnEvalMacroCall(TemplateToken templateToken, IParserContext context, out Dictionary<string, IValueWithType> parameters)
        {
            return Config.EvalMacroCall(Parser, templateToken, context, out parameters);
        }

        private bool EvalIf(TemplateToken templateToken, IParserContext context)
		{
            return OnEvalIf(templateToken, context);
		}

		protected virtual bool OnEvalIf(TemplateToken templateToken, IParserContext context)
		{
		    return Config.EvalIf(Parser, templateToken, context);
		}

		private IEnumerable EvalForeach(ForeachTemplateToken templateToken, IParserContext context)
		{
            return OnEvalForeach(templateToken, context);
		}

		protected virtual IEnumerable OnEvalForeach(ForeachTemplateToken templateToken, IParserContext context)
		{
		    return Config.EvalForeach(Parser, templateToken, context);
		}

        private void EvalIteration(string iteratorName, int rowNum, object obj, IParserContext localContext)
        {
            OnEvalIteration(iteratorName, rowNum, obj, localContext);
        }

		protected virtual void OnEvalIteration(string iteratorName, int rowNum, object obj, IParserContext localContext)
		{
		    Config.EvalIteration(iteratorName, rowNum, obj, localContext);
		}

		private string EvalText(string text)
		{
		    return OnEvalText(text);
		}

		protected virtual string OnEvalText(string text)
		{
		    return Config.EvalText(text);
		}
    }
}
