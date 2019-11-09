using System.Globalization;
using System.Windows.Controls;
using Aura.Themes.Localization;

namespace Aura.Controls.ValidationRules
{
    public class IsDoubleRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var isDouble = double.TryParse(value.ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, cultureInfo, out double result);
            var res = isDouble ? new ValidationResult(true, null) : new ValidationResult(false, Errors_en_EN.OnlyDoubles);
            return res;
        }
    }

    public class DotRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var toDouble = double.TryParse(value.ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, cultureInfo, out double res);
            if (!toDouble) return new ValidationResult(false, Errors_en_EN.OnlyDoubles);

            if (res.ToString(cultureInfo) != value.ToString()) return new ValidationResult(false, Errors_en_EN.OnlyDoubles);

            return new ValidationResult(true, null);
        }
    }
}