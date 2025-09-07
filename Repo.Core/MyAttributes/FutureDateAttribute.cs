using System.ComponentModel.DataAnnotations;

namespace Repo.Core.MyAttributes;

public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        DateTime date = (DateTime)value;
        if (date < DateTime.Now)
        {
            return new ValidationResult("Date cannot be in the past");
        }
        return ValidationResult.Success;
    }
}