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

namespace Vici.Core.Parser
{
    internal class RPNExpression
	{
		private class TokenQueue : Queue<ExpressionToken>
		{
			private ExpressionToken _waiting;

			public new void Enqueue(ExpressionToken token)
			{
				if (token.IsPartial)
				{
					if (_waiting != null)
					{
						if (_waiting.Root == token.Root)
							base.Enqueue(new ExpressionToken(token.Root,_waiting.Text+token.Text));
						else 
							throw new LexerException("Mismatched ternary operators", token.TokenPosition, token.Text);

						_waiting = null;
					}
					else
					{
						_waiting = token;
					}

					return;
				}

				base.Enqueue(token);
			}
		}

		private readonly Stack<ExpressionToken> _operatorStack = new Stack<ExpressionToken>();
		private readonly TokenQueue _tokenQueue = new TokenQueue();
		private readonly Stack<ExpressionToken> _functionStack = new Stack<ExpressionToken>();
        private bool _lastWasFunction;
    	private bool _lastWasOperator = true;

        private ExpressionToken[] _tokenList;

    	private readonly TokenEvaluator _functionEvaluator;

    	public RPNExpression(TokenEvaluator functionEvaluator)
    	{
    		_functionEvaluator = functionEvaluator;
    	}

    	internal void ApplyToken(ExpressionToken token)
		{
            if (token.IsRightParen && _lastWasFunction)
                _functionStack.Peek().NumTerms = 0;

            _lastWasFunction = false;

            if (token.IsFunction)
            {
                _functionStack.Push(token);

            	_lastWasFunction = true;

                token.NumTerms = 1;
            }

			if (token.IsLeftParen)
			{
				if (!_lastWasOperator)
				{
					ExpressionToken callToken = new FunctionCallToken(token.Text, _functionEvaluator, token.TokenPosition);

					ApplyToken(callToken);
				}

				_operatorStack.Push(token);

				_lastWasOperator = true;

				return;
			}

			if (token.IsTerm)
			{
				_tokenQueue.Enqueue(token);

				_lastWasOperator = false;

				return;
			}

            if (token.IsArgumentSeparator)
            {
                while (_operatorStack.Count > 0 && !_operatorStack.Peek().IsLeftParen)
                {
                    _tokenQueue.Enqueue(_operatorStack.Pop());
                }

                _functionStack.Peek().NumTerms++;

            	_lastWasOperator = true;

                return;
            }

			if (token.IsRightParen)
			{
				while (_operatorStack.Count > 0)
				{
                    ExpressionToken stackOperator = _operatorStack.Pop();

					if (stackOperator.IsLeftParen)
					{
						if (_operatorStack.Count > 0 && _operatorStack.Peek().IsFunction)
						{
							_tokenQueue.Enqueue(_operatorStack.Pop());

						    _functionStack.Pop();
						}

						break;
					}

					_tokenQueue.Enqueue(stackOperator);
				}

				_lastWasOperator = false;

				return;
			}

            if (_lastWasOperator != token.IsUnary)
            {
                if (token.Alternate != null)
                {
                    ApplyToken(token.Alternate);
                    return;
                }

                throw new LexerException("Misplaced operator " + token.Text, token.TokenPosition, token.Text);
            }

			// When we get here, it certainly is an operator or function call

			while (_operatorStack.Count > 0)
			{
                ExpressionToken stackOperator = _operatorStack.Peek();

				if (   (token.Associativity == OperatorAssociativity.Right && token.Precedence < stackOperator.Precedence)
					|| (token.Associativity == OperatorAssociativity.Left && token.Precedence <= stackOperator.Precedence))
					_tokenQueue.Enqueue(_operatorStack.Pop());
				else
					break;
			}

			_operatorStack.Push(token);

        	_lastWasOperator = true;
		}

		internal void Start()
		{
			_operatorStack.Clear();
			_tokenQueue.Clear();
            _functionStack.Clear();

		    _lastWasFunction = false;
		}

		internal void Finish()
		{
			while (_operatorStack.Count > 0)
				_tokenQueue.Enqueue(_operatorStack.Pop());

            _tokenList = _tokenQueue.ToArray();

			_tokenQueue.Clear();
		}

        public Expression Compile()
        {
            Stack<Expression> resultStack = new Stack<Expression>();

            Expression currentExpression = null;

            foreach (ExpressionToken token in _tokenList)
            {
                TokenEvaluator evaluator = token.Evaluator;

                if (token.IsTerm)
                {
                    currentExpression = evaluator(token.Text, token.TokenPosition, null);

                    resultStack.Push(currentExpression);
                }
                else
                {
                    int numTerms = token.NumTerms;

					if (token.IsFunction)
						numTerms++; // include the function name as the first parameter

                    Expression[] parameters = new Expression[numTerms];

                    for (int i = numTerms-1; i >= 0; i--)
                        parameters[i] = resultStack.Pop();

                    currentExpression = evaluator(token.Text, token.TokenPosition, parameters);

                    resultStack.Push(currentExpression);
                }
            }

            return currentExpression;
        }
	}
}
