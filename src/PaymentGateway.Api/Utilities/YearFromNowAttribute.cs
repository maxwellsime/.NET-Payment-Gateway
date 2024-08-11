using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Utilities;

public class YearFromNowAttribute : ValidationAttribute
{
    private readonly string _errorMessage;

    public YearFromNowAttribute(string errorMessage)
    {
        _errorMessage = errorMessage;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var year = (int)value;

        return DateTime.Now.Year > year ?
            new ValidationResult(_errorMessage) :
            ValidationResult.Success;
    }
}