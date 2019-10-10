using System.Globalization;
using System.Windows.Controls;

namespace MMS.Domain
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return string.IsNullOrWhiteSpace((value ?? "").ToString())
                ? new ValidationResult(false, "Поле не может быть пустым")
                : ValidationResult.ValidResult;
        }
    }
}
