using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PoliceDB.WPF.Converters
{
    public class RoleToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Visibility.Collapsed;

            // Преобразуем значение в строку
            string role = string.Empty;

            if (value is string roleString)
            {
                role = roleString;
            }
            else if (value is System.Windows.Controls.ComboBoxItem comboBoxItem)
            {
                role = comboBoxItem.Content?.ToString() ?? "";
            }
            else
            {
                role = value.ToString() ?? "";
            }

            // Возвращаем Visible для следователей, Collapsed для остальных
            return (role == "Следователь" || role == "Старший следователь")
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}