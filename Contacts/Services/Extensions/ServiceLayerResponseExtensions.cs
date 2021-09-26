using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using FluentValidation.Results;
using Models.ServiceLayerResponseWrapper;

namespace Services.Extensions
{
    public static class ServiceLayerResponseExtensions
    {
        public static ServiceLayerResponseWrapper<T> AddValidationErrors<T>(this ServiceLayerResponseWrapper<T>  response, ValidationResult validationResult)
        {
            if (validationResult.Errors.Any() == false)
            {
                return response;
            }
            validationResult.Errors.ForEach(error =>
                response.AddInformation<ValidationError>($"{error.PropertyName}: {error.ErrorMessage}"));
            return response;
        }
    }
}
