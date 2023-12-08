using System;
using System.Linq.Expressions;
using Montreal.Core.Crosscutting.Common.Data;

namespace Montreal.Core.Crosscutting.Common.Extensions
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Remove desnecessary casting in a expression
        /// </summary>
        /// <typeparam name="TType">Type of Expression to remove casting</typeparam>
        /// <param name="source">Expression to remove casting</param>
        /// <returns>Expression without casting</returns>
        public static LambdaExpression StripConvert<TType>(this Expression<Func<TType, object>> source)
        {
            var result = source.Body;

            while ((result.NodeType == ExpressionType.Convert || result.NodeType == ExpressionType.ConvertChecked)
                && (result.Type == typeof(object)))
            {
                result = ((UnaryExpression)result).Operand;
            }

            return Expression.Lambda(result, source.Parameters);
        }

        /// <summary>
        /// Replace parameter of a expression
        /// </summary>
        /// <typeparam name="TExpression">Type of Expression</typeparam>
        /// <param name="expression">Expression to replace</param>
        /// <param name="params">ParameterExpression</param>
        /// <returns>Expression with parameter replaced</returns>
        public static TExpression ReplaceParameter<TExpression>(this TExpression expression, ParameterExpression @params) where TExpression : Expression
        {
            return ParameterReplacer.Replace(@params, expression);
        }
    }
}
