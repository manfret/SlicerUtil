using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Settings.Memento;

namespace Aura.Controls.Util
{
    public class FiberInfillTypeToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            FiberInfillType? type = null;
            FiberInfillType? parameterFiberInfillType = null;
            bool? additionalVisibility = null;

            if (values[0] is FiberInfillType) type = (FiberInfillType) values[0];
            if (values[1] is FiberInfillType) type = (FiberInfillType) values[1];

            if (values[0] is bool) additionalVisibility = (bool)values[0];
            if (values[1] is bool) additionalVisibility = (bool)values[1];

            if (parameter is FiberInfillType) parameterFiberInfillType = (FiberInfillType) parameter;

            if (type == null || parameterFiberInfillType == null || type != parameterFiberInfillType) return Visibility.Collapsed;

            if (additionalVisibility == null) return Visibility.Visible;
            return additionalVisibility.Value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    public class FiberInfillTypeInvertToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            FiberInfillType? type = null;
            FiberInfillType? parameterFiberInfillType = null;
            bool? additionalVisibility = null;

            if (values[0] is FiberInfillType) type = (FiberInfillType)values[0];
            if (values[1] is FiberInfillType) type = (FiberInfillType)values[1];

            if (values[0] is bool) additionalVisibility = (bool)values[0];
            if (values[1] is bool) additionalVisibility = (bool)values[1];

            if (parameter is FiberInfillType) parameterFiberInfillType = (FiberInfillType)parameter;

            if (type == null || parameterFiberInfillType == null || type == parameterFiberInfillType) return Visibility.Collapsed;

            if (additionalVisibility == null) return Visibility.Visible;
            return additionalVisibility.Value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}