using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Job_Portal_System.Validation
{
    public class MinimumCountAttribute : Attribute, IModelValidator
    {
        public string ErrorMessage { get; set; }
        private readonly int _minElements;
        public MinimumCountAttribute(int minElements)
        {
            _minElements = minElements;
        }

        public IEnumerable<ModelValidationResult> Validate(ModelValidationContext context)
        {
            if (context.Model is IList list && list.Count >= _minElements)
            { 
                return Enumerable.Empty<ModelValidationResult>();
            }
            return new List<ModelValidationResult>
            {
                new ModelValidationResult(context.ModelMetadata.PropertyName, ErrorMessage)
            };
        }
    }
}
