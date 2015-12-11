namespace Vici.Core.Parser
{
    public class ValueOrNullExpression : BinaryExpression
    {
        public ValueOrNullExpression(TokenPosition position, Expression condition, Expression value) : base(position, condition, value)
        {
        }

        public Expression Condition
        {
            get { return Left; }
        }

        public Expression Value
        {
            get { return Right; }
        }

        public override ValueExpression Evaluate(IParserContext context)
        {
            ValueExpression result = Condition.Evaluate(context);

            if (context.ToBoolean(result.Value))
                return Value.Evaluate(context);
            else
                return new ValueExpression(TokenPosition, null, typeof(object));
        }

#if DEBUG
        public override string ToString()
        {
            return "(" + Condition + " ?: " + Value + ")";
        }
#endif
    }
}