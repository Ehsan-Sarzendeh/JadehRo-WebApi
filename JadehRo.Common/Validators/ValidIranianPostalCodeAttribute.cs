using System.ComponentModel.DataAnnotations;
using Sahand.Common.Utilities.Validators;

namespace JadehRo.Common.Validators;

/// <summary>
/// Determines whether the specified value of the object is a valid IranianPostalCode.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public sealed class ValidIranianPostalCodeAttribute : ValidationAttribute
{
    /// <summary>
    /// Determines whether the specified value of the object is valid.
    /// </summary>
    public override bool IsValid(object value)
    {
        if (value == null || string.IsNullOrEmpty(value.ToString()))
        {
            return true; // returning false, makes this field required.
        }
        return value.ToString().IsValidIranianPostalCode();
    }
}