using System.Linq.Expressions;
using System.Reflection;
using FinanceDashboard.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FinanceDashboard.Server
{
    public class FieldValidator
    {
        public static FieldValidator<TRequest> Create<TRequest>(TRequest request) where TRequest : class
        {
            return new FieldValidator<TRequest>(request);
        }
    }

    public class FieldValidator<TSource> where TSource : class
    {
        private readonly TSource _request;

        public FieldValidator(TSource request)
        {
            _request = request;
        }

        private readonly List<string> _validationErrors = new();

        public bool Any() => _validationErrors.Any();

        public BadRequestObjectResult BadRequest()
        {
            return new BadRequestObjectResult(string.Join("\n", _validationErrors.ToList()));
        }


        public FieldValidator<TSource> FieldIsRequired<TValue>(Expression<Func<TSource, TValue?>> propertySelector)
        {
            var value = propertySelector.Compile().Invoke(_request);

            var propertyInfo = Helpers.GetPropertyInfo(propertySelector);
            if (value == null)
            {
                AddErrorFieldIsRequired(propertyInfo);
            }

            return this;
        }

        private void AddErrorFieldIsRequired(PropertyInfo propertyInfo)
        {
            _validationErrors.Add($"Field {propertyInfo.Name} is required");
        }
    }
}
