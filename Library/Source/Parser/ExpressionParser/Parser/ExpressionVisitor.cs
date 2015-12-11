using System;
using System.Runtime.InteropServices;

namespace Vici.Core.Parser
{
    public abstract class ExpressionVisitor
    {
        protected virtual Expression Visit(Expression expression)
        {
            if (expression == null)
                return null;

            if (expression is BinaryExpression)
                return VisitBinary((BinaryExpression) expression);

            if (expression is UnaryExpression)
                return VisitUnary((UnaryExpression) expression);

            if (expression is CallExpression)
                return VisitCall((CallExpression) expression);

            if (expression is ConditionalExpression)
                return VisitConditional((ConditionalExpression) expression);

            if (expression is ConstructorExpression)
                return VisitConstructor((ConstructorExpression) expression);

            if (expression is FieldExpression)
                return VisitField((FieldExpression) expression);

            if (expression is IndexExpression)
                return VisitIndex((IndexExpression) expression);

            if (expression is SequenceExpression)
                return VisitSequence((SequenceExpression) expression);

            if (expression is ValueExpression)
                return VisitValue((ValueExpression) expression);

            if (expression is VariableExpression)
                return VisitVariable((VariableExpression) expression);

            return VisitUnknown(expression);
        }

        protected virtual Expression VisitUnknown(Expression expression)
        {
            throw new Exception(expression + " not supported by ExpressionVisitor");
        }

        protected virtual Expression VisitBinary(BinaryExpression expression)
        {
            Visit(expression.Left);
            Visit(expression.Right);

            return expression;
        }

        protected virtual Expression VisitUnary(UnaryExpression expression)
        {
            Visit(expression.Value);

            return expression;
        }

        protected virtual Expression VisitCall(CallExpression expression)
        {
            Visit(expression.MethodExpression);

            foreach (var parameter in expression.Parameters)
                Visit(parameter);

            return expression;
        }

        protected virtual Expression VisitConditional(ConditionalExpression expression)
        {
            Visit(expression.Condition);
            Visit(expression.TrueValue);
            Visit(expression.FalseValue);

            return expression;
        }

        protected virtual Expression VisitConstructor(ConstructorExpression expression)
        {
            Visit(expression.ClassName);

            foreach (var parameter in expression.Parameters)
                Visit(parameter);

            return expression;
        }

        protected virtual Expression VisitField(FieldExpression expression)
        {
            Visit(expression.Target);

            return expression;
        }

        protected virtual Expression VisitIndex(IndexExpression expression)
        {
            Visit(expression.Target);

            foreach (var parameter in expression.Parameters)
                Visit(parameter);

            return expression;
        }

        protected virtual Expression VisitSequence(SequenceExpression expression)
        {
            foreach (var subExpression in expression.Expressions)
                Visit(subExpression);

            return expression;
        }

        protected virtual Expression VisitValue(ValueExpression expression)
        {
            return expression;
        }

        protected virtual Expression VisitVariable(VariableExpression expression)
        {
            return expression;
        }
    }
}