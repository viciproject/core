namespace Vici.Core.Parser
{
    public abstract class UnaryExpression : Expression
    {
        public Expression Value { get; private set; }

        protected UnaryExpression(TokenPosition position, Expression value) : base(position)
        {
            Value = value;
        }
    }
}