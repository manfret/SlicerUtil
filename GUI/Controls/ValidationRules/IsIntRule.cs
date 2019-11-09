using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Aura.Themes.Localization;

namespace Aura.Controls.ValidationRules
{
    public class IsIntRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return int.TryParse(value as string, out int result) ? new ValidationResult(true, null) : new ValidationResult(false, Errors_en_EN.OnlyIntegers);
        }
    }

    public class MinMaxRule : ValidationRule
    {
        public double Minimum { get; set; }
        public double Maximum { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var canConvert = double.TryParse(value.ToString(), NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, cultureInfo, out double result);
            if (!canConvert) return new ValidationResult(false, Errors_en_EN.OnlyDoubles);

            if (result <= Maximum && result >= Minimum) return new ValidationResult(true, null);
            return new ValidationResult(false, string.Format(Errors_en_EN.InTheRange, Minimum, Maximum));
        }
    }

    public class ShouldBeLessRule : ValidationRule
    {
        public ValueWrapper Wrapper { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var val = (double)value;
            return val > Wrapper.Value ? new ValidationResult(false, $"Should be less than {Wrapper.Value}") : new ValidationResult(true, null);
        }
    }

    public class ShouldBeMoreRule : ValidationRule
    {
        public ValueWrapper Wrapper { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var val = (double)value;
            return val < Wrapper.Value ? new ValidationResult(false, $"Should be more than {Wrapper.Value}") : new ValidationResult(true, null);
        }
    }

    public class EmailRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return (regex.IsMatch(value.ToString())) ? new ValidationResult(true, null) : new ValidationResult(false, Common_en_EN.WrongEmail);
        }
    }

    public class ValueWrapper : DependencyObject
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double),
                typeof(ValueWrapper));

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }

    public static class IllegasFileName
    {
        public static List<string> Illegals { get; private set; }

        static IllegasFileName()
        {
            Illegals = new List<string>
            {
                @"\",
                @"/",
                @":",
                @"*",
                @"?",
                "\"",
                @"CON",
                @"PRN",
                @"AUX",
                @"NUL",
                @"COM1",
                @"COM2",
                @"COM3",
                @"COM4",
                @"COM5",
                @"COM6",
                @"COM7",
                @"COM8",
                @"COM9",
                @"LPT1",
                @"LPT2",
                @"LPT3",
                @"LPT4",
                @"LPT5",
                @"LPT6",
                @"LPT7",
                @"LPT8",
                @"LPT9"
            };
        }

        public static bool CheckForValidity(string value)
        {
            if (value == null || !value.Any()) return false;
            foreach (var illegal in Illegals)
            {
                if (value.Contains(illegal)) return false;
            }
            return true;
        }
    }

    public class NameValidator : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (value == null) return new ValidationResult(false, "value cannot be empty.");
            else
            {
                var path = value.ToString();
                var isValid = IllegasFileName.CheckForValidity(path);
                return !isValid ? new ValidationResult(false, $"Name should not have symbols {string.Join(",", IllegasFileName.Illegals)}") : new ValidationResult(true, null);
            }
        }
    }
}
