using System.ComponentModel.DataAnnotations;

namespace Repo.Core.MyAttributes;

public class LoginAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value is null)
            return new ValidationResult("Login is required");
        if(value is not string stringValue )
            return new ValidationResult("Login needs to be a string");
        if(stringValue.Length < 6)
            return new ValidationResult("Login needs to be at least 6 characters");
        if(!char.IsUpper(stringValue[0]))
            return new ValidationResult("Login needs to start with an uppercase letter");
        if(stringValue.Length > 15)
            return new ValidationResult("Login needs to be less than 15 characters");
        
        return ValidationResult.Success;
    }
}