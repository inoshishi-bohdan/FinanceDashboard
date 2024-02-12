using System.Linq.Expressions;
using System.Reflection;

namespace FinanceDashboard.Shared
{
    public class Helpers
    {
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertySelector)
        {
            return (PropertyInfo)GetMemberExpression(propertySelector).Member;
        }

        private static MemberExpression GetMemberExpression<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertySelector)
        {
            if (propertySelector.Body is UnaryExpression unaryExpression)
            {
                return (MemberExpression)unaryExpression.Operand;
                //if (unaryExpression is System.Linq.Expressions.PropertyExpression)
            }

            if (propertySelector.Body is MemberExpression memberExpression)
            {
                return memberExpression;
            }

            throw new Exception($"Property selector {propertySelector} is not unary or member expression");
        }
    }
}
