using System;

namespace Vici.Core.Parser
{
    public static class Exp
    {
        public static AddExpression Add(TokenPosition position, Expression left, Expression right) { return new AddExpression(position, left, right); }
        public static SubtractExpression Subtract(TokenPosition position, Expression left, Expression right) { return new SubtractExpression(position, left, right); }
        public static MultiplyExpression Multiply(TokenPosition position, Expression left, Expression right) { return new MultiplyExpression(position, left, right); }
        public static DivideExpression Divide(TokenPosition position, Expression left, Expression right) { return new DivideExpression(position, left, right); }
        public static ValueExpression<T> Value<T>(TokenPosition position, T value) { return new ValueExpression<T>(position, value); }
        public static ValueExpression Value(TokenPosition position, object value, Type type) { return new ValueExpression(position, value, type); }
        public static ReturnValueExpression ReturnValue(TokenPosition position, object value, Type type) { return new ReturnValueExpression(position, value, type); }
        public static BinaryArithmicExpression Op(TokenPosition position, string op, Expression left, Expression right) { return new BinaryArithmicExpression(position, op, left, right); }
        public static AndAlsoExpression AndAlso(TokenPosition position, Expression left, Expression right) { return new AndAlsoExpression(position, left, right); }
        public static OrElseExpression OrElse(TokenPosition position, Expression left, Expression right) { return new OrElseExpression(position, left, right); }
        public static ValueExpression NullValue(TokenPosition position) { return Value(position, null, typeof(object)); }
        public static BinaryArithmicExpression Equal(TokenPosition position, Expression left, Expression right) {  return new BinaryArithmicExpression(position,"==",left,right); }
        public static FieldExpression Field(TokenPosition position, Expression target, string fieldName) {  return new FieldExpression(position,target,fieldName); }

        public static AddExpression Add(Expression left, Expression right) { return new AddExpression(TokenPosition.Unknown, left, right); }
        public static SubtractExpression Subtract(Expression left, Expression right) { return new SubtractExpression(TokenPosition.Unknown, left, right); }
        public static MultiplyExpression Multiply(Expression left, Expression right) { return new MultiplyExpression(TokenPosition.Unknown, left, right); }
        public static DivideExpression Divide(Expression left, Expression right) { return new DivideExpression(TokenPosition.Unknown, left, right); }
        public static ValueExpression<T> Value<T>(T value) { return new ValueExpression<T>(TokenPosition.Unknown, value); }
        public static ValueExpression Value(object value, Type type) { return new ValueExpression(TokenPosition.Unknown, value, type); }
        public static ReturnValueExpression ReturnValue(object value, Type type) { return new ReturnValueExpression(TokenPosition.Unknown, value, type); }
        public static BinaryArithmicExpression Op(string op, Expression left, Expression right) { return new BinaryArithmicExpression(TokenPosition.Unknown, op, left, right); }
        public static AndAlsoExpression AndAlso(Expression left, Expression right) { return new AndAlsoExpression(TokenPosition.Unknown, left, right); }
        public static OrElseExpression OrElse(Expression left, Expression right) { return new OrElseExpression(TokenPosition.Unknown, left, right); }
        public static ValueExpression NullValue() { return Value(TokenPosition.Unknown, null, typeof(object)); }
        public static BinaryArithmicExpression Equal(Expression left, Expression right) { return new BinaryArithmicExpression(TokenPosition.Unknown, "==", left, right); }
        public static FieldExpression Field(Expression target, string fieldName) { return new FieldExpression(TokenPosition.Unknown, target, fieldName); }

    }
}