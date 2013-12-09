using System;
using System.Linq;

namespace Vici.Core.Parser
{
    public class DefaultValueExpression : Expression
    {
        private readonly Expression _value;
        private readonly Expression _defaultValue;

        public DefaultValueExpression(TokenPosition position, Expression value, Expression defaultValue)
            : base(position)
        {
            _value = value;
            _defaultValue = defaultValue;
        }

        public override ValueExpression Evaluate(IParserContext context)
        {
            ValueExpression result = _value.Evaluate(context);


            if (context.ToBoolean(result.Value))
                return result;
            else
                return _defaultValue.Evaluate(context);
        }

        public override string ToString()
        {
            return "(" + _value + " ?: " + _defaultValue + ")";
        }
    }
}