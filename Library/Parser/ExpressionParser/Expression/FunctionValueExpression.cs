using System;

namespace Vici.Core.Parser
{
    public class FunctionValueExpression : ValueExpression
    {
        public FunctionValueExpression(TokenPosition position, FunctionDefinitionExpression function) : base(position, function, typeof(FunctionDefinitionExpression))
        {
        }
    }
}