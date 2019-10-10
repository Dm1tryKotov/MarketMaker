using System.Globalization;
using System.Windows.Controls;

namespace MMS.Domain
{
    public class IsIntegerValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            
            return !int.TryParse((value ?? "").ToString(), out int n)
                ? new ValidationResult(false, "значение должно быть целочисленным")
                : ValidationResult.ValidResult;
        }
    }
}
