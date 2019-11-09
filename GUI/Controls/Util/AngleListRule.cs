using System.Globalization;
using System.Windows.Controls;
using Aura.Themes.Localization;
using Settings.Memento;

namespace Aura.Controls.Util
{
    public class AngleListRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var (res, _) = LayupRule.CheckAngles(value as string);
            return !res ? new ValidationResult(false, Errors_en_EN.OnlyAngles) : new ValidationResult(true, null);
        }
    }
}