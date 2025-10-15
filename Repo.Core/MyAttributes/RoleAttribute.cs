using System.Collections;
using System.ComponentModel.DataAnnotations;
using Repo.Core.Models.auth;

namespace Repo.Core.MyAttributes;

public class RoleAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value is null)
            return new ValidationResult("Role is required");

        if (value is ICollection<string> values)
        {
            var invalidValues = values
                .Where(x => !Roles.TryParse(typeof(Roles), x, true, out _)).ToList();

            if (invalidValues.Any())
            {
                return new ValidationResult("Invalid role: "+string.Join(", ", invalidValues));
            }
            
            return ValidationResult.Success;
        }

        return new ValidationResult("Result is not a collection");
    }
}