using System.Linq.Expressions;

namespace Montreal.Core.Crosscutting.Common.Data
{
    class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _param;

        private ParameterReplacer(ParameterExpression param)
        {
            _param = param;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node.Type == _param.Type ? base.VisitParameter(_param) : node;
        }

        public static TType Replace<TType>(ParameterExpression param, TType exp) where TType : Expression
        {
            return (TType)new ParameterReplacer(param).Visit(exp);
        }
    }
}
