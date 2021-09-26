using System.Linq;
using FluentValidation;
using Models;

namespace Services.Validators
{
    public class ContactValidator: AbstractValidator<Contact>
    {
        public ContactValidator()
        {
            RuleFor(c => c.Phone)
                .Must(p => p.Select(p => p.Type).Distinct().Count() == p.Count())
                .WithMessage("Only one phone number per type is allowed");
            RuleForEach(c => c.Phone.Select(p => p.Type)).IsInEnum().WithName("Phone")
                .WithMessage("Phone Type must be either Home, Mobile or Work");            
            RuleForEach(c => c.Phone.Select(p => p.Number)).NotEmpty().WithName("Phone")
                .WithMessage("Phone number is required");

            RuleFor(c => c.Name.First).NotEmpty().WithMessage("First Name is required");
            RuleFor(c => c.Name.Last).NotEmpty().WithMessage("Last Name is required");
        }
    }
}
