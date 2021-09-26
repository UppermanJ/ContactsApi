using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Models;

namespace Services.Validators
{
    public class PhoneValidator :AbstractValidator<IEnumerable<Phone>>
    {
    }
}
